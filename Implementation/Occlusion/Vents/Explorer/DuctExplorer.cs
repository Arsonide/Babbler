using System.Collections.Generic;
using UnityEngine;

namespace Babbler.Implementation.Occlusion.Vents.Explorer;

/// <summary>
/// This is a slightly modified version of DuctExplorer from Ventrix Sync Disks to help us walk the vent network.
/// </summary>
public class DuctExplorer
{
    private readonly Queue<AirDuctGroup.AirDuctSection> _queue = new Queue<AirDuctGroup.AirDuctSection>();
    private readonly HashSet<Vector3Int> _visited = new HashSet<Vector3Int>();
    private readonly List<Vector3Int> _results = new List<Vector3Int>();

    private List<AirDuctGroup.AirDuctSection> _neighbors = new List<AirDuctGroup.AirDuctSection>();
    private List<Vector3Int> _neighborOffsets = new List<Vector3Int>();
    private List<AirDuctGroup.AirVent> _vents = new List<AirDuctGroup.AirVent>();

    public void Reset()
    {
        _queue.Clear();
        _visited.Clear();
        _results.Clear();
        _neighbors.Clear();
        _vents.Clear();
    }
    
    public void StartExploration(AirDuctGroup.AirDuctSection startSection)
    {
        _queue.Enqueue(startSection);
        _visited.Add(startSection.duct);
    }

    public List<Vector3Int> TickExploration(AirDuctGroup.AirDuctSection duct)
    {
	    // Recursively tick over everything.
	    _results.Clear();
        
        if (_queue.Count <= 0)
        {
            return _results;
        }

        int nodesInCurrentLevel = _queue.Count;

        for (int i = 0; i < nodesInCurrentLevel; i++)
        {
            AirDuctGroup.AirDuctSection currentDuct = _queue.Dequeue();
            GetVentInformation(currentDuct, ref _neighbors, ref _neighborOffsets, ref _vents);

            for (int j = 0; j < _neighbors.Count; ++j)
            {
                AirDuctGroup.AirDuctSection neighbor = _neighbors[j];

                if (_visited.Contains(neighbor.duct))
                {
                    continue;
                }

                _queue.Enqueue(neighbor);
                _visited.Add(neighbor.duct);
            }

            bool addedCurrent = false;
            
            foreach (AirDuctGroup.AirVent vent in _vents)
            {
	            if (vent.ventType == NewAddress.AirVent.ceiling)
	            {
		            // Ceiling vents are in the same node, positionally, as the duct.
		            _results.Add(currentDuct.node.nodeCoord);
		            addedCurrent = true;
	            }
	            else
	            {
		            // Wall vents are in the connected room node, positionally.
		            _results.Add(vent.roomNode.nodeCoord);
	            }
            }

            if (currentDuct.peekSection && !addedCurrent)
            {
	            _results.Add(currentDuct.node.nodeCoord);
            }
        }

        return _results;
    }
    
#region Vent Information
	
    private void GetVentInformation(AirDuctGroup.AirDuctSection thisDuct, ref List<AirDuctGroup.AirDuctSection> neighbors, ref List<Vector3Int> neighborOffsets, ref List<AirDuctGroup.AirVent> vents)
    {
	    // I did not write this convoluted logic, I needed to appropriate it to optimize it a little bit though.
	    neighbors.Clear();
	    neighborOffsets.Clear();
	    vents.Clear();

	    foreach (Vector3Int offset in CityData.Instance.offsetArrayX6)
	    {
		    Vector3Int offsetCoordinate = thisDuct.duct + offset;

		    if (!thisDuct.node.building.ductMap.TryGetValue(offsetCoordinate, out AirDuctGroup.AirDuctSection offsetDuct))
		    {
			    continue;
		    }

		    bool isNeighbor = offset == thisDuct.next || offset == thisDuct.previous || offsetDuct.next == -offset || offsetDuct.previous == -offset;

		    if (isNeighbor && !neighbors.Contains(offsetDuct))
		    {
			    neighbors.Add(offsetDuct);
			    neighborOffsets.Add(offset);
		    }
	    }
	    
	    if (thisDuct.level == 2)
	    {
		    AirDuctGroup.AirVent airVent = FindCeilingVent(thisDuct.node);

		    if (airVent != null && !vents.Contains(airVent))
		    {
			    vents.Add(airVent);
		    }

		    return;
	    }
	    
	    foreach (Vector2Int lateralOffset in CityData.Instance.offsetArrayX4)
	    {
		    Vector3Int lateralOffset2D = new Vector3Int(lateralOffset.x, lateralOffset.y, 0);
		    Vector3Int foundCoordinate = thisDuct.node.nodeCoord + lateralOffset2D;

		    if (!PathFinder.Instance.nodeMap.TryGetValue(foundCoordinate, out NewNode foundNode))
		    {
			    continue;
		    }

		    AirDuctGroup.AirVent airVent = FindWallVent(foundNode, thisDuct.node);

		    if (airVent == null)
		    {
			    continue;
		    }

		    if (airVent.ventType == NewAddress.AirVent.wallUpper && thisDuct.level == 1)
		    {
			    if (!vents.Contains(airVent))
			    {
				    vents.Add(airVent);
			    }
		    }
		    else if (airVent.ventType == NewAddress.AirVent.wallLower && thisDuct.level == 0)
		    {
			    if (!vents.Contains(airVent))
			    {
				    vents.Add(airVent);
			    }
		    }
	    }
    }

    private AirDuctGroup.AirVent FindCeilingVent(NewNode ductNode)
    {
	    foreach (AirDuctGroup.AirVent vent in ductNode.room.airVents)
	    {
		    if (vent.node.nodeCoord == ductNode.nodeCoord && vent.ventType == NewAddress.AirVent.ceiling)
		    {
			    return vent;
		    }
	    }

	    return null;
    }
    
    private AirDuctGroup.AirVent FindWallVent(NewNode foundNode, NewNode ductNode)
    {
	    foreach (AirDuctGroup.AirVent vent in foundNode.room.airVents)
	    {
		    if (vent.node.nodeCoord == ductNode.nodeCoord && vent.roomNode.nodeCoord == foundNode.nodeCoord)
		    {
			    return vent;
		    }
	    }

	    return null;
    }
    
#endregion
}