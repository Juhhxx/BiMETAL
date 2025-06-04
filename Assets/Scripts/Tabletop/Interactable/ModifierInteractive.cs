using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NaughtyAttributes;
using UnityEngine;

public abstract class ModifierInteractive : Interactive
{
    [SerializeField] protected Modifier _modifier;
    public Modifier Modifier
    {
        get
        {
            if (!_cloned && _modifier != null)
            {
                _modifier = _modifier.Clone();
                _cloned = true;
                Debug.Log("Get mod from " + gameObject.name + " mod: " + _modifier.name);
            }
            return _modifier;
        }
    }
    private bool _cloned = false;
    public bool HasModifier => Modifier != null;
    public bool Modified { get; protected set; } = false;

    [ShowIf(nameof(HasModifier))]
    [SerializeField] protected PathfinderType _modRangeType;

    [field:SerializeField] public int Reach { get; protected set; }


    public Pathfinder ModPathfinder { get; protected set; }

    protected override void Start()
    {
        base.Start();

        if ( HasModifier )
        {
            ModPathfinder = PathfinderChooser.ChooseRange(this, _modRangeType);

            if ( ModPathfinder != null )
                ModPathfinder.Path.CollectionChanged += DemonstratePath;
        }
    }

    public override void Hover(bool onOrOff = true)
    {
        base.Hover(onOrOff);

        // Debug.Log("modifier name: " + gameObject.name + "  cell: " + Cell);
    }

    public override void Select()
    {
        base.Select();
    }

    public abstract void Modify();

    public abstract void Path(ObservableStack<HexagonCell> other = null);


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
                    // Debug.Log("Pathing Modifying cell and count is: " + ModPathfinder.Path.Count);
                    yield return new WaitForSeconds(0.01f);
                    newItem.Modify(Modifier);
                }

            if (e.OldItems != null)
                foreach (HexagonCell oldItem in e.OldItems)
                {
                    // Debug.Log("Un-Pathing Modifying cell and count is: " + ModPathfinder.Path.Count);
                    yield return new WaitForSeconds(0.005f);
                    oldItem.Modify(Modifier);
                }
        }

        // Debug.Log("queue count 2: " + _queue.Count);
        _queue.Dequeue();

        if (_queue.Count > 0)
            StartCoroutine(_queue.Peek());
    }

    protected virtual void OnDisable()
    {
        if ( ModPathfinder != null )
            ModPathfinder.Path.CollectionChanged -= DemonstratePath;
    }
}
