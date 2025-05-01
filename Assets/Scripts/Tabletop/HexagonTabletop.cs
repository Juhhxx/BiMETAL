using System.Collections.Generic;
using UnityEngine;

public class HexagonTabletop : MonoBehaviour
{
    public Dictionary<Vector2, HexagonCell> CellDict { get; private set; }
    public HexagonCell[] Cells { get; private set; }
    [SerializeField] private LayerMask _cells;
    public static LayerMask CellLayer;

    public Grid Grid { get; private set; }

    private void Awake()
    {
        CellLayer = _cells;

        CellDict = new Dictionary<Vector2, HexagonCell>();
        Grid = GetComponent<Grid>();

        CreateCells();
    }

    private void Start()
    {
        foreach (HexagonCell cell in Cells)
            cell.SetNeighbors();
    }

    public void CreateCells()
    {
        Cells = GetComponentsInChildren<HexagonCell>();

        // Debug.Log("Found " + cells.Length + " cells.");

        foreach (HexagonCell cell in Cells)
            CellDict[cell.InitializeCell(this)] = cell;

        // Debug.Log("Initialized " + Cells.Count + " cells.");
    }

    public void ResetPaths()
    {
        foreach (HexagonCell cell in Cells)
       {
            while ( cell.PathStack != 0 )
            {
                if ( cell.PathStack > 0 )
                    cell.StopPathCell();
                else if ( cell.PathStack < 0 )
                    cell.PathCell();
            }
       }
    }

    public HexagonCell GetCell(Vector2 pos) => CellDict.TryGetValue(pos, out HexagonCell tile) ? tile : null;
}
