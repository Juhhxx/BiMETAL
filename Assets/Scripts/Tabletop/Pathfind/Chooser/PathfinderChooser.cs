using UnityEngine;

/// <summary>
/// This exists so we can choose in editor the path we want
/// </summary>
public class PathfinderChooser
{
    public static Pathfinder ChooseRange(MonoBehaviour other, PathfinderType rangeType, bool nonAvoid = false)
    {
        return rangeType switch
        {
            PathfinderType.AStar => new AStarPathfinder(other, nonAvoid),
            PathfinderType.OpenStar => new OpenStarPathfinder(other, nonAvoid),
            PathfinderType.BFSRange => new BFSRangePathfinder(other, nonAvoid),
            PathfinderType.HexagonRange => new HexRangePathfinder(other, nonAvoid),
            PathfinderType.StarRange => new StarRangePathfinder(other, nonAvoid),

            // fallback on AStar
            _ => new AStarPathfinder(other, nonAvoid),
        };
    }
}