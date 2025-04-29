using System.Collections;
using UnityEngine;

public class HexRangePathfinder : Pathfinder
{

    public HexRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        if ( totalWeight < 1 ) yield break;

        Done = false;

        HexagonCell current;
        int adjacentDir;

        // Debug.Log("Starting pathfinding from " + start + " to " + objective);
        // Debug.Log("Finding path. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);

        // could do it by direction first which i think may be faster

        for ( int dir = 0; dir <= 6; dir++)
        {
            current = objective;
            for (int e = 1; e <= totalWeight; e++ )
            {
                if ( !current.TryGetNeighborInDirection(dir, out HexagonCell next) )
                    break;
                
                // if it reaches the last cell, add it to closed and start searching its hexagon shape adjacent cells
                // H is now the euclidean distance (for a nice effect, its purely visual, closer cells will appear first ect)

                // it uses the closed list as closed list is a hash set,
                // so it wont repeat any cells despite them searching in each others directions

                if ( e == totalWeight )
                {
                    _closedList.Add(next);

                    adjacentDir = HexagonCell.ReverseDirection(dir);

                    AddAdjacent(next, adjacentDir +1, totalWeight);
                    AddAdjacent(next, adjacentDir -1, totalWeight);
                }

                current = next;
            }
        }

        // So it sorts it into the path by euclidean distance

        foreach ( HexagonCell cell in _closedList )
            OpenList.Add(new CellData(cell, 0f,
                Vector3.Distance(cell.transform.position, objective.transform.position), null));

        foreach ( CellData cell in OpenList )
            Path.ObservePush(cell.Cell);

        // Debug.Log("piece? Path found. Stats:   open " + OpenList.Count + "   closed " + _closedList.Count + "   data " + _data.Count);
        Done = true;
    }

    private void AddAdjacent(HexagonCell start, int dir, int totalWeight)
    {
        HexagonCell current = start;

        for (int o = 1; o <= totalWeight; o++ )
        {
            if ( !current.TryGetNeighborInDirection(dir, out HexagonCell next) )
                break;
            
            _closedList.Add(next);
            
            current = next;
        }
    }
}