using System.Collections.Generic;
using UnityEngine;

public class HexagonCell : MonoBehaviour
{
    public Vector2 CellValue { get; private set; }
    private HexagonTabletop _tabletop;

    // Any enemy can pass through here, but will take down the piece
    public bool Walkable()
    {
        return Piece == null;
    }
    public bool IsNonAvoidable()
    {
        return Piece is ModifierInteractive && !Piece.Modified;
    }
    public int Weight
    {
        get { return 1 + (Modifier?.Weight ?? 0); }
    }

    public Interactive Piece { get; private set; }
    public Modifier Modifier { get; private set; }
    [SerializeField] private GameObject _Cosmetic;

    public List<HexagonCell> Neighbors;

    public bool WalkOn(Interactive piece = null)
    {
        if (Piece != null && piece != null)
            return false;
        Piece = piece;
        return true;
        // probably use the piece in the future to communicate between each piece, create father class piece for tabletop movement and then branch into player and enemy movement
    }


    // It needs to modify if its a modifier changing it but only modify if its null and its a piece changing it
    public bool Modify(Modifier mod, bool dynamic = false)
    {
        if (dynamic && Modifier != null)
            return false;

        Modifier = mod;

        _Cosmetic.GetComponent<Renderer>().material.color = Modifier.Color;

        SetPoints();

        return true;
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


        // Debug.Log("Initializing " + this );

        SetNeighbors();
        SetPoints();

        if (_Cosmetic == null)
            _Cosmetic = GetComponentInChildren<Renderer>().gameObject;

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

        Collider[] colliders = Physics.OverlapSphere(transform.position, _tabletop.Grid.cellSize.y * 0.75f);

        foreach (Collider col in colliders)
            if (col.transform.parent.TryGetComponent(out HexagonCell neighbor) && neighbor != this)
                Neighbors.Add(neighbor);

        // Debug.Log($"Neighbors of {this}:             {string.Join(", ", Neighbors)}");
    }

    private void SetPoints()
    {
        // viewer that changes material color for testing
        // Weight = Random.Range(1, 4);
        Material mat = GetComponentInChildren<Renderer>().material;

        float grayscaleValue = Mathf.Lerp(1f, 0f, (Weight - 1) / 2f) / 2f;
        mat.color = new Color(grayscaleValue, grayscaleValue, grayscaleValue);
    }


    // Using Axial Distance we can determine the distance in the 3 hexagonal directions with only 2 of these directions.
    // Its different than just using unity's transform.GetDistance because that gives the distance in a circle shape rather than the tabletop's desired hexagonal shape.     
    public float GetDistance(HexagonCell other)
    {
        // Getting vector to other from the current cell
        Vector2 dis = new(other.CellValue[0] - CellValue[0], other.CellValue[1] - CellValue[1]);

        // Are these heuristics correct? lol

        // Calculating the axial distance
        return Mathf.Max(Mathf.Abs(dis.x), Mathf.Abs(dis.y), Mathf.Abs(dis.x + dis.y));
    }

    private bool _hovered = false;
    public void HoverCell(bool onOrOff = true)
    {
        if (_hovered == onOrOff) return;

        if (_hovered)
            _Cosmetic.transform.Translate(Vector3.down * 0.2f);
        else if (!_hovered)
            _Cosmetic.transform.Translate(Vector3.up * 0.2f);

        _hovered = onOrOff;

        Piece?.Hover(_hovered);
    }

    private int _pathStack = 0;

    /// <summary>
    /// Increases the path highlight stack count, but only visually changes when transitioning from 0 to 1.
    /// </summary>
    public void PathCell()
    {
        if (_pathStack == 0)
        {
            _Cosmetic.transform.Translate(Vector3.up * 0.1f);

            /*if (Piece is ModifierInteractive piece)
                piece.Path();*/
        }

        _pathStack++;
    }

    /// <summary>
    /// Decreases the path highlight stack count, but only visually changes when transitioning from 0 to 1.
    /// </summary>
    public void StopPathCell()
    {
        _pathStack--;

        if (_pathStack <= 0)
        {
            _pathStack = 0;

            _Cosmetic.transform.Translate(Vector3.down * 0.1f);
        }
    }

    public void SelectCell()
    {
        Piece?.Select();
    }
    public void SelectionError()
    {
        Piece?.SelectionError();
    }

    public override string ToString()
    {
        return $"Hex({CellValue.x}, {CellValue.y}), W({Weight})";
    }

    public List<PieceInteractive> GetPieces(HashSet<HexagonCell> visited = null)
    {
        visited ??= new HashSet<HexagonCell>();

        List<PieceInteractive> result = new();

        if (visited.Contains(this))
            return result;

        visited.Add(this);

        if (Piece is PieceInteractive interactive)
            result.Add(interactive);


        foreach (var neighbor in Neighbors)
        {
            if (neighbor == null || visited.Contains(neighbor))
                continue;

            if (neighbor.Piece is PieceInteractive)
                result.AddRange(neighbor.GetPieces(visited));
        }

        return result;
    }
}
