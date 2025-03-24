using System.Collections;
using UnityEngine;

public class PlayerTabletopMovement : TabletopMovement
{
    [SerializeField] private LayerMask _cellLayer;
    private HexagonCell _hoveredCell;
    private HexagonCell _selectedCell;

    protected override void Start()
    {
        base.Start();
        _pathfinder = new AStarPathfinder(this, false);
        _pathfinder.Path.CollectionChanged += DemonstratePath;
    }

    private void Update()
    {
        CheckForHover();
        CheckForSelection();
    }

    private void CheckForHover()
    {
        if (_moving) return;

        if (InputManager.HoverCell(_cellLayer, out HexagonCell newCell))
        {
            // ShowPath();

            if (_hoveredCell == newCell) return;

            if (_hoveredCell != null)
            {
                // // HidePath();
                /*_pathfinder.Stop();
                _hoveredCell.HoverCell(false);
                _hoveredCell = null;*/
                DoneMoving();
            }

            if (newCell == CurrentCell) return;

            _hoveredCell = newCell;

            // Only show cell has selectable if it's in range and is not already hovered ( it twitches otherwise )
            // Doesnt apply for the players current cell because it think its self explanatory for them
            if (newCell != CurrentCell)
            {
                _hoveredCell.HoverCell();
                _pathfinder.FindPath(CurrentCell, _hoveredCell, Points);
            }
        }
        else if (_hoveredCell != null)
        {
            // HidePath();
            /*_pathfinder.Stop();
            _hoveredCell.HoverCell(false);
            _hoveredCell = null;*/
            DoneMoving();
        }
    }

    private void CheckForSelection()
    {
        if (_moving || Path == null) return;
        // Chose up, so if the player hovers and buttons down the wrong button,
        // they can still navigate to another button so select it

        if (_hoveredCell != null && InputManager.Select())
        {
            // Previously the points were counting with the first and last cell we find in the pathfinded stack, we should change it to not count the first cell, so we add one
            if (Path.Count <= Points + 1)
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
    protected override IEnumerator Move()
    {
        Debug.Log("Starting movement from " + CurrentCell + " to " + _selectedCell);
        _moving = true;

        // Stack<HexagonCell> path = _pathfinder.FindPath(CurrentCell, _selectedCell);

        yield return new WaitUntil(() => _pathfinder.Done);

        if (Path == null)
        {
            DoneMoving();

            Debug.Log("Can't move there. ");
            yield break;
        }

        // Stack<HexagonCell> final =  new(_pathfinder.Path);

        _pathfinder.Reverse();

        _startCell = CurrentCell;
        Debug.Log("Stop moving current? " + CurrentCell + "    path count: " + Path.Count);

        // HidePath();

        HexagonCell next;

        _hoveredCell?.HoverCell(false);
        _hoveredCell = null;

        while (CurrentCell != _selectedCell && Path.Count > 0)
        {
            next = Path.Peek(); // previously giving an error here because pops where happening more than pushes, must also remove as hovered here
            // need to peek first so we dont spook the next pieces modifier interactive

            Debug.Log("next: " + next + "      current: " + CurrentCell + "      selected: " + _selectedCell + "      start: " + _startCell + "      are they the same? " + (next == _selectedCell));
            // Debug.Log("current is selected? " + (CurrentCell == _selectedCell) + "  _current? " + CurrentCell );

            yield return new WaitForSeconds(0.2f);

            if (next.Piece != null && next != CurrentCell)
            {
                Interact(next.Piece);
                // here we have to wait until the interaction is done...
                // yield return WaitUntil(() )
                // break for now
                DoneMoving();
                yield break;
            }

            Path.ObservePop();

            CurrentCell.WalkOn();
            CurrentCell = next;
            // Debug.Log("current is selected?2 " + (CurrentCell == _selectedCell) + "      current: " + CurrentCell + "      selected: " + _selectedCell );
            next.WalkOn(Interactive);

            // Debug.Log("count: " + final.Count);

            // move
            transform.position = new Vector3(next.transform.position.x, transform.position.y, next.transform.position.z);

            // rotate
            if (Path.Count > 0)
            {
                next = Path.Peek();

                Vector3 target = next.transform.position;
                target.y = transform.position.y;

                transform.LookAt(target);
            }

            // Debug.Log("current is selected?3 " + (CurrentCell == _selectedCell) + "      current: " + CurrentCell + "      selected: " + _selectedCell );
        }

        DoneMoving();

        // Debug.Log("stopped moving start?" + _startCell);
    }

    private void DoneMoving()
    {
        _moving = false;
        _pathfinder.Stop();

        if ( _hoveredCell != null )
            _hoveredCell.HoverCell(false);
        
        _hoveredCell = null;
    }
}
