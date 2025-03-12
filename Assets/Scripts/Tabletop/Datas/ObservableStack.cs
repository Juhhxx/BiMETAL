using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged
{
    
    public ObservableStack() : base() { }

    public ObservableStack(IEnumerable<T> collection) : base(collection) { }

    public ObservableStack(int capacity) : base(capacity) { }


    public virtual new T Pop()
    {
        T item = base.Pop();
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        
        // Debug.Log("removing " + item);

        return item;
    }

    public virtual new void Push(T item)
    {
        base.Push(item);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
    }

    public virtual new void Clear()
    {
        foreach(T item in this)
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);

        base.Clear();

        // Debug.Log("clearing");
    }

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, T item)
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
            action
            , item
            , item == null ? -1 : 0)
        );
    }
}
