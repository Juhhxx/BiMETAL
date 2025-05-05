using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public abstract class TabletopMovement : TabletopBase
{
    [field: SerializeField] public int Points { get; set; } = 7;
    
    [SerializeField] private Animation _hopAnimation;

    protected Pathfinder _pathfinder;
    public Pathfinder Pathfinder => _pathfinder;
    public ObservableStack<HexagonCell> Path => _pathfinder.Path;
    private Queue<IEnumerator> _queue = new();


    public bool Moving { get; protected set; }

    protected override void Start()
    {
        base.Start();
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
                    yield return new WaitForSeconds(0.02f);

                    newItem.PathCell();

                    // if (newItem.IsNonAvoidable())
                    ModifierInteractive mod = newItem.Piece as ModifierInteractive;
                    if ( mod != null )
                        mod.Path(_pathfinder.Path);
                }

            if (e.OldItems != null)
                foreach (HexagonCell oldItem in e.OldItems)
                {
                    yield return new WaitForSeconds(0.01f);

                    oldItem.StopPathCell();

                    // if (oldItem.IsNonAvoidable())
                    ModifierInteractive mod = oldItem.Piece as ModifierInteractive;
                    if ( mod != null )
                        mod.Path();
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
        if ( CurrentCell != null )
            CurrentCell.WalkOn();
        if ( Pathfinder != null )
            Pathfinder.Stop();
        Debug.LogWarning("Disabled: " + gameObject.name + "'s " + this);
    }
    protected virtual void OnDestroy()
    {
        Moving = false;
        if (_pathfinder != null)
            _pathfinder.Path.CollectionChanged -= DemonstratePath;
    }

    protected IEnumerator Step(HexagonCell current, HexagonCell next)
    {
        if (_hopAnimation == null || _hopAnimation.clip == null)
        {
            Debug.LogWarning("Missing hop animation");
            yield break;
        }

        _hopAnimation.Stop();
        _hopAnimation.Play();

        // movement start and end
        Vector3 startPos = transform.position;
        Vector3 endPos = current.transform.position;
        endPos.y = startPos.y;

        // rotation start and end
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot;

        if (next != null)
        {
            Vector3 direction = (next.transform.position - current.transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero) // it gives weird long rotations if end rotation is zero, still giving some, but not as long as before
                endRot = Quaternion.LookRotation(direction);
        }

        float duration = _hopAnimation.clip.length;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            float eased = Mathf.SmoothStep(0f, 1f, t); // creates a smooth convex curve for easing movement (half sin wave)

            transform.SetPositionAndRotation(
                Vector3.Lerp(startPos, endPos, eased),
                Quaternion.Slerp(startRot, endRot, eased));
            
            time += Time.deltaTime;
            yield return null;
        }

        // final snap to clean up
        transform.SetPositionAndRotation(endPos, endRot);
    }

    protected void Interact(Interactive other)
    {
        if (other == Interactive) return;

        other.Interact(Interactive);
    }
}
