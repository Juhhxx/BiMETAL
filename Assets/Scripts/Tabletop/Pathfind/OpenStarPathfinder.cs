using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gets an AStars' open list as the path
/// </summary>
public class OpenStarPathfinder : Pathfinder
{

    public OpenStarPathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        // objective and start are switched

        Done = false;

        _data[start] = new CellData(start, start.Weight, start.GetDistance(objective), null);

        OpenList.Add(_data[start]);

        HexagonCell current = OpenList.Min.Cell;

        while (OpenList.Any())
        {
            current = OpenList.Min.Cell;
            OpenList.Remove(OpenList.Min);
            _closedList.Add(current);

            Path.ObservePush(current);

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => t == objective && !_closedList.Contains(t)))
            {
                float costToNeighbor = 0;


                if (neighbor != objective)
                    costToNeighbor = _data[current].G + neighbor.Weight;

                // needs to use bfs to find a shot based path to a cell near it


                if (!_data.TryGetValue(neighbor, out CellData cellData))
                {
                    cellData = new CellData(neighbor, float.MaxValue, neighbor.GetDistance(objective), null);
                    _data[neighbor] = cellData;
                }

                if (costToNeighbor < cellData.G)
                {
                    // Need to remove and readd the element from the sorted list so that it will reorder based on new F and H
                    OpenList.Remove(cellData);

                    cellData.G = costToNeighbor;

                    cellData.H = neighbor.GetDistance(objective);
                    // cellData.H = Vector2.Distance(neighbor.CellValue, objective.CellValue);

                    cellData.Connection = _data[current];

                    // readd
                    OpenList.Add(cellData);
                }
            }

            yield return null;
        }

        // Debug.Log("No path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }
}
