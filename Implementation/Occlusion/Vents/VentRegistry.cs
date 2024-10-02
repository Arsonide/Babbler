using System;
using System.Collections.Generic;
using Babbler.Implementation.Config;
using Babbler.Implementation.Occlusion.Vents.Explorer;
using SOD.Common;
using UnityEngine;

namespace Babbler.Implementation.Occlusion.Vents;

public static class VentRegistry
{
    // I don't see the point of making this configurable. Players won't really understand.
    private const int REGISTRATION_BATCH_COUNT = 5;

    private static bool _occlusionEnabled;
    
    private static readonly Queue<VentTagCache> _unregisteredTags = new Queue<VentTagCache>();
    private static readonly Dictionary<int, List<VentTagCache>> _registeredTags = new Dictionary<int, List<VentTagCache>>();
    private static readonly List<int> _registeredRooms = new List<int>();
    private static readonly List<List<VentTagCache>> _registrationPool = new List<List<VentTagCache>>();
    
    private static readonly HashSet<Vector3Int> _nearbyVentCoordinates = new HashSet<Vector3Int>();
    private static Vector3Int _lastNearbyCheck = Vector3Int.zero;
    
    private static readonly DuctExplorer _explorer = new DuctExplorer();

    public static void Initialize()
    {
        _occlusionEnabled = BabblerConfig.OcclusionEnabled.Value;
        
        if (!_occlusionEnabled)
        {
            return;
        }
        
        // We add these to these prefabs while in the main menu, to allow them to register when they spawn in-game.
        InteriorControls.Instance.ductStraightWithPeekVent.AddComponent<VentTagPeekable>();
        PrefabControls.Instance.airVent.prefab.AddComponent<VentTagInteractable>();

        Lib.SaveGame.OnBeforeLoad -= OnBeforeEnterGame;
        Lib.SaveGame.OnBeforeLoad += OnBeforeEnterGame;
        
        Lib.SaveGame.OnBeforeNewGame -= OnBeforeEnterGame;
        Lib.SaveGame.OnBeforeNewGame += OnBeforeEnterGame;
    }
    
    public static void Uninitialize()
    {
        if (!_occlusionEnabled)
        {
            return;
        }
        
        Lib.SaveGame.OnBeforeLoad -= OnBeforeEnterGame;
        Lib.SaveGame.OnBeforeNewGame -= OnBeforeEnterGame;
    }
    
    private static void OnBeforeEnterGame(object sender, EventArgs eventArgs)
    {
        ClearRegistrations();
        _unregisteredTags.Clear();
    }

    public static void Tick()
    {
        if (!_occlusionEnabled || PathFinder.Instance == null)
        {
            return;
        }
     
        int allowedToProcess = REGISTRATION_BATCH_COUNT;

        while (_unregisteredTags.Count > 0 && allowedToProcess > 0)
        {
            VentTagCache tag = _unregisteredTags.Dequeue();

            if (PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(tag.TransformPosition), out NewNode node) && node?.room != null)
            {
                tag.Node = node;
                tag.Room = node.room;
                RegisterVent(tag.Room.roomID, tag);
            }
            
            allowedToProcess--;
        }
    }
    
    public static bool TryGetVents(NewRoom room, out List<VentTagCache> vents)
    {
        return _registeredTags.TryGetValue(room.roomID, out vents);
    }
    
    public static void CacheNearbyVentCoordinates(AirDuctGroup.AirDuctSection playerDuct)
    {
        Vector3Int nodeCoord = playerDuct.node.nodeCoord;

        if (nodeCoord == _lastNearbyCheck)
        {
            return;
        }
        
        _lastNearbyCheck = nodeCoord;
        
        _explorer.Reset();
        _explorer.StartExploration(playerDuct);

        for (int i = 0, iC = BabblerConfig.OcclusionVentRange.Value; i < iC; ++i)
        {
            List<Vector3Int> ventOpeningCoordinates = _explorer.TickExploration(playerDuct);

            foreach (Vector3Int ventOpeningCoordinate in ventOpeningCoordinates)
            {
                _nearbyVentCoordinates.Add(ventOpeningCoordinate);
            }
        }
    }

    public static bool IsVentNearby(VentTagCache tag)
    {
        return _nearbyVentCoordinates.Contains(tag.Node.nodeCoord);
    }

    public static void QueueVentTagRegistration(VentTagCache tag)
    {
        _unregisteredTags.Enqueue(tag);
    }
    
    private static void RegisterVent(int roomId, VentTagCache tag)
    {
        if (!_registeredTags.TryGetValue(roomId, out List<VentTagCache> list))
        {
            list = GetPooledList();
            list.Add(tag);
            _registeredTags.Add(roomId, list);
            _registeredRooms.Add(roomId);
        }
        else
        {
            list.Add(tag);
        }
    }

    private static void ClearRegistrations()
    {
        for (int i = _registeredRooms.Count - 1; i >= 0; --i)
        {
            int room = _registeredRooms[i];
            ReturnPooledList(_registeredTags[room]);
            _registeredTags.Remove(room);
            _registeredRooms.RemoveAt(i);
        }
    }

    private static List<VentTagCache> GetPooledList()
    {
        List<VentTagCache> list;
        
        if (_registrationPool.Count > 0)
        {
            int end = _registrationPool.Count - 1;
            list = _registrationPool[end];
            _registrationPool.RemoveAt(end);
        }
        else
        {
            list = new List<VentTagCache>();
        }

        return list;
    }
    
    private static void ReturnPooledList(List<VentTagCache> list)
    {
        list.Clear();
        _registrationPool.Add(list);
    }
}