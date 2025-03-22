using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ModifierInteractive : Interactive
{
    [SerializeField] protected Modifier _modifier;
    [SerializeField] protected Pathfinder _pathfinder;
    [SerializeField] protected int _reach;
    protected bool _dynamic = false;

    protected override void Start()
    {
        base.Start();
        _pathfinder.Path.CollectionChanged += DemonstratePath;
    }

    public override void Interact(Interactive other = null)
    {
        Modify();

        Modified = true;
    }

    public override void Hover(bool onOrOff = true)
    {
        base.Hover();
    }

    public override void Select()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Modify()
    {
        // some cosmetic way of saying the _modifier now already modifed and wont be modified again

        _pathfinder.Path.Clear();
    }

    public virtual void Path( ObservableStack<HexagonCell> other = null )
    {   
        if ( other == null )
        {
            _pathfinder.Stop();
            return;
        }
        
        HexagonCell last = other.Pop();

        while ( last != Cell )
            last = other.Pop();
        
        last = other.Pop();
        
        _pathfinder.FindPath(Cell, last, _reach);
    }


    protected Queue<IEnumerator> _queue = new();

    protected virtual void DemonstratePath(object sender, NotifyCollectionChangedEventArgs e)
    {
        IEnumerator cor = DemonstrateSlowPath(e);
        
        _queue.Enqueue(cor);

        if ( _queue.Count <= 1 )
            StartCoroutine(cor);
    }

    protected virtual IEnumerator DemonstrateSlowPath(NotifyCollectionChangedEventArgs e)
    {
        // Debug.Log("queue count: " + _queue.Count);

        if ( ( e.NewItems != null && !e.NewItems.Contains(Cell) )
            || ( e.OldItems != null && !e.OldItems.Contains(Cell) ) )
        {
            if (e.NewItems != null)
                foreach (HexagonCell newItem in e.NewItems)
                {
                    yield return new WaitForSeconds(0.01f);
                    newItem.Modify(_modifier, _dynamic);
                }

            if (e.OldItems != null)
                foreach (HexagonCell oldItem in e.OldItems)
                {
                    yield return new WaitForSeconds(0.01f);
                    oldItem.Modify(_modifier, _dynamic);
                }
        }

        // Debug.Log("queue count 2: " + _queue.Count);
        _queue.Dequeue();

       if( _queue.Count > 0)
            StartCoroutine(_queue.Peek());
    }

    protected virtual void OnDisable()
    {
        _pathfinder.Path.CollectionChanged -= DemonstratePath;
    }
}
