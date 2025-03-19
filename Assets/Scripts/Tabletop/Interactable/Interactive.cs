using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    public bool Ignore { get; protected set; } = false;
    
    protected virtual void Start()
    {

    }

    
    public virtual void Hover(bool onOrOff = true)
    {
        if ( onOrOff )
            transform.Translate(Vector3.down * 0.2f);
        else if ( !onOrOff )
            transform.Translate(Vector3.up * 0.2f);
    }

    public virtual void Select()
    {

    }

    public virtual void SelectionError()
    {

    }

    public abstract void Interact();
}
