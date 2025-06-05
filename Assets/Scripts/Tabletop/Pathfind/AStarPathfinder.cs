using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// A* pathfinding algorithm on a hex grid.
/// Calculates the shortest path between a start and an objective cell.
/// </summary>
public class AStarPathfinder : Pathfinder
{

    public AStarPathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    /// <summary>
    /// Begins the A* pathfinding search from the given start to the objective cell.
    /// It can start a second pathfinder inside it if an environment modifier is encountered.
    /// </summary>
    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        // objective and start are switched

        Done = false;
        
        _data[start] = new CellData(start, start.Weight, start.GetDistance(objective), null);

        OpenList.Add(_data[start]);

        // Debug.Log("Starting pathfinding from " + start + " to " + objective);
        // Debug.Log("Finding path. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        /*foreach(HexagonCell cell in _data.Keys)
        {
            Debug.Log("                 Data stats for " + cell + "  with " + _data[cell] + " and walkable: " + cell.Walkable);
        }*/

        HexagonCell current = OpenList.Min.Cell;

        while (OpenList.Any())
        {
            current = OpenList.Min.Cell;
            OpenList.Remove(OpenList.Min);
            _closedList.Add(current);

            if (current == objective)
            {
                Path.Push(objective);
                HexagonCell currentCell = _data[objective].Connection.Cell;
                int weight = 0;
                while (currentCell != start)
                {
                    weight += currentCell.Weight;
                    if (totalWeight == -1 || weight <= totalWeight)
                    {
                        Path.ObservePush(currentCell);

                        if (_includeNonAvoidance && currentCell.IsNonAvoidable())
                        {
                            if ( currentCell.Piece is EnvironmentInteractive envi )
                            {
                                ModPath = envi.ModPathfinder;
                                Debug.Log("envi: " + envi.ModPathfinder.Done);
                                yield return new WaitUntil(() => envi.ModPathfinder.Done && envi.ModPathfinder.Path.Count > 0);
                            }
                            
                            Debug.Log("Stopped at envi: " + currentCell.Piece.Name);
                            Done = true;
                            yield break;
                        }
                    }
                    else
                    {
                        Done = true;
                        yield break;
                    }

                    currentCell = _data[currentCell].Connection.Cell;
                    // Debug.Log("this cell G: " + currentCell + "   cell: " + _data[currentCell].Cell + "  connection: " + _data[currentCell].Connection);
                }
                weight += start.Weight;
                if (totalWeight == -1 || weight <= totalWeight)
                    Path.ObservePush(start);
                // Debug.Log("this start cell G: " + start + "   cell: " + _data[start].Cell + "  connection: " + _data[start].Connection);

                Done = true;
                yield break;
            }

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => t != null && 
                ( t.Walkable() || ( _includeNonAvoidance && t.IsNonAvoidable() ) ||
                t == objective) && !_closedList.Contains(t)))
            {
                // by switching the distance here by Points (which is how many points it takes to cross times the distance)
                // we can reorganize pathfinding to account for how much it costs to move
                // it calculates how much to walk to the next one from the current (not to neighbor)

                // neighbor points or current points?
                float costToNeighbor = 0;


                if (neighbor != objective)
                    costToNeighbor = _data[current].G + neighbor.Weight;

                // the tabletop movement is then responsible for curating how far they go (not the pathfinder)

                // if ( totalWeight != -1 && costToNeighbor > totalWeight ) continue;


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

            // this line was consuming a lot of time
            // yield return null;
        }

        // Debug.Log("No path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }
}
