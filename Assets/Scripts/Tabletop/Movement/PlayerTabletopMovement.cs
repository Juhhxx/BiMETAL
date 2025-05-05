using System;
using System.Collections;
using UnityEngine;

public class PlayerTabletopMovement : TabletopMovement
{
    [SerializeField] private CellInfo _cellInformation;
    [SerializeField] private Camera _cam;
    public LayerMask CellLayer => HexagonTabletop.CellLayer;

    private HexagonCell _hover;
    private HexagonCell _hoveredCell {
        get => _hover;
        set {
            _hover = value;
            if ( _cellInformation != null )
                _cellInformation.Hover(_hover);
        }
    }
    public HexagonCell Hovered => _hover;
    
    private HexagonCell _selectedCell;

    public bool InputEnabled { get; set; } = true;

    private void Awake()
    {
        if ( _cellInformation == null )
            _cellInformation = FindFirstObjectByType<CellInfo>();

        if ( _cam == null )
            _cam = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
        _pathfinder = new AStarPathfinder(this, false);
        _pathfinder.Path.CollectionChanged += DemonstratePath;

        if ( _cellInformation == null )
            _cellInformation = FindFirstObjectByType<CellInfo>();
    }

    private void Update()
    {
        CheckForHover();
        CheckForSelection();
    }

    private void CheckForHover()
    {
        if ( !InputEnabled || Moving ) return;

        if ( InputManager.HoverCell(_cam, CellLayer, out HexagonCell newCell ))
        {
            // ShowPath();

            if (_hoveredCell == newCell) return;

            if (_hoveredCell != null)
            {
                // // HidePath();
                /*_pathfinder.Stop();
                _hoveredCell.HoverCell(false);
                _hoveredCell = null;*/
                DoneHovering();
            }

            if ( InputManager.MouseX() != 0 || InputManager.MouseY() != 0  ) return;

            _hoveredCell = newCell;

            // Only show cell has selectable if it's in range and is not already hovered ( it twitches otherwise )
            // Doesnt apply for the players current cell because i think its self explanatory for them

            if ( newCell == CurrentCell ) return;
            
            _hoveredCell.HoverCell();
            _pathfinder.FindPath(CurrentCell, _hoveredCell, Points);
            // Debug.Log("Find path. " + Points);
        }
        else if (_hoveredCell != null)
        {
            // HidePath();
            /*_pathfinder.Stop();
            _hoveredCell.HoverCell(false);
            _hoveredCell = null;*/
            DoneHovering();
        }
    }

    public Action PlayerTurn;
    private void CheckForSelection()
    {
        if ( ! InputEnabled || Moving || Path == null || _hoveredCell == null || _selectedCell != null ) return;
        // Chose up, so if the player hovers and buttons down the wrong button,
        // they can still navigate to another button so select it

        if ( InputManager.Select() )
        {
            // not whats happening anymore // Previously the points were counting with the first and last cell we find in the pathfinded stack, we should change it to not count the first cell, so we add one
            if ( _hoveredCell != CurrentCell )
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
        // Debug.Log("Starting movement from " + CurrentCell + " to " + _selectedCell);
        Moving = true;

        // Stack<HexagonCell> path = _pathfinder.FindPath(CurrentCell, _selectedCell);

        yield return new WaitUntil( () => _pathfinder.Done &&
            ( _pathfinder.ModPath == null ||
            ( _pathfinder.ModPath.Done && _pathfinder.ModPath.Path.Count > 0 ) ));

        if (Path == null || Path.Count <= 0)
        {
            DoneMoving();

            Debug.LogWarning("Can't move there. ");
            yield break;
        }

        // Stack<HexagonCell> final =  new(_pathfinder.Path);

        // HexagonCell last = Path.Peek();
        _pathfinder.Reverse();
        // Debug.Log("Stop moving current? " + CurrentCell + "    path count: " + Path.Count);

        _hoveredCell?.HoverCell(false);
        _hoveredCell = null;

        // remove start
        Path.Pop();
        // get the next cell
        HexagonCell next = Path.Peek();

        while ( Path.Count > 0 ) // uses >= 0 count as next is loa
        {
            /*next = Path.Peek(); // previously giving an error here because pops where happening more than pushes, must also remove as hovered here
            // need to peek first so we dont spook the next pieces modifier interactive
            Debug.Log("peek1: " + next);*/

            // yield return new WaitForSeconds(0.2f);

            if ( next.Piece != null )
            {
                Interact(next.Piece);
                // here we have to wait until the interaction is done...
                // yield return WaitUntil(() )
                // break for now
                PieceInteractive piece = next.Piece as PieceInteractive;

                DoneMoving( piece != null );
                yield break;
            }

            Path.ObservePop();

            CurrentCell.WalkOn();
            CurrentCell = next;
            CurrentCell.WalkOn(Interactive);

            next = Path.Count > 0 ? Path.Peek() : null;
            
            // Debug.Log("last position before: " + transform.position + "  cell: " + CurrentCell);
            yield return Step(CurrentCell, next);
            // Debug.Log("last position after: " + transform.position + "  cell: " + CurrentCell);

            // Debug.Log("current is last? " + (CurrentCell == last) + "      current: " + CurrentCell + "      last: " + last + "      next: " + next);
        }

        DoneMoving();
    }

    private void DoneHovering()
    {
        _pathfinder.Stop();

        if ( _hoveredCell != null )
            _hoveredCell.HoverCell(false);
        
        _hoveredCell = null;
    }
    private void DoneMoving(bool isBattle = false)
    {
        Moving = false;
        _selectedCell = null;
        DoneHovering();
        if ( ! isBattle )
            PlayerTurn?.Invoke();
    }
}
