using System.Collections;
using UnityEngine;

public class StarRangePathfinder : Pathfinder
{

    public StarRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        if ( totalWeight < 1 ) yield break;

        HexagonCell current;

        // Debug.Log("Starting pathfinding from " + start + " to " + objective);
        // Debug.Log("Finding path. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);

        // could do it by direction first which i think may be faster

        for ( int dir = 0; dir <= 6; dir++)
        {
            current = objective;
            for (int e = 0; e <= totalWeight; e++ )
            {
                if ( !current.TryGetNeighborInDirection(dir, out HexagonCell next) )
                    break;
                
                OpenList.Add(new CellData(next, 0f, next.GetDistance(objective), null));
                current = next;
            }
        }

        // But I want to do it by weight first which is prettier

        foreach ( CellData cell in OpenList )
            Path.ObservePush(cell.Cell);

        Debug.Log("piece? Path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }
}
