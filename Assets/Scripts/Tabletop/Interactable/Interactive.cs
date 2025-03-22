using UnityEngine;

/// <summary>
// Interactives shouldnt be raycasted, the cells under them are raycast targets
// and then they send the signals to their Interactive if it exits.
/// </summary>
public abstract class Interactive : MonoBehaviour
{
    private HexagonCell _cell;
    public bool Modified { get; protected set; } = true;
    public HexagonCell Cell
    {
        get
        {
            UpdateCurrentCell();
            return _cell;
        }
    }
    [SerializeField] private LayerMask _cells;

    protected virtual void Start()
    {

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

    protected void UpdateCurrentCell()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, _cells))
            _cell = hit.transform.GetComponentInChildren<HexagonCell>();
    }
}
