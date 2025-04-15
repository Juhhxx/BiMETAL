using System.Collections.Generic;
using UnityEngine;

public class HexagonCell : MonoBehaviour
{
    public Vector2 CellValue { get; private set; }
    private HexagonTabletop _tabletop;

    // Any enemy can pass through here, but will take down the piece
    public Interactive Piece { get; private set; }


    public Modifier Modifier =>  _environmentMod ?? _dynamicMod;

    private Modifier _dynamicMod;
    private Modifier _environmentMod;


    [SerializeField] private GameObject _Cosmetic;

    public HexagonCell[] Neighbors { get; private set; }

    public bool Walkable() => Piece == null && ( Modifier ? ! Modifier.NonWalkable : true );

    public bool IsNonAvoidable() => Piece is EnvironmentInteractive piece && !piece.Modified;

    public int Weight => 1 + (Modifier != null ? Modifier.Weight : 0);

    public bool WalkOn(Interactive piece = null)
    {
        if (Piece != null && piece != null)
            return false;
        
        Piece = piece;

        return true;
        // probably use the piece in the future to communicate between each piece, create father class piece for tabletop movement and then branch into player and enemy movement
    }

    /// <summary>
    /// For blocking modifiers
    /// </summary>
    public void CutConnections()
    {
        for ( int i = 0; i < 6; i++)
        {
            if ( TryGetNeighborInDirection(i, out HexagonCell cell ))
            {
                if ( cell == null ) continue;

                cell.CutConnection(ReverseDirection(i));
            }
            cell.Neighbors[i] = null;
        }
    }

    /// <summary>
    /// Neighbor set is private, must cut connection some other way
    /// </summary>
    /// <param name="dir"> the direction to cut the neighbor of. </param>
    public void CutConnection(int dir) => Neighbors[dir] = null;

    // It needs to modify if its a modifier changing it but only modify if its null and its a piece changing it
    public bool Modify(Modifier mod)
    {
        Debug.Log("Hex: " + this + "     Modifying to: " + mod + " from: " + Modifier);

        /*if ( mod.Dynamic && Modifier != null && Modifier != mod )
            return false;*/

        // Changed modifier way of knowing if its dynamic or not

        if (mod.Dynamic)
            _dynamicMod = ( _dynamicMod == mod ) ? null : mod;
        else // Envionrment mod still needs to be able to get it to null despite the games visual behavior because of pathing visuals ( EnvironmentModifier )
            _environmentMod = ( _environmentMod == mod ) ? null : mod;

        CosmeticModify();

        return true;
    }

    private void CosmeticModify()
    {
        _Cosmetic.GetComponentInChildren<Renderer>().material.color = Modifier? Modifier.Color : Color.white;
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

        if (_Cosmetic == null)
            _Cosmetic = GetComponentInChildren<Renderer>().gameObject;


        CosmeticModify();

        return CellValue;
    }

    private float Round(float value, int decimals)
    {
        float scale = Mathf.Pow(10, decimals);
        return Mathf.Round(value * scale) / scale;
    }

