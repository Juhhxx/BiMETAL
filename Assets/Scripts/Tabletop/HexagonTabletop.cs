using System.Collections.Generic;
using UnityEngine;

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

    public HexagonCell GetCell(Vector2 pos) => CellDict.TryGetValue(pos, out HexagonCell tile) ? tile : null;
}
