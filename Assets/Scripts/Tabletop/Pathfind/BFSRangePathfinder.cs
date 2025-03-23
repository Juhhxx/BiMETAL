using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BFSRangePathfinder : Pathfinder
{

    public BFSRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    private HashSet<HexagonCell> _set;
    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        Done = false;

        _data[start] = new CellData(start, start.Weight, start.GetDistance(objective), null);
        OpenList.Add(_data[start]);

        _set = new();


        HexagonCell current = OpenList.Min.Cell;

        while (OpenList.Any())
        {
            current = OpenList.Min.Cell;
            OpenList.Remove(OpenList.Min);
            _closedList.Add(current);

            // when open list is done, add hash set bfs to path
            if ( OpenList.Count <= 0 )
            {
                HexagonCell currentCell;

                while ( _set.Any() )
                {
                    currentCell = _set.Last();
                    Path.ObservePush(currentCell);
                }
                // Path.ObservePush(start);

                Done = true;
                yield break;
            }

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => (t.Walkable() ||
                (_includeNonAvoidance && t.IsNonAvoidable()) ||
                t == objective) && !_closedList.Contains(t)))
            {
                float costToNeighbor = 0;

                if (neighbor != objective)
                    costToNeighbor = _data[current].G + neighbor.Weight;

                if (!_data.TryGetValue(neighbor, out CellData cellData))
                {
                    cellData = new CellData(neighbor, float.MaxValue, neighbor.GetDistance(objective), null);
                    _data[neighbor] = cellData;
                }

                if ( costToNeighbor < cellData.G && costToNeighbor <= totalWeight)
                {
                    // Need to remove and readd the element from the sorted list so that it will reorder based on new F and H
                    OpenList.Remove(cellData);

                    cellData.G = costToNeighbor;
                    // readd
                    OpenList.Add(cellData);

                    // had bfs correct path to here
                    _set.Add(neighbor);
                }
            }

            yield return null;
        }

        Done = true;
    }
}