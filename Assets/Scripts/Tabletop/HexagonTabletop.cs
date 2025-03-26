using System.Collections.Generic;
using UnityEngine;

public class HexagonTabletop : MonoBehaviour
{
    public Dictionary<Vector2, HexagonCell> Cells { get; private set; }
    [SerializeField] private LayerMask _cells;
    public static LayerMask CellLayer;

    public Grid Grid { get; private set; }

    private void Awake()
    {
        CellLayer = _cells;

        Cells = new Dictionary<Vector2, HexagonCell>();
        Grid = GetComponent<Grid>();

        CreateCells();
    }

    public void CreateCells()
    {
        HexagonCell[] cells = GetComponentsInChildren<HexagonCell>();

        // Debug.Log("Found " + cells.Length + " cells.");

        foreach (HexagonCell cell in cells)
            Cells[cell.InitializeCell(this)] = cell;
        foreach (HexagonCell cell in cells)
            cell.SetNeighbors();

        // Debug.Log("Initialized " + Cells.Count + " cells.");
    }

    public HexagonCell GetCell(Vector2 pos) => Cells.TryGetValue(pos, out HexagonCell tile) ? tile : null;

    // pass cell visualization handleing to here
}
