using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for tabletop grid.
/// </summary>
public class HexagonTabletop : MonoBehaviour
{
    public Dictionary<Vector2, HexagonCell> CellDict { get; private set; }
    [field:SerializeField] public List<HexagonCell> Cells { get; set; }
    [SerializeField] private LayerMask _cells;
    public static LayerMask CellLayer;

    [field:SerializeField] public Grid Grid { get; private set; }

    public bool Done { get; private set; } = false;

    private void Awake()
    {
        CellLayer = _cells;

        CellDict = new Dictionary<Vector2, HexagonCell>();

        if ( Grid == null )
            Grid = GetComponent<Grid>();

        foreach ( HexagonCell cell in Cells )
            CellDict[cell.CellValue] = cell;
    }

    private void Start()
    {
        Debug.Log("Initializing Cells Neighbors. ");

        /*foreach (HexagonCell cell in Cells)
            cell.SetNeighbors();*/

        Done = true;
    }

    /// <summary>
    /// Clears any active path indicators from all hex cells.
    /// </summary>
    public void ResetPaths()
    {
        Debug.Log("level? Reset cells");
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
}
