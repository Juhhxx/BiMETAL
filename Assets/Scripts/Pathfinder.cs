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

    protected Dictionary<HexagonCell, CellData> _data;
    // becomes open for environment variables to see
    public SortedSet<CellData> OpenList  { get; protected set; }
    protected HashSet<HexagonCell> _closedList;

    public Pathfinder(HexagonTabletop tabletop, MonoBehaviour owner)
    {
        _owner = owner;
        _tabletop = tabletop;
        Path = new();
        Done = true;

        _data = new Dictionary<HexagonCell, CellData>();
        OpenList = new SortedSet<CellData>();
        _closedList = new HashSet<HexagonCell>();
    }
    public Stack<HexagonCell> FindPath(HexagonCell start, HexagonCell objective)
    {
        Stop();

        _getPath = _owner.StartCoroutine(GetPath(start, objective));
        return Path;
    }

    public abstract IEnumerator GetPath(HexagonCell objective, HexagonCell start);

    public void Stop()
    {
        if (_getPath != null)
        {
            _owner.StopCoroutine(_getPath);
        }

        _data.Clear();
        OpenList.Clear();
        _closedList.Clear();

        Path.Clear();

        Done = true;
    }
}
