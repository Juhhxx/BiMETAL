using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinder : Pathfinder
{

    public AStarPathfinder(HexagonTabletop tabletop, MonoBehaviour owner) : base(tabletop, owner)
    {}

    public override IEnumerator GetPath(HexagonCell objective, HexagonCell start)
    {
        Done = false;
        List<HexagonCell> openList = new() { start };
        List<HexagonCell> closedList = new();

        Debug.Log("Starting pathfinding from " + start + " to " + objective);

        while (openList.Any())
        {
            HexagonCell current = openList.OrderBy(cell => cell.F).ThenBy(cell => cell.H).First();
            openList.Remove(current);
            closedList.Add(current);

            if (current == objective)
            {
                HexagonCell currentCell = objective;
                while (currentCell != start)
                {
                    Path.Push(currentCell);
                    currentCell = currentCell.Connection;
                }

                Path.Push(start);
                Done = true;
                yield break;
            }

            foreach (HexagonCell neighbor in current.Neighbors.Where(t => t.Walkable && !closedList.Contains(t)))
            {
                float costToNeighbor = current.G + current.GetDistance(neighbor);

                if (!openList.Contains(neighbor) || costToNeighbor < neighbor.G)
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(current);
                    neighbor.SetH(neighbor.GetDistance(objective));
                    neighbor.SetF(neighbor.G + neighbor.H);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }

            yield return null;
        }

        Debug.Log("No path found.");
        Done = true;
    }
}
