using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTabletopMovement : TabletopMovement, IComparable<EnemyTabletopMovement>
{
    protected PlayerTabletopMovement _player;
    public int Priority { get; set; }

    protected Pathfinder _rangePathfinder;
    protected Pathfinder _movementPathfinder;

    protected override void Start()
    {
        base.Start();

        _movementPathfinder = new AStarPathfinder(this, true);
        _rangePathfinder = new BFSRangePathfinder(this, true);

        _movementPathfinder.Path.CollectionChanged += DemonstratePath;
        _rangePathfinder.Path.CollectionChanged += DemonstratePath;

        _player = FindFirstObjectByType<PlayerTabletopMovement>();

        TogglePath();
    }

    public void TogglePath()
    {
        _pathfinder?.Stop();
        
        if ( _pathfinder == _rangePathfinder ) _pathfinder = _movementPathfinder;
        else _pathfinder = _rangePathfinder;
    }

    public int CompareTo(EnemyTabletopMovement other)
    {
        if (other == null) return -1;

        if (Path.Count == 0 || other.Path.Count == 0)
            return Path.Count.CompareTo(other.Path.Count);

        HexagonCell thisTop = Path.Peek();
        HexagonCell otherTop = other.Path.Peek();

        // same peek then doesn't matter
        if (thisTop == otherTop)
            return 0;

        // other goes first
        if (other.Path.Contains(thisTop))
            return 1;

        // this goes first
        if (Path.Contains(otherTop))
            return -1;

        return Path.Count.CompareTo(other.Path.Count);
    }

    public void FindPath()
    {
        _pathfinder.FindPath(CurrentCell, _player.CurrentCell, Points);
    }
    public void Stop()
    {
        _pathfinder.Stop();
    }

    // Casually spent two weeks trying to remember that I had to create my own queue
    // and not use the base class's and thats why my system was completely broken
    // AND fixed my start cell bug in the process
    // I am stupid on a genius level
    protected Queue<IEnumerator> _queue = new();
    public int QueueCount => _queue.Count;

    public void MoveEnemy()
    {
        IEnumerator cor = Move();
        _queue.Enqueue(cor);

        if (_queue.Count <= 1)
        {
            Moving = true;
            StartCoroutine(cor);
        }
    }

    protected override IEnumerator Move()
    {
        yield return new WaitUntil(() => _pathfinder.Done);

        Debug.Log("0 start? Enemy: " + gameObject.name + 
            " | Path null? " + (Path == null) +
            " | Path count: " + (Path?.Count ?? -1) +
            " | CurrentCell: " + (CurrentCell != null ? CurrentCell : "null") +
            " | Moving: " + Moving);

        if ( Path == null || Path.Count <= 0 )
        {
            Debug.LogWarning("Can't move more. ");
            yield return EndMovement();
            yield break;
        }

        HexagonCell next = Path.Peek();
        yield return new WaitForSeconds(0.2f);

        if (next.Piece != null && next != CurrentCell)
        {
            if (next.Piece is ModifierInteractive)
                Interact(next.Piece);

            Debug.Log("Blocked by piece. Ending movement early for " + gameObject.name);
            yield return EndMovement();
            yield break;
        }

        Debug.Log("1.5 start? path is: " + Path + " count: " + Path.Count);
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

        yield return EndMovement();
    }

    private IEnumerator EndMovement()
    {
        if (_queue.Count > 0)
            _queue.Dequeue();

        if (_queue.Count > 0)
            yield return StartCoroutine(_queue.Peek());
        else
            DoneMoving();
    }

    private void DoneMoving()
    {
        // _queue.Clear();
        _pathfinder.Stop();
        Moving = false;
    }
}
