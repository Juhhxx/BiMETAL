using System.Collections;
using System.Linq;
using UnityEngine;

public class AStarPathfinder : Pathfinder
{

    public AStarPathfinder(HexagonTabletop tabletop, MonoBehaviour owner) : base(tabletop, owner)
    {}

    public override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        // objective and start are switched

        Done = false;

        _data[start] = new CellData(start, start.Weight, start.GetDistance(objective), null);

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
                    Path.ObservePush(currentCell);
                    currentCell = _data[currentCell].Connection.Cell;
                    // Debug.Log("this cell G: " + currentCell + " " + _data[currentCell].G);
                }

                Path.ObservePush(start);
                // Debug.Log("this start cell G: " + start + " " + _data[start].G);

                // while ( Path.Count > 0 )
                //     Debug.Log("this cell: " + Path.Peek() + "    is start? " + (Path.Peek() == start) + "    is objective? " + (Path.Pop() == objective));
                
                Done = true;
                yield break;
            }

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => (t.Walkable || t == objective) && !_closedList.Contains(t)))
            {
                // by switching the distance here by Points (which is how many points it takes to cross times the distance)
                // we can reorganize pathfinding to account for how much it costs to move
                // it calculates how much to walk to the next one from the current (not to neighbor)
                
                // neighbor points or current points?
                // float costToNeighbor = _data[current].G + neighbor.Weight;
                float costToNeighbor = 0;

                if ( neighbor != objective ) 
                    costToNeighbor = _data[current].G + neighbor.Weight;
                // the tabletop movement is then responsible for curating how far they go (not the pathfinder)

                // Debug.Log("cost: " + costToNeighbor + "     neighbor points: " + neighbor.Weight + "    total cost: " + totalWeight);

                if ( totalWeight != -1 && costToNeighbor > totalWeight ) continue;

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
                    // Do we need this line?
                    // cellData.H = neighbor.GetDistance(objective);
                    cellData.H = Vector2.Distance(neighbor.CellValue, objective.CellValue);
                    cellData.Connection = _data[current];

                    // readd
                    OpenList.Add(cellData);
                }
            }

            yield return null;
        }

        Debug.Log("No path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;

        while ( Path.Count > 0 )
            Debug.Log("this cell: " + Path.Peek() + "    is start? " + (Path.Peek() == start) + "    is objective? " + (Path.Pop() == objective));
    }
}
