using UnityEngine;

/// <summary>
/// This exists so we can choose in editor the path we want
/// </summary>
public class PathfinderChooser
{
    public static Pathfinder ChooseRange(MonoBehaviour other, PathfinderType rangeType)
    {
        return rangeType switch
        {
            PathfinderType.AStar => new AStarPathfinder(other, false),
            PathfinderType.OpenStar => new OpenStarPathfinder(other, false),
            PathfinderType.BFSRange => new BFSRangePathfinder(other, false),
            PathfinderType.HexagonRange => new HexRangePathfinder(other, false),
            PathfinderType.StarRange => new StarRangePathfinder(other, false),
            
            // fallback on AStar
            _ => new AStarPathfinder(other, false),
        };
    }
}