    /* pointy example:
    0 East (+q, 0)
    1 Northeast (+q, -r)
    2 Northwest (0, -r)
    3 West (−q, 0)
    4 Southwest (−q, +r)
    5 Southeast (0, +r)
    traditional flat:
        new(+1, 0),
        new(+1, -1),
        new(0, -1),
        new(-1, 0),
        new(-1, +1),
        new(0, +1)
    traditional pointy:
        new(0, +1),
        new(+1, 0),
        new(+1, -1),
        new(0, -1),
        new(-1, 0),
        new(-1, +1)
    */
    private static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new(+1, +1),
        new(+1, 0),
        new(+1, -1),
        new(-1, -1),
        new(-1, 0),
        new(-1, +1)
    };

    public void SetNeighbors()
    {
        Neighbors = new HexagonCell[6];

        Vector2Int[] directions = Directions;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _tabletop.Grid.cellSize.x * 0.75f);

        foreach (Collider col in colliders)
        {
            if (!col.transform.parent.TryGetComponent(out HexagonCell neighbor) || neighbor == this)
                continue;

            Vector2 delta = neighbor.CellValue - CellValue;
            delta = delta.normalized;

           Vector2Int dir = new(
                Mathf.RoundToInt(delta.x),
                Mathf.RoundToInt(delta.y)
            );

            // Debug.Log("neighbor cel: " + neighbor + "    dir: " + (neighbor.CellValue - CellValue) + "    normal: " + delta + "    int dir: " + dir);

            for (int i = 0; i < 6; i++)
                if (dir == directions[i])
                {
                    // Debug.Log("new neighbor " + i + "  cel: " + neighbor);
                    Neighbors[i] = neighbor;
                    break;
                }
        }

        /*for ( int i = 0 ; i < 6 ; i++ )
            Debug.Log("neighbor " + i + "  for dir: " + directions[i] + "  cel: " + Neighbors[i]);*/
    }


    // Using Axial Distance we can determine the distance in the 3 hexagonal directions with only 2 of these directions.
    // Its different than just using unity's transform.GetDistance because that gives the distance in a circle shape rather than the tabletop's desired hexagonal shape.     
    public float GetDistance(HexagonCell other)
    {
        // Getting vector to other from the current cell
        // Vector2 dis = new(other.CellValue[0] - CellValue[0], other.CellValue[1] - CellValue[1]);
        Vector2 dis = other.CellValue - CellValue;

        // Are these heuristics correct? lol

        // Calculating the axial distance
        return Mathf.Max(Mathf.Abs(dis.x), Mathf.Abs(dis.y), Mathf.Abs(dis.x + dis.y));
    }

    private bool _hovered = false;
    public void HoverCell(bool onOrOff = true)
    {
        if (_hovered == onOrOff) return;

        Debug.Log("Hovering cell: " + this);

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
            if ( Piece == null )
                CosmeticPathCell(true);
        }

        _pathStack++;
    }
    
    private void CosmeticPathCell(bool upOrDown)
    {
        Debug.Log("Cosmetic turning: " + upOrDown + " at cell: " + this + " with piece null? " + (Piece == null));
        
        if ( upOrDown )
            _Cosmetic.transform.Translate(Vector3.up * 0.1f);
        else
            _Cosmetic.transform.Translate(Vector3.down * 0.1f);
    }

    /// <summary>
    /// Decreases the path highlight stack count, but only visually changes when transitioning from 0 to 1.
    /// </summary>
    public void StopPathCell()
    {
        _pathStack--;

        if (_pathStack <= 0)
        {
            Debug.Log("Stop path count: " + _pathStack);
            CosmeticPathCell(false);
            
            // _pathStack = 0;
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

    public override string ToString() => $"Hex({CellValue.x}, {CellValue.y}), W({Weight}), P({Piece} is {Piece?.gameObject.name}), N({_pathStack})";

    public List<PieceInteractive> GetPieces(HashSet<HexagonCell> visited = null)
    {
        visited ??= new HashSet<HexagonCell>();

        List<PieceInteractive> result = new();

        if (visited.Contains(this))
            return result;

        visited.Add(this);

        if (Piece is PieceInteractive interactive)
            result.Add(interactive);


        foreach (HexagonCell neighbor in Neighbors)
            if (neighbor != null && !visited.Contains(neighbor) && neighbor.Piece is PieceInteractive)
                result.AddRange(neighbor.GetPieces(visited));

        return result;
    }

    public int GetDirectionToNeighbor(HexagonCell other)
    {
        for (int dir = 0; dir < 6; dir++)
            if (GetNeighborInDirection(dir) == other)
                return dir;
        // it's not a neighbor
        return -1;
    }
    // if we can only do get neighbor in direction, we can still use reverse to get the next direction
    public static int ReverseDirection(int direction) => (direction + 3) % 6;

    public HexagonCell GetNeighborInDirection(int direction)
    {
        if (direction < 0 || direction > 5) return null;

        return Neighbors[direction];
    }
    public bool TryGetNeighborInDirection(int direction, out HexagonCell neighbor)
    {
        neighbor = GetNeighborInDirection(direction);
        return neighbor != null;
    }
}
