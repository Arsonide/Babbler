using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Babbler.Implementation.Config;
using Babbler.Implementation.Occlusion.Vents;
using UnityEngine;

namespace Babbler.Implementation.Occlusion;

public static class OcclusionChecker
{
    private static Vector3Int _occlusionBoundsMin;
    private static Vector3Int _occlusionBoundsMax;

#region Occlusion
    
    public static OcclusionResult CheckOcclusion(Human speaker, Human listener)
    {
        if (!BabblerConfig.OcclusionEnabled.Value)
        {
            return OcclusionResult.CreateNoOcclusion();
        }
        
        // Quickly discount people over two blocks away.
        if (!speaker.currentCityTile.isInPlayerVicinity)
        {
            return OcclusionResult.CreateFullOcclusion();
        }

        bool speakerOutside = IsHumanOutside(speaker);
        bool listenerOutside = IsHumanOutside(listener);
        
        // Both outside, all we can do is check distance.
        if (speakerOutside && listenerOutside)
        {
            return IsHumanInOcclusionBounds(speaker) ? OcclusionResult.CreateNoOcclusion() : OcclusionResult.CreateFullOcclusion();
        }
        
        // One is outside, one isn't, check the threshold.
        if (speakerOutside ^ listenerOutside)
        {
            return IsHumanInOcclusionBounds(listener) ? CalculateOcclusionDoorMuffleResult(speaker, listener, true) : OcclusionResult.CreateFullOcclusion();
        }

        // Both are not on the street at this point, and they are not in the same building.
        if (speaker.currentBuilding == null || speaker.currentBuilding.buildingID != listener.currentBuilding.buildingID)
        {
            return OcclusionResult.CreateFullOcclusion();
        }

        // After this point they are in the same building. These checks are needed regardless of lobby state.
        if (Mathf.Abs(speaker.currentNodeCoord.z - listener.currentNodeCoord.z) > 3 || !IsHumanInOcclusionBounds(speaker))
        {
            return OcclusionResult.CreateFullOcclusion();
        }

        // The player is in a vent, we have a special method for this.
        if (listener.inAirVent)
        {
            return CalculateOcclusionVentResult(speaker, listener);
        }
        
        // If we're both in the lobby just let FMOD do its thing.
        if (speaker.currentGameLocation.isLobby && listener.currentGameLocation.isLobby)
        {
            return OcclusionResult.CreateNoOcclusion();
        }

        bool lobbyThreshold = listener.currentGameLocation.isLobby || speaker.currentGameLocation.isLobby;

        // I shouldn't hear my neighbor's farts.
        if (!lobbyThreshold && speaker.currentGameLocation.thisAsAddress.id != listener.currentGameLocation.thisAsAddress.id)
        {
            return OcclusionResult.CreateFullOcclusion();
        }
        
        // Either we're crossing a threshold from lobby to apartment, or rooms within an apartment. It doesn't matter. There's a neighboring room the sound originates in.
        return CalculateOcclusionDoorMuffleResult(speaker, listener, lobbyThreshold);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OcclusionResult CalculateOcclusionDoorMuffleResult(Human speaker, Human listener, bool crossThreshold)
    {
        NewRoom speakerRoom = speaker.currentRoom;
        NewRoom listenerRoom = listener.currentRoom;
        
        if (speakerRoom.roomID == listenerRoom.roomID)
        {
            return OcclusionResult.CreateNoOcclusion();
        }

        // Just in case there are multiple doors connecting our rooms, we'll keep a running sum to average their positions.
        int openCount = 0;
        int closedCount = 0;
        
        Vector3 openPositions = Vector3.zero;
        Vector3 closedPositions = Vector3.zero;
        
        foreach (NewNode.NodeAccess entrance in speakerRoom.entrances)
        {
            // There are other access types other than doors and windows, but the game treats them all as open doors.
            if (entrance.accessType == NewNode.NodeAccess.AccessType.window)
            {
                continue;
            }
            
            if (entrance.GetOtherRoom(speakerRoom).roomID != listenerRoom.roomID)
            {
                continue;
            }

            bool open = entrance.door == null || !entrance.door.isClosed;

            if (open)
            {
                openCount++;
                openPositions += entrance.worldAccessPoint;
            }
            else
            {
                closedCount++;
                closedPositions += entrance.worldAccessPoint;
            }
        }
        
        if (openCount + closedCount > 0)
        {
            OcclusionState targetState;
            Vector3 doorAverage;
            
            // Average the positions of all connecting doors, if there are more than one, this isn't perfect but it is good enough.
            if (openCount > 0)
            {
                targetState = OcclusionState.MuffleOpenDoor;
                doorAverage = openPositions / openCount;
            }
            else
            {
                targetState = OcclusionState.MuffleClosedDoor;
                doorAverage = closedPositions / closedCount;
            }
            
            // Project the sound "behind" the door relative to the listener to keep the distance attenuation.
            Vector3 listenerToDoor = doorAverage - listener.aimTransform.position;
            float soundToDoorDistance = Vector3.Distance(speaker.aimTransform.position, doorAverage);
            Vector3 projectedPosition = doorAverage + listenerToDoor.normalized * soundToDoorDistance;
            
            return new OcclusionResult
            {
                State = targetState, AlternativePosition = true, Position = projectedPosition,
            };
        }

        // Assume IsHumanInOcclusionBounds has been called prior to calling this more complex method, so the sound is nearby, just not in a neighboring room.
        return crossThreshold ? OcclusionResult.CreateFullOcclusion() : OcclusionResult.CreateDistantOcclusion();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OcclusionResult CalculateOcclusionVentResult(Human speaker, Human listener)
    {
        if (!VentRegistry.TryGetVents(speaker.currentRoom, out List<VentTagCache> speakerRoomVents) || speakerRoomVents.Count <= 0)
        {
            return OcclusionResult.CreateFullOcclusion();
        }
        
        VentRegistry.CacheNearbyVentCoordinates(Player.Instance.currentDuctSection);
        int nearbyCount = 0;
        Vector3 nearbyPositions = Vector3.zero;
        
        foreach (VentTagCache speakerRoomVent in speakerRoomVents)
        {
            if (!VentRegistry.IsVentNearby(speakerRoomVent))
            {
                continue;
            }

            nearbyCount++;
            nearbyPositions += speakerRoomVent.AudioPosition;
        }
        
        if (nearbyCount <= 0)
        {
            return OcclusionResult.CreateFullOcclusion();
        }

        Vector3 ventAverage = nearbyPositions / nearbyCount;

        // Project the sound "behind" the door relative to the listener to keep the distance attenuation.
        Vector3 listenerToVent = ventAverage - listener.aimTransform.position;
        float soundToVentDistance = Vector3.Distance(speaker.aimTransform.position, ventAverage);
        Vector3 projectedPosition = ventAverage + listenerToVent.normalized * soundToVentDistance;
            
        return new OcclusionResult
        {
            State = OcclusionState.MuffleVent, AlternativePosition = true, Position = projectedPosition,
        };
    }
    
#endregion

#region Bounds
    
    public static void CachePlayerBounds()
    {
        Vector3Int playerNode = Player.Instance.currentNodeCoord;
        int range = BabblerConfig.OcclusionNodeRange.Value;
        
        _occlusionBoundsMin = playerNode - new Vector3Int(range, range, range);
        _occlusionBoundsMax = playerNode + new Vector3Int(range, range, range);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsHumanInOcclusionBounds(Human speaker)
    {
        return speaker.currentNodeCoord.x >= _occlusionBoundsMin.x && speaker.currentNodeCoord.x <= _occlusionBoundsMax.x &&
               speaker.currentNodeCoord.y >= _occlusionBoundsMin.y && speaker.currentNodeCoord.y <= _occlusionBoundsMax.y &&
               speaker.currentNodeCoord.z >= _occlusionBoundsMin.z && speaker.currentNodeCoord.z <= _occlusionBoundsMax.z;
    }
    
#endregion
    
#region Utilities

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsHumanOutside(Human human)
    {
        if (human.inAirVent)
        {
            return false;
        }
        
        return human.isOnStreet || human.currentNode.isOutside || human.currentRoom.IsOutside();
    }
    
#endregion
}