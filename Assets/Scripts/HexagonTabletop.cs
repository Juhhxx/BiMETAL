using System.Collections.Generic;
using UnityEngine;

public class HexagonTabletop : MonoBehaviour
{
    public Dictionary<Vector2, HexagonCell> Cells { get; private set; }

    public Grid Grid { get; private set; }

    private void Awake()
    {
        Cells = new Dictionary<Vector2, HexagonCell>();
        Grid = GetComponent<Grid>();

        CreateCells();
    }

    public void CreateCells()
    {
        HexagonCell[] cells = GetComponentsInChildren<HexagonCell>();

        Debug.Log("Found " + cells.Length + " cells.");

        foreach ( HexagonCell cell in cells)
            Cells[cell.InitializeCell(this)] = cell;
        
        Debug.Log("Initialized " + Cells.Count + " cells.");
    }

    public HexagonCell GetCell(Vector2 pos) => Cells.TryGetValue(pos, out var tile) ? tile : null;

    public int GridFloatIntoInt(HexagonCell hexCell)
    {
        if ( hexCell == null ) return 1000;

        int total = 0;

        Debug.Log("hex H len: " + hexCell.H);

        /* for flat top art sprites W✕H, set size to Point(W/2, H/sqrt(3)). The example fits 100✕100 sprites that are slightly taller.
for pointy top art sprites W✕H, set size to Point(W/sqrt(3), H/2). The example fits 100✕100 sprites that are slightly wider.*/
        // Z is up, pointy top

        // Pos = _q * new Vector2(Sqrt3, 0) + _r * new Vector2(Sqrt3 / 2, 1.5f);

        // Point(size.x * sqrt(3)/2, size.y)

        // Vector2Int newPlacement =  (Vector2Int) transform.localPosition * new Vector2Int(
        // Grid.cellSize.x * sqrt(3)/2,
        // Grid.cellSize.y)
        Vector2 newFloatPlacement = new();

        switch ( Grid.cellSwizzle )
        {
            case  GridLayout.CellSwizzle.XYZ :
            {
                newFloatPlacement =  hexCell.transform.localPosition * new Vector2(
                Grid.cellSize.x * Mathf.Sqrt(3),
                Grid.cellSize.y);
                break;
            }
            case  GridLayout.CellSwizzle.YXZ :
            {
                newFloatPlacement =  hexCell.transform.localPosition * new Vector2(
                Grid.cellSize.x,
                Grid.cellSize.y * Mathf.Sqrt(3));
                break;
            }
            default:
            {
                Debug.Log("ERROR");
                break;
            }
        }

        Debug.Log("This many points away: " + total);
        Debug.Log("Pos in fake int grid: " + newFloatPlacement);

        return total;
    }
}
