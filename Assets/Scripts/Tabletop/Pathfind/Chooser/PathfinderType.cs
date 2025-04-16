using UnityEngine;

/// <summary>
/// This exists so we can choose in editor the path we want
/// </summary>
public enum PathfinderType
{
    AStar,
    OpenStar,
    BFSRange,
    HexagonRange,
    StarRange,
    LineRange
}