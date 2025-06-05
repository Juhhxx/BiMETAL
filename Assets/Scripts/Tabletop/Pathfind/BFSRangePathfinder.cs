using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A breadth first pathfinder that finds all reachable hex cells from a starting position
/// based on a total movement weight to highlight all reachable cells from a given reach.
/// </summary>
public class BFSRangePathfinder : Pathfinder
{
    public BFSRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    private HashSet<HexagonCell> _set;

    /// <summary>
    /// Begins the weighted breadth first search from the objective cell, collecting all reachable neighbors
    /// within the movement weight budget.
    /// </summary>
    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        Done = false;

        _set = new HashSet<HexagonCell>();

        // objective is start as stated before
        _data[objective] = new CellData(objective, 0f, 0f, null);
        OpenList.Add(_data[objective]);

        while (OpenList.Any())
        {
            HexagonCell current = OpenList.Min.Cell;
            OpenList.Remove(OpenList.Min);
            _closedList.Add(current);

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => t != null && !_closedList.Contains(t) &&
                ( t.Walkable() || (_includeNonAvoidance && t.IsNonAvoidable())) ))
            {

                float costToNeighbor = _data[current].G + neighbor.Weight;

                // if the cost is too great we don't want this neighbor
                if (costToNeighbor > totalWeight)
                    continue;

                if (!_data.TryGetValue(neighbor, out CellData cellData))
                {
                    cellData = new CellData(neighbor, float.MaxValue, 0f, null);
                    _data[neighbor] = cellData;
                }

                // if its better than the last cost, we update it
                if (costToNeighbor < cellData.G)
                {
                    OpenList.Remove(cellData);

                    cellData.G = costToNeighbor;
                    
                    OpenList.Add(cellData);
                    
                    _set.Add(neighbor);
                }
            }

            // consuming a lot of time again
            // yield return null;
        }

        foreach (HexagonCell cell in _set)
            if ( cell != objective )
                Path.ObservePush(cell);

        yield return null;

        Done = true;
    }
}