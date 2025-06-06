using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// A variant of A* pathfinding that floods outward from a start point,
/// but returns all visited cells in order as the path.
/// </summary>
public class OpenStarPathfinder : Pathfinder
{

    public OpenStarPathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    /// <summary>
    /// Begins the open star pathfinding but spreads outward while avoiding movement directly
    /// along the input direction, and stops when the objective cell is reached or the open list is done.
    /// </summary>
    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        // objective and start are switched

        Debug.Log("envi: modifier? starting pathfinder at " + _owner.gameObject.name);
        Done = false;


        int dir = start.GetDirectionToNeighbor(objective);
        HexagonCell current = objective;

        // Above, current is now "objective" (starting cell), bellow, objective becomes the most ahead cell
        // gets the last cell in the new direction, within reach.

        for ( int i = 0 ; i <= totalWeight; i++)
        {
            if ( !objective.TryGetNeighborInDirection(dir, out HexagonCell next) )
                break;
        
            objective = next;
        }

        // DEBUG objective.PathCell();


        _data[current] = new CellData(current, current.Weight, current.GetDistance(objective), null);
        OpenList.Add(_data[current]);

        while (OpenList.Any())
        {
            current = OpenList.Min.Cell;
            OpenList.Remove(OpenList.Min);
            _closedList.Add(current);

            Path.ObservePush(current);

            if ( current == objective )
            {
                // Debug.Log("modifier? Found start");
                Done = true;
                yield break;
            }

            // avoid going in the direct direction of the objective (dir)
            foreach (HexagonCell neighbor in current.Neighbors.Where(t => t != null && 
                !_closedList.Contains(t) && dir != current.GetDirectionToNeighbor(t)))
            {
                float costToNeighbor = 0;

                // previously weighted here, no more
                if (neighbor != objective)
                    costToNeighbor = _data[current].G + current.Weight;

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
                    // cellData.H = HeuristicWeightDistance(neighbor, objective);

                    // readd
                    OpenList.Add(cellData);
                }
            }
            
            // consuming a lot of time again
            // yield return null;
        }

        // DEBUG objective.StopPathCell();

        Debug.Log("envi: modifier? Path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }

    /* Nevermind

    private float HeuristicWeightDistance(HexagonCell one, HexagonCell two)
    {
        // the seed needs to be the same for each cell
        System.Random rng = new(two.CellValue.GetHashCode());
        // the float determines the random force
        return one.GetDistance(two)/ (float) rng.NextDouble() * 0.5f;
    }*/
}
