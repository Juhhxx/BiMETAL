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
        
        _currentCell.WalkOn(this);

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
                _hoveredCell?.HoverCell(false);
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
            _hoveredCell.HoverCell(false);
            _hoveredCell = null;
        }
    }

    private Queue<IEnumerator> _queue = new();
    public void DemonstratePath(object sender, NotifyCollectionChangedEventArgs e)
    {
        IEnumerator cor = DemonstrateSlowPath(e);
        
        _queue.Enqueue(cor);

        if ( _queue.Count <= 1 )
            StartCoroutine(cor);
    }

    private IEnumerator DemonstrateSlowPath(NotifyCollectionChangedEventArgs e)
    {
        // Debug.Log("queue count: " + _queue.Count);

        if ( ( e.NewItems != null && !e.NewItems.Contains(_currentCell) )
            || ( e.OldItems != null && !e.OldItems.Contains(_currentCell) ) )
       {
            // temporary count viewing, ideally point accumulation would be handled by the pathfinder itself.
            if (e.NewItems != null)
                foreach (HexagonCell newItem in e.NewItems)
                {
                    yield return new WaitForSeconds(0.05f);
                    newItem.PathCell();
                }

            if (e.OldItems != null)
                foreach (HexagonCell oldItem in e.OldItems)
                {
                    yield return new WaitForSeconds(0.02f);
                    if ( oldItem == _startCell )
                    {
                        _startCell = null;
                        break;
                    }
                    oldItem.StopPathCell();
                    if ( oldItem == _selectedCell )
                        _selectedCell = null;
                }
       }
        
        // Debug.Log("queue count 2: " + _queue.Count);
        _queue.Dequeue();

       if( _queue.Count > 0)
            StartCoroutine(_queue.Peek());
    }

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

    private HexagonCell _startCell;
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

        _startCell = _currentCell;
        Debug.Log("Stop moving current? " + _currentCell + "    path count: " + _pathfinder.Path.Count);

        // HidePath();

        HexagonCell next;
        do
        {
            yield return new WaitForSeconds(0.2f);

            next = _pathfinder.Path.Pop(); // giving an error here, find out why
            Debug.Log("next: " + next + "      current" + _currentCell + "   are they the same? " + (next == _currentCell));

            _currentCell.WalkOn();
            _currentCell = next;
            next.WalkOn(this);

            // Debug.Log("count: " + final.Count);

            if ( _currentCell.Piece != null )
                StartBattle(_currentCell.Piece);

            transform.position = new Vector3(next.transform.position.x, transform.position.y, next.transform.position.z);


            if ( _pathfinder.Path.Count > 0 )
            {
                next = _pathfinder.Path.Peek();

                Vector3 target = next.transform.position;
                target.y = transform.position.y;

                transform.LookAt(target);
            }
        }
        while ( _currentCell != _selectedCell );

        _pathfinder.Stop();
        _moving = false;

        Debug.Log("stopped moving start?" + _startCell);
    }

    private void OnDisable()
    {
        _moving = false;
    }

    private void StartBattle(TabletopMovement enemy)
    {

    }
}
