using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TabletopMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _cellLayer;
    private HexagonCell _hoveredCell;
    [SerializeField] private HexagonCell _currentCell;
    private HexagonCell _selectedCell;
    private Pathfinder _pathfinder;
    private HexagonTabletop _tabletop;
    public int Points { get; private set; } = 7;


    private bool _moving;
    // private HashSet<HexagonCell> _shownPath;

    private void Start()
    {
        if ( _currentCell == null )
            _currentCell = FindFirstObjectByType<HexagonCell>();
        transform.position = new Vector3(_currentCell.transform.position.x, transform.position.y, _currentCell.transform.position.z);

        _tabletop = FindFirstObjectByType<HexagonTabletop>();
        _pathfinder = new AStarPathfinder(_tabletop, this);
        _pathfinder.Path.CollectionChanged += DemonstratePath;

        // _shownPath = new HashSet<HexagonCell>();
    }
    private void Update()
    {
        CheckForHover();
        CheckForSelection();
    }

    private void CheckForHover()
    {
        if ( _moving ) return;

        if ( InputManager.HoverCell( _cellLayer, out var newCell) )
        {
            // ShowPath();

            if ( _hoveredCell == newCell ) return;

            if ( _hoveredCell != null &&  newCell != _currentCell )
            {
                // // HidePath();
                _pathfinder.Stop();
                _hoveredCell?.StopHoverCell();
            }

            _hoveredCell = newCell;
    
            // Only show cell has selectable if it's in range and is not already hovered ( it twitches otherwise )
            // Doesnt apply for the players current cell because it think its self explanatory for them
            if ( newCell != _currentCell )
            {
                _hoveredCell.HoverCell();
                _pathfinder.FindPath(_currentCell, _hoveredCell);
            }
        }
        else if ( _hoveredCell != null )
        {
            // HidePath();
            _pathfinder.Stop();
            _hoveredCell.StopHoverCell();
            _hoveredCell = null;
        }
    }

    public void DemonstratePath(object sender, NotifyCollectionChangedEventArgs e)
    {
        StartCoroutine(DemonstrateSlowPath(e));
    }

    private IEnumerator DemonstrateSlowPath(NotifyCollectionChangedEventArgs e)
    {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
        if ( e.NewItems == _currentCell || e.OldItems == _currentCell) yield break;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast

        if (e.NewItems != null && _pathfinder.Path.Count < Points)
            foreach (HexagonCell newItem in e.NewItems)
            {
                yield return new WaitForSeconds(0.1f);
               newItem.PathCell();
            }

        if (e.OldItems != null)
            foreach (HexagonCell oldItem in e.OldItems)
            {
                yield return new WaitForSeconds(0.1f);
               oldItem.StopPathCell(); 
            }
    }

    /*private void ShowPath()
    {
        StartCoroutine(SlowPath());
        HideExtraPath();
    }
    private IEnumerator SlowPath()
    {
        if ( _pathfinder.Path != null )
        {
            HexagonCell[] temp = _pathfinder.Path.Skip(1).Take(Points).ToArray();

            Debug.Log("Showing path count : " + _pathfinder.Path.Count + "      to : " + _hoveredCell);

            foreach (HexagonCell cell in temp)
            {
                if ( !_shownPath.Contains(cell) )
                {
                    cell.HoverCell();
                    _shownPath.Add(cell);

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        // Remove cells that are out of pathfinder

        if ( _shownPath == null ) yield break;

        Debug.Log("Hiding extra path count : " + _pathfinder.Path.Count + "   shownPath count: " + _shownPath.Count);

        List<HexagonCell> removePath = new();

        foreach(HexagonCell cell in _shownPath)
       {
            if ( !_pathfinder.Path.Contains(cell) )
                removePath.Add(cell);
       }

        foreach(HexagonCell cell in removePath)
       {
            cell.StopHoverCell();
            _shownPath.Remove(cell);
            yield return new WaitForSeconds(0.1f);
       }
    }
    private void HideExtraPath()
    {
    }
    private void HidePath()
    {
        if ( _shownPath == null ) return;

        Debug.Log("Hiding path count : " + _pathfinder.Path.Count);
        foreach(HexagonCell cell in _shownPath)
       {
            cell.StopHoverCell();
       }

        _shownPath.Clear();
        _pathfinder.Path.Clear();
    }*/

    private void CheckForSelection()
    {
        if (_moving || _pathfinder.Path == null ) return;
        // Chose up, so if the player hovers and buttons down the wrong button,
        // they can still navigate to another button so select it

        if (_hoveredCell != null && InputManager.Select())
        {
            if ( _pathfinder.Path.Count <= Points )
            {
                _selectedCell = _hoveredCell;
                StartCoroutine(Move());
            }
            else
            {
                _hoveredCell.SelectionError();
            }
        }

    }
    private IEnumerator Move()
    {
        Debug.Log("Starting movement from " + _currentCell + " to " + _selectedCell );
        _moving = true;

        // Stack<HexagonCell> path = _pathfinder.FindPath(_currentCell, _selectedCell);

        yield return new WaitUntil(() => _pathfinder.Done);

        if ( _pathfinder.Path == null )
        {
            _moving = false;
            Debug.Log("Can't move there. ");
            yield break;
        }

        Stack<HexagonCell> final =  new(_pathfinder.Path);

        // HidePath();

        HexagonCell next;
        do
        {
            yield return new WaitForSeconds(0.2f);

            next = final.Pop();

            if ( next == _selectedCell )
                Debug.Log("at point");
            else
                Debug.Log("not at point");
            Debug.Log("count: " + final.Count);
            
            _currentCell = next;

            transform.position = new Vector3(next.transform.position.x, transform.position.y, next.transform.position.z);

            if ( final.Count > 0 )
            {
                next = final.Peek();

                Vector3 target = next.transform.position;
                target.y = transform.position.y;

                transform.LookAt(target);
            }
        }
        while ( _currentCell != _selectedCell );

        _selectedCell = null;

        _moving = false;

        _pathfinder.Stop();

        _currentCell.WalkOn(this);
    }

    private void OnDisable()
    {
        _moving = false;
    }
}
