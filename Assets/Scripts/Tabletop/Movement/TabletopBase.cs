using UnityEngine;

public class TabletopBase : MonoBehaviour
{
    [field: SerializeField] public HexagonCell CurrentCell { get; protected set; }
    [field: SerializeField] public Vector2 LastCell { get; protected set; }
    [field: SerializeField] public Interactive Interactive { get; protected set; }

    protected virtual void Start()
    {
        if (CurrentCell == null)
            CurrentCell = FindFirstObjectByType<HexagonCell>();

        CurrentCell.WalkOn(Interactive);

        transform.position = new Vector3(CurrentCell.transform.position.x, transform.position.y, CurrentCell.transform.position.z);
    }

    public void SetCurrent(HexagonCell newCurr)
    {
        if ( CurrentCell != null )
            CurrentCell.WalkOn();
        
        CurrentCell = newCurr;

        CurrentCell.WalkOn(Interactive);

        transform.position = new Vector3(CurrentCell.transform.position.x, transform.position.y, CurrentCell.transform.position.z);
    }

    public void SetLast()
    {
        Debug.Log("Curr? " + CurrentCell);
        LastCell = CurrentCell.CellValue;
    }
}
