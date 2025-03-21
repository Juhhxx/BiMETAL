using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTabletopMovement : TabletopMovement
{
    [SerializeField] private LayerMask _cellLayer;
    private HexagonCell _hoveredCell;
    private HexagonCell _selectedCell;

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

            if ( _hoveredCell != null )
            {
                // // HidePath();
                _pathfinder.Stop();
                _hoveredCell.HoverCell(false);
                _hoveredCell = null;
            }
            
            if ( newCell == _currentCell ) return;

            _hoveredCell = newCell;
    
            // Only show cell has selectable if it's in range and is not already hovered ( it twitches otherwise )
            // Doesnt apply for the players current cell because it think its self explanatory for them
            if ( newCell != _currentCell )
            {
                _hoveredCell.HoverCell();
                _pathfinder.FindPath(_currentCell, _hoveredCell, Points);
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

    private void CheckForSelection()
    {
        if (_moving || _pathfinder.Path == null ) return;
        // Chose up, so if the player hovers and buttons down the wrong button,
        // they can still navigate to another button so select it

        if (_hoveredCell != null && InputManager.Select())
        {
            // Previously the points were counting with the first and last cell we find in the pathfinded stack, we should change it to not count the first cell, so we add one
            if ( _pathfinder.Path.Count <= Points +1 )
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

        // Stack<HexagonCell> final =  new(_pathfinder.Path);

        _pathfinder.Path.Reverse();

        _startCell = _currentCell;
        Debug.Log("Stop moving current? " + _currentCell + "    path count: " + _pathfinder.Path.Count);

        // HidePath();

        HexagonCell next;

        while ( _currentCell != _selectedCell &&  _pathfinder.Path.Count > 0 )
        {
            next = _pathfinder.Path.ObservePop(); // previously giving an error here because pops where happening more than pushes
            Debug.Log("next: " + next + "      current: " + _currentCell + "      selected: " + _selectedCell + "      start: " + _startCell + "      are they the same? " + (next == _selectedCell));
            // Debug.Log("current is selected? " + (_currentCell == _selectedCell) + "  _current? " + _currentCell );

            yield return new WaitForSeconds(0.2f);

            if ( _currentCell.Piece != null )
                Interact(_currentCell.Piece);

            _currentCell.WalkOn();
            _currentCell = next;
            // Debug.Log("current is selected?2 " + (_currentCell == _selectedCell) + "      current: " + _currentCell + "      selected: " + _selectedCell );
            next.WalkOn(Interactive);

            // Debug.Log("count: " + final.Count);

            transform.position = new Vector3(next.transform.position.x, transform.position.y, next.transform.position.z);

            if ( _pathfinder.Path.Count > 0 )
            {
                next = _pathfinder.Path.Peek();

                Vector3 target = next.transform.position;
                target.y = transform.position.y;

                transform.LookAt(target);
            }

            // Debug.Log("current is selected?3 " + (_currentCell == _selectedCell) + "      current: " + _currentCell + "      selected: " + _selectedCell );
        }

        _pathfinder.Stop();
        _moving = false;

        // Debug.Log("stopped moving start?" + _startCell);
    }
}
