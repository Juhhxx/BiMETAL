using System.Collections;
using UnityEngine;

/// <summary>
/// A pathfinder that traces a straight line of hexagonal cells from the objective
/// toward the start cell direction, up to reach.
/// </summary>
public class LineRangePathfinder : Pathfinder
{

    public LineRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    /// <summary>
    /// Calculates a straight path from the objective cell, in the direction pointing toward the start cell,
    /// and continues for a number of steps equal to totalWeight.
    /// </summary>
    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        if ( totalWeight < 1 ) yield break;

        Done = false;

        HexagonCell current = objective;

        // Debug.Log("Starting pathfinding from " + start + " to " + objective);
        // Debug.Log("Finding path. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);

        // could do it by direction first which i think may be faster

        int dir = objective.GetDirectionToNeighbor(start);
        dir = HexagonCell.ReverseDirection(dir);

        Path.ObservePush(objective);

        for ( int tryNum = 0; tryNum < totalWeight; tryNum++)
        {

            if ( !current.TryGetNeighborInDirection(dir, out HexagonCell next) )
                break;
            // Don't let it modify environment modifier cells
            /* if ( next.Piece != null && next.Piece is EnvironmentInteractive)
                continue;*/
            
            Path.ObservePush(next);
            current = next;
        }

        // Debug.Log("piece? Path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }
}
