using System;
using System.Collections.Generic;
using System.Collections.Specialized;

public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged
{
    
    public ObservableStack() : base() { }

    public ObservableStack(IEnumerable<T> collection) : base(collection) { }

    public ObservableStack(int capacity) : base(capacity) { }


    public T ObservePop()
    {
        if ( Count == 0) throw new InvalidOperationException("Stack is empty.");

        // Debug.Log("count1 " + Count);
        T item = Peek();
        // Debug.Log("count2 " + Count);
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        // Debug.Log("count3 " + Count);
        Pop();
        
        // Debug.Log("removing " + item);

        return item;
    }

    public void ObservePush(T item)
    {
        Push(item);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
    }

    public void ObserveClear()
    {
        // We need to use actual new methods like this because we cant make sure the overrides will go before or after base.
        
        while ( Count > 0 )
        {
            // OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            ObservePop();
        }

        Clear();
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

    public void Reverse()
    {
        if (Count <= 1) return;

        T[] items = this.ToArray();
        Clear();

        for (int i = 0; i < items.Length; i++)
            Push(items[i]);
    }
}
