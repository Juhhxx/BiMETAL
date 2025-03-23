using UnityEngine;

/// <summary>
// Interactives shouldnt be raycasted, the cells under them are raycast targets
// and then they send the signals to their Interactive if it exits.
/// </summary>
public abstract class Interactive : MonoBehaviour
{
    public bool Modified { get; protected set; } = true;
    protected TabletopBase _base;
    public HexagonCell Cell => _base?.CurrentCell;
    [SerializeField] private LayerMask _cells;

    protected virtual void Start()
    {
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
}
