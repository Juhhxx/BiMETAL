using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public abstract class TabletopMovement : MonoBehaviour
{
    [field: SerializeField] public HexagonCell CurrentCell { get; protected set; }
    [field: SerializeField] public Interactive Interactive { get; protected set; }
    [field: SerializeField] public int Points { get; protected set; } = 7;

    protected Pathfinder _pathfinder;
    public Pathfinder Pathfinder => _pathfinder;
    public ObservableStack<HexagonCell> Path => _pathfinder.Path;
    protected HexagonTabletop _tabletop;
    protected Queue<IEnumerator> _queue = new();
    protected HexagonCell _startCell;


    protected bool _moving;

    protected virtual void Start()
    {
        if (CurrentCell == null)
            CurrentCell = FindFirstObjectByType<HexagonCell>();

        CurrentCell.WalkOn(Interactive);

        transform.position = new Vector3(CurrentCell.transform.position.x, transform.position.y, CurrentCell.transform.position.z);

        _tabletop = FindFirstObjectByType<HexagonTabletop>();
    }

    protected virtual void DemonstratePath(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Debug.Log("count: " + _pathfinder.Path.Count + "      points: " + Points);
        // count viewing, ideally point accumulation would be handled by the pathfinder itself, but it is shown here
        // if ( _pathfinder.Path.Count > Points ) return;

        IEnumerator cor = DemonstrateSlowPath(e);

        _queue.Enqueue(cor);

        if (_queue.Count <= 1)
            StartCoroutine(cor);
    }

    protected virtual IEnumerator DemonstrateSlowPath(NotifyCollectionChangedEventArgs e)
    {
        // Debug.Log("queue count: " + _queue.Count);

        if ((e.NewItems != null && !e.NewItems.Contains(CurrentCell))
            || (e.OldItems != null && !e.OldItems.Contains(CurrentCell)))
        {
            if (e.NewItems != null)
                foreach (HexagonCell newItem in e.NewItems)
                {
                    if (newItem == _startCell)
                        Debug.Log("Add start?: " + _startCell);
                    yield return new WaitForSeconds(0.05f);
                    newItem.PathCell();

                // if (newItem.IsNonAvoidable())
                    if (newItem.Piece is ModifierInteractive piece)
                        piece.Path(_pathfinder.Path);
                }

            if (e.OldItems != null)
                foreach (HexagonCell oldItem in e.OldItems)
                {
                    if (oldItem == _startCell)
                    {
                        Debug.Log("Reset start?: " + _startCell);
                        _startCell = null;
                        // continue;
                    }
                    yield return new WaitForSeconds(0.02f);
                    oldItem.StopPathCell();

                    // if (oldItem.IsNonAvoidable())
                        if (oldItem.Piece is ModifierInteractive piece)
                            piece.Path();
                }
        }

        // Debug.Log("queue count 2: " + _queue.Count);
        _queue.Dequeue();

        if (_queue.Count > 0)
            StartCoroutine(_queue.Peek());
    }
    protected abstract IEnumerator Move();

    protected virtual void OnDisable()
    {
        _moving = false;
        if ( _pathfinder != null )
            _pathfinder.Path.CollectionChanged -= DemonstratePath;
    }

    protected void Interact(Interactive other)
    {
        if ( other == Interactive ) return;
        
        other.Interact(Interactive);
    }
}
