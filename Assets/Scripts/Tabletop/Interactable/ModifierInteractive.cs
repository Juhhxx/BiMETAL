using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ModifierInteractive : Interactive
{
    [SerializeField] protected Modifier _modifier;
    public bool HasModifier => _modifier != null;

    [ShowIf(nameof(HasModifier))]
    [SerializeField] protected PathfinderType _modRangeType;

    [ShowIf(nameof(HasModifier))]
    [SerializeField] protected int _reach;


    protected Pathfinder _modPathfinder;
    protected bool _dynamic = false;

    protected override void Start()
    {
        base.Start();
        Modified = false;

        if ( HasModifier )
        {
            _modPathfinder = PathfinderChooser.ChooseRange(this, _modRangeType);

            if ( _modPathfinder != null )
                _modPathfinder.Path.CollectionChanged += DemonstratePath;
        }
    }

    public override void Hover(bool onOrOff = true)
    {
        base.Hover(onOrOff);

        Debug.Log("modifier name: " + gameObject.name + "  cell: " + Cell);
    }

    public override void Interact(Interactive other = null)
    {
        if ( Modified ) return;

        Modify();

        Modified = true;
    }

    public override void Select()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Modify()
    {
        if ( ! HasModifier ) return;

        // some cosmetic way of saying the _modifier now already modifed and wont be modified again?
        // foreach ( HexagonCell cell in _modPathfinder.Path)
            //cell.Modify

        Debug.Log("modifier? setting path as fr fr hopefully, is path null or count 0?  " + (_modPathfinder.Path.Count == 0 || _modPathfinder.Path == null));

        // we just clear the current path to save the current cells settings and move on
       _modPathfinder.Path.Clear();

    }

    public virtual void Path(ObservableStack<HexagonCell> other = null)
    {
        if ( ! HasModifier || Modified ) return;

        
        Debug.Log("modifier? modifying? " + (other == null));
        // Debug.Log("modifier " + gameObject.name + " trying to path cell: " + Cell + " and other " + other?.Contains(Cell) + " contains it");
        
        if ( other == null || other.Count <= 0 )
        {
            Debug.Log("modifier? stopping");
            _modPathfinder.Stop();
            return;
        }

        ObservableStack<HexagonCell> clone = new(other);

        HexagonCell last = clone.Peek();

        while ( clone.Count > 0 && last != Cell)
        {
            last = clone.Pop();
            // Debug.Log("modifier clone at: " + clone.Count);

            if ( !clone.Any() || clone.Peek() == Cell)
                break;
        }

        if ( clone.Count < 1 )
            return;

        Debug.Log("modifier? starting");
        // only supposed to do this once
        _modPathfinder.FindPath(Cell, last, _reach);
    }


    protected Queue<IEnumerator> _queue = new();

    protected virtual void DemonstratePath(object sender, NotifyCollectionChangedEventArgs e)
    {
        IEnumerator cor = DemonstrateSlowPath(e);

        _queue.Enqueue(cor);

        if (_queue.Count <= 1)
            StartCoroutine(cor);
    }

    protected virtual IEnumerator DemonstrateSlowPath(NotifyCollectionChangedEventArgs e)
    {
        // Debug.Log("queue count: " + _queue.Count);

        if ((e.NewItems != null && !e.NewItems.Contains(Cell))
            || (e.OldItems != null && !e.OldItems.Contains(Cell)))
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

        if (_queue.Count > 0)
            StartCoroutine(_queue.Peek());
    }

    protected virtual void OnDisable()
    {
        if ( _modPathfinder != null )
            _modPathfinder.Path.CollectionChanged -= DemonstratePath;
    }
}
