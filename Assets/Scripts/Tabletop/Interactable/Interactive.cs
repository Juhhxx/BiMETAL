using UnityEngine;

/// <summary>
// Interactives shouldnt be raycasted, the cells under them are raycast targets
// and then they send the signals to their Interactive if it exits.
/// </summary>
public abstract class Interactive : MonoBehaviour
{
    [SerializeField] protected TabletopBase _base;
    public TabletopBase Base => _base;
    [field:SerializeField] public string Name { get; private set; }
    public HexagonCell Cell => _base != null ? _base.CurrentCell : null;

    protected virtual void Start()
    {
        if ( _base == null )
            _base = GetComponentInChildren<TabletopBase>();
    }


    public virtual void Hover(bool onOrOff = true)
    {
        if (onOrOff)
            transform.Translate(Vector3.up * 0.2f);
        else if (!onOrOff)
            transform.Translate(Vector3.down * 0.2f);
    }

    public virtual void Select()
    {

    }

    public virtual void SelectionError()
    {

    }

    public abstract void Interact(Interactive other = null);

    public virtual void AwakeRegion() {}
    
    public virtual void SleepRegion() {}
}
