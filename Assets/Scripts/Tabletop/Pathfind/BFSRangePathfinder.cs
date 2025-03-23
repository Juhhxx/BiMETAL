using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// gets any cell you can walk to with total weight, accounting for avoidables and walkables
/// </summary>
public class BFSRangePathfinder : Pathfinder
{
    public BFSRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    private HashSet<HexagonCell> _set;

    /// <summary>
    /// objective is the current cell and start is the objective
    /// </summary>
    /// <param name="objective"></param>
    /// <param name="start"></param>
    /// <param name="totalWeight"></param>
    /// <returns></returns>
    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        Done = false;

        _data.Clear();
        _closedList.Clear();
        OpenList.Clear();
        Path.Clear();

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

            yield return null;
        }

        foreach (HexagonCell cell in _set)
            Path.ObservePush(cell);

        Done = true;
    }
}