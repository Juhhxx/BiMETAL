using System;
using System.Collections;
using UnityEngine;

public class EnemyTabletopMovement : TabletopMovement, IComparable<EnemyTabletopMovement>
{
    protected PlayerTabletopMovement _player;
    public int Priority { get; set; }

    protected override void Start()
    {
        base.Start();
        _pathfinder = new AStarPathfinder(this, true);
        _pathfinder.Path.CollectionChanged += DemonstratePath;
        _player = FindFirstObjectByType<PlayerTabletopMovement>();
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


    public void MoveEnemy()
    {
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
        _moving = true;

        yield return new WaitUntil(() => _pathfinder.Done);

        if (Path == null || Path.Count < 0)
        {
            Debug.Log("Can't move more. ");
        }
        else
        {

            HexagonCell next = Path.ObservePop();

            yield return new WaitForSeconds(0.2f);

            if (Path.Count > 0)
            {
                next = Path.Peek();

                Vector3 target = next.transform.position;
                target.y = transform.position.y;

                transform.LookAt(target);
            }

            if (next.Piece is not PieceInteractive)
            {
                Interact(next.Piece);
                Pathfinder.Stop();
            }
            else
            {
                CurrentCell.WalkOn();
                CurrentCell = next;

                next.WalkOn(Interactive);

                transform.position = new Vector3(next.transform.position.x, transform.position.y, next.transform.position.z);

                if (Interactive is PieceInteractive piece)
                    piece.Modify();
            }

        }

        _queue.Dequeue();

        if (_queue.Count > 0)
            StartCoroutine(_queue.Peek());
        else
            _moving = false;
    }
}
