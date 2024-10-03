using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Hosts;
using Il2CppSystem.Collections.Generic;
using SOD.Common;
using SOD.Common.Helpers;

namespace Babbler.Implementation.Emotes;

public static class TVWatcher
{
    private static AIActionPreset _turnOnTVPreset;
    
    public static void Initialize()
    {
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return;
        }
        
        Lib.Time.OnMinuteChanged -= Tick;
        Lib.Time.OnMinuteChanged += Tick;
    }

    public static void Uninitialize()
    {
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return;
        }
        
        Lib.Time.OnMinuteChanged -= Tick;
    }

    private static void Tick(object sender, TimeChangedArgs args)
    {
        if (Utilities.IsHumanOutside(Player.Instance))
        {
            return;
        }

        NewBuilding playerBuilding = Player.Instance.currentBuilding;
        
        if (playerBuilding == null)
        {
            return;
        }

        NewRoom playerRoom = Player.Instance.currentRoom;

        if (playerRoom == null)
        {
            return;
        }
        
        int playerFloorIndex = playerRoom.floor.floor;
        
        for (int floorLevel = playerFloorIndex - 3; floorLevel <= playerFloorIndex + 3; ++floorLevel)
        {
            if (!playerBuilding.floors.TryGetValue(floorLevel, out NewFloor floor))
            {
                continue;
            }

            foreach (NewAddress address in floor.addresses)
            {
                foreach (Actor occupant in address.currentOccupants)
                {
                    TickOccupant(occupant as Human);
                }
            }
        }
    }

    private static void TickOccupant(Human occupant)
    {
        if (occupant == null || occupant.isPlayer || !occupant.isHome)
        {
            return;
        }
        
        if (occupant.animationController.idleAnimationState != CitizenAnimationController.IdleAnimationState.sitting)
        {
            return;
        }

        if (!EmoteSoundRegistry.CanPlayIncidentals(occupant, true))
        {
            return;
        }
        
        if (!IsTelevisionOn(occupant.currentRoom))
        {
            return;
        }
        
        if (!EmoteSoundRegistry.IsEmoteRelevantBroadphase(occupant))
        {
            return;
        }
        
        if (!EmoteSoundRegistry.ShouldPlayExtravertedEmote(occupant, BabblerConfig.IncidentalsMinTVChance.Value, BabblerConfig.IncidentalsMaxTVChance.Value))
        {
            return;
        }
        
        SpeakerHostPool.Emotes.Play("chuckle", SoundContext.OverheardEmote, occupant);
    }

    private static bool IsTelevisionOn(NewRoom room)
    {
        if (_turnOnTVPreset == null)
        {
            foreach (KeyValuePair<AIActionPreset, List<Interactable>> pair in room.actionReference)
            {
                if (pair.Key.presetName != "TurnOnTV")
                {
                    continue;
                }

                _turnOnTVPreset = pair.Key;
                break;
            }

            if (_turnOnTVPreset == null)
            {
                return false;
            }
        }
        
        if (!room.actionReference.TryGetValue(_turnOnTVPreset, out List<Interactable> interactables))
        {
            return false;
        }
        
        foreach (Interactable interactable in interactables)
        {
            if (interactable.sw0)
            {
                return true;
            }
        }

        return false;
    }
}