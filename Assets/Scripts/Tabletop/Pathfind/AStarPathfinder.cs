using System.Collections;
using System.Linq;
using UnityEngine;

public class AStarPathfinder : Pathfinder
{

    public AStarPathfinder(HexagonTabletop tabletop, MonoBehaviour owner) : base(tabletop, owner)
    {}

    public override IEnumerator GetPath(HexagonCell objective, HexagonCell start)
    {
        // objective and start are switched

        Done = false;

        _data[start] = new CellData(start, 0, start.GetDistance(objective), null);

        OpenList.Add(_data[start]);

        Debug.Log("Starting pathfinding from " + start + " to " + objective);
        Debug.Log("Finding path. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        /*foreach(HexagonCell cell in _data.Keys)
        {
            Debug.Log("                 Data stats for " + cell + "  with " + _data[cell] + " and walkable: " + cell.Walkable);
        }*/

        while (OpenList.Any())
        {
            HexagonCell current = OpenList.Min.Cell;
            OpenList.Remove(OpenList.Min);
            _closedList.Add(current);

            if (current == objective)
            {
                HexagonCell currentCell = objective;
                while (currentCell != start)
                {
                    Path.Push(currentCell);
                    currentCell = _data[currentCell].Connection;
                }

                Path.Push(start);
                Done = true;
                yield break;
            }

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => (t.Walkable || t == objective) && !_closedList.Contains(t)))
            {
                // by switching the distance here by Points (which is how many points it takes to cross times the distance)
                // we can reorganize pathfinding to account for how much it costs to move
                // it calculates how much to walk to the next one from the current (not to neighbor)
                float costToNeighbor = _data[current].G + current.Points;
                // the tabletop movement is then responsible for curating how far they go (not the pathfinder)

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
                    cellData.Connection = current;

                    // readd
                    OpenList.Add(cellData);
                }
            }

            yield return null;
        }

        Debug.Log("No path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }
}
