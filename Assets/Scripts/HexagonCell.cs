using System.Collections.Generic;
using UnityEngine;

public class HexagonCell : MonoBehaviour
{
    public Vector2 CellValue { get; private set; }
    private HexagonTabletop _tabletop;
    public bool Walkable { get; private set; } = true;
    [SerializeField] private GameObject _Cosmetic;
    private bool _pathed = false;
    private bool _hovered = false;

    public float G { get; private set; } = 0f; // Quantidade andada
    public float H { get; private set; } = 0f;  // Distancia do objetivo
    public float F { get; private set; } = 0f;  // Junção de G e H
    public HexagonCell Connection { get; private set; }  // Conexão momentanea

    public List<HexagonCell> Neighbors;

    public void WalkOn(TabletopMovement piece)
    {
        Walkable = false;
        // probably use the piece in teh future to communicate between them, create father class piece for tabletop movement and then branch into player and enemy movement
    }

    public void SetG(float g)
    {
        G = g;
    }
    public void SetH(float h)
    {
        H = h;
    }
    public void SetF(float f)
    {
        F = f;
    }
    public void SetConnection(HexagonCell connection)
    {
        Connection = connection;
    }
    public Vector2 InitializeCell(HexagonTabletop tabletop)
    {
        _tabletop = tabletop;

        // Pos = _q * new Vector2(Sqrt3, 0) + _r * new Vector2(Sqrt3 / 2, 1.5f);
        // We can just get the grid position, better in floats for hexagons in specific

        CellValue = new Vector2(
            Round(transform.localPosition.x, 2),
            Round(transform.localPosition.y, 2)
        );
        
        
        Debug.Log("Initializing " + this );
        
        SetNeighbors();

        if ( _Cosmetic == null )
            _Cosmetic = gameObject;

        return CellValue;
    }

    private float Round(float value, int decimals)
    {
        float scale = Mathf.Pow(10, decimals);
        return Mathf.Round(value * scale) / scale;
    }

    private void SetNeighbors()
    {
        Neighbors = new List<HexagonCell>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _tabletop.Grid.cellSize.y*0.75f);

        foreach ( Collider col in colliders )
            if (col.transform.parent.TryGetComponent(out HexagonCell neighbor))
                Neighbors.Add(neighbor);
        
        Debug.Log($"Neighbors of {this}:             {string.Join(", ", Neighbors)}");
    }

    /*public float GetDistance(ICoords other) => (this - (HexCoords)other).AxialLength();

    private static readonly float Sqrt3 = Mathf.Sqrt(3);

    public Vector2 Pos { get; set; }

    private int AxialCoords() {
        if (_q == 0 && _r == 0) return 0;
        if (_q > 0 && _r >= 0) return _q + _r;
        if (_q <= 0 && _r > 0) return -_q < _r ? _r : -_q;
        if (_q < 0) return -_q - _r;
        return -_r > _q ? -_r : _q;
    }

    public static HexCoords operator -(HexCoords a, HexCoords b) {
        return new HexCoords(a._q - b._q, a._r - b._r);
        
         |
         |
        V
        
        */


    // Using Axial Distance we can determine the distance in the 3 hexagonal directions with only 2 of these directions.
    // Its different than just using unity's transform.GetDistance because that gives the distance in a circle shape rather than the tabletop's desired hexagonal shape.     
    public float GetDistance(HexagonCell other)
    {
        // Getting vector to other from the current cell
        Vector2 dis = new(other.CellValue[0] - CellValue[0], other.CellValue[1] - CellValue[1]);

        // Calculating the axial distance
        if (dis.x == 0 && dis.y == 0) return 0;
        if (dis.x > 0 && dis.y >= 0) return dis.x + dis.y;
        if (dis.x <= 0 && dis.y > 0) return -dis.x < dis.y ? dis.y : -dis.x;
        if (dis.x < 0) return -dis.x - dis.y;
        return -dis.y > dis.x ? -dis.y : dis.x;
    }

    public void HoverCell()
    {
        if ( _hovered ) return;

        StopPathCell();

        _Cosmetic.transform.Translate(Vector3.up * 0.2f);

        _hovered = true;
    }
    public void StopHoverCell()
    {
        if ( !_hovered ) return;

        _Cosmetic.transform.Translate(Vector3.down * 0.2f);

        _hovered = false;
    }

    public void PathCell()
    {
        if ( _pathed ) return;

        _Cosmetic.transform.Translate(Vector3.up * 0.1f);

        _pathed = true;
    }
    public void StopPathCell()
    {
        if ( !_pathed ) return;

        _Cosmetic.transform.Translate(Vector3.down * 0.1f);

        _pathed = false;
    }

    public void SelectCell()
    {
        
    }
    public void SelectionError()
    {
        
    }

    public override string ToString()
    {
        return $"Hex({CellValue.x}, {CellValue.y})G:{G} H:{H} F:{F}";
    }
}
