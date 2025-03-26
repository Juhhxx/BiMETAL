using System;
using System.Collections;
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
        _startCell = CurrentCell;
        _pathfinder.FindPath(CurrentCell, _player.CurrentCell, Points);
    }
    public void Stop()
    {
        _pathfinder.Stop();
    }


    public void MoveEnemy()
    {
        Debug.Log("Moving? enemy: " + gameObject.name);
        IEnumerator cor = Move();

        _queue.Enqueue(cor);

        if (_queue.Count <= 1)
        {
            _startCell = CurrentCell;
            StartCoroutine(cor);
        }
    }

    protected override IEnumerator Move()
    {
        Moving = true;

        yield return new WaitUntil(() => _pathfinder.Done);

        if (Path == null || Path.Count <= 0)
        {
            DoneMoving();
            Debug.Log("Can't move more. ");
            yield break;
        }

        HexagonCell next = Path.Peek();

        yield return new WaitForSeconds(0.2f);

        if (next.Piece != null && next != CurrentCell)
        {
            if (next.Piece is ModifierInteractive)
            {
                Interact(next.Piece);
                Pathfinder.Stop();
            }
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

        DoneMoving();
    }

    private void DoneMoving()
    {
        _queue.Dequeue();

        if (_queue.Count > 0)
            StartCoroutine(_queue.Peek());
        else
            Moving = false;
    }
}
