using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Incase I need to implement new pathfinding methods we make a an abstract pathfinder
/// </summary>
public abstract class Pathfinder
{
    protected readonly HexagonTabletop _tabletop;
    public bool Done { get; protected set; }
    public ObservableStack<HexagonCell> Path { get; protected set; }
    protected MonoBehaviour _owner;
    protected Coroutine _getPath;
    protected bool _includeNonAvoidance;

    protected Dictionary<HexagonCell, CellData> _data;
    // becomes open for environment variables to see
    public SortedSet<CellData> OpenList { get; protected set; }
    protected HashSet<HexagonCell> _closedList;

    // lpa based static path dictionary? uwu
    // protected static Dictionary<HexagonCell, HexagonCell> _discoveredPaths;

    public Pathfinder(MonoBehaviour owner, bool nonAvoid)
    {
        _owner = owner;
        Path = new();
        Done = true;
        _includeNonAvoidance = nonAvoid;

        _data = new Dictionary<HexagonCell, CellData>();
        OpenList = new SortedSet<CellData>();
        _closedList = new HashSet<HexagonCell>();
    }
    public Stack<HexagonCell> FindPath(HexagonCell start, HexagonCell objective, int points)
    {
        Stop();
        // Debug.Log("piece? owner: " + _owner + "  start: " + start + " objective: " + objective + "  points: " + points);
        _getPath = _owner.StartCoroutine(GetPath(start, objective, points));
        return Path;
    }

    protected abstract IEnumerator GetPath(HexagonCell objective, HexagonCell start, int points);

    public void Stop()
    {
        // Debug.Log("piece? stopping " + _owner + " path.");
        
        if (_getPath != null)
        {
            _owner.StopCoroutine(_getPath);
        }

        _data.Clear();
        OpenList.Clear();
        _closedList.Clear();

        Path.ObserveClear();

        Done = true;
    }

    public void Reverse()
    {
        Path.Reverse();
    }
}
