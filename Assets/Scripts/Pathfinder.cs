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
    public Pathfinder(HexagonTabletop tabletop, MonoBehaviour owner)
    {
        _owner = owner;
        _tabletop = tabletop;
        Path = new();
        Done = true;
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

        Path.Clear();
    }
}
