using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single hexagonal cell on the tabletop.
/// </summary>
public class HexagonCell : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private GameObject _hoverObject;
    [SerializeField] private GameObject _pathObject;

    [field:SerializeField] public Vector2 CellValue { get; private set; }
    [SerializeField] private HexagonTabletop _tabletop;

    // Any enemy can pass through here, but will take down the piece
    public Interactive Piece { get; private set; }


    public Modifier Modifier => GetTopTemporaryMod() != null ? GetTopTemporaryMod() : (_environmentMod != null ? _environmentMod : _dynamicMod);

    private Modifier GetTopTemporaryMod()
    {
        _temporaryMod ??= new();
        foreach ( Modifier mod in _temporaryMod )
            if ( mod.NonWalkable )
                return mod;
        
        if ( _temporaryMod.Count > 0 )
            return _temporaryMod[^1];
        
        return null;
    }
    
    public Modifier EnvironmentMod => _environmentMod;

    private Modifier _dynamicMod;
    [field:SerializeField] private List<Modifier> _temporaryMod = new();
    [SerializeField] private Modifier _environmentMod;
    public Modifier LastMod { get; private set; }


    [SerializeField] private GameObject _cosmetic;
    [SerializeField] private Renderer _renderer;

    [field:SerializeField] public HexagonCell[] Neighbors { get; private set; }

    public bool Walkable() => Piece == null && ( Modifier ? ! Modifier.NonWalkable : true );

    private void Start()
    {
        if (_renderer == null)
            _renderer = _cosmetic.GetComponentInChildren<Renderer>();

        CosmeticModify();
    }

    public bool IsNonAvoidable()
    {
        EnvironmentInteractive envi = Piece as EnvironmentInteractive;
        if ( envi != null )
            Debug.Log("enviornment variable? " + envi.Name);
        return envi != null && !envi.Modified;
    }

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
        // Debug.Log("Hex: " + this + "     Modifying to: " + mod + " from: " + Modifier);

        // if ( Modifier != null && Modifier.NonWalkable && Modifier != mod ) return false;

        /*if ( mod.Dynamic && Modifier != null && Modifier != mod )
            return false;*/

        // Changed modifier way of knowing if its dynamic or not

        if (mod.Dynamic)
            _dynamicMod = ( _dynamicMod == mod ) ? null : mod;
        else // Envionrment mod still needs to be able to get it to null despite the games visual behavior because of pathing visuals ( EnvironmentModifier )
        {
            if ( _temporaryMod.Contains(mod) )
                _temporaryMod.Remove(mod);
            else
                _temporaryMod.Add(mod);
        }

        CosmeticModify();

        return true;
    }
    public void SetEnvironment(Modifier mod)
    {
        if ( _environmentMod != null && _environmentMod.NonWalkable ) return;

        // Debug.Log("get mod? set envi: " + mod);
        if ( _temporaryMod.Contains(mod) )
            _environmentMod = mod;
    }

    public void SetMod( Modifier mod )
    {
        if ( mod != null )
        {
            Modify(mod);
            SetEnvironment(mod);
        }
    }

    public void SetLast()
    {
        LastMod = _environmentMod;
    }

    private void CosmeticModify()
    {
        if ( Modifier != null )
        {
            if ( Modifier.Material != null )
            {
                _cosmetic.GetComponentInChildren<Renderer>().material = Modifier.Material;
            }
            else
            {
                _cosmetic.GetComponentInChildren<Renderer>().material = _defaultMaterial;
                _cosmetic.GetComponentInChildren<Renderer>().material.color = Modifier.Color;
            }
        }
        else
        {
            _cosmetic.GetComponentInChildren<Renderer>().material = _defaultMaterial;
            _cosmetic.GetComponentInChildren<Renderer>().material.color = _defaultColor;
        }
    }

    /// <summary>
    /// Initializes the cell's logical coordinate and assigns the tabletop reference.
    /// </summary>
    public Vector2 InitializeCell(HexagonTabletop tabletop)
    {
        _tabletop = tabletop;
        // Pos = _q * new Vector2(Sqrt3, 0) + _r * new Vector2(Sqrt3 / 2, 1.5f);
        // We can just get the grid position, better in floats for hexagons in specific

        CellValue = new Vector2(
            Round(transform.localPosition.x, 2),
            Round(transform.localPosition.y, 2)
        );

        return CellValue;
    }

    private float Round(float value, int decimals)
    {
        float scale = Mathf.Pow(10, decimals);
        return Mathf.Round(value * scale) / scale;
    }

    public void SetNeighbors()
    {
        Neighbors = new HexagonCell[6];
        Collider[] colliders = Physics.OverlapSphere(transform.position, _tabletop.Grid.cellSize.x * 0.75f);

        Debug.Log("find neighbor, tabletop null? " + _tabletop);

        foreach (Collider col in colliders)
        {
            // Debug.Log("neighbor");
            Transform parent = col.transform.parent;
            if (parent == null || ! parent.TryGetComponent(out HexagonCell neighbor) || neighbor == this)
                continue;

            Vector2 delta = neighbor.CellValue - CellValue;
            delta = delta.normalized;

            // if ( Piece != null && Piece is PieceInteractive piece && ! piece.IsEnemy )
            // Debug.Log("neighbor cel: " + neighbor + "    dir: " + (neighbor.CellValue - CellValue) + "    int dir: " + delta);


            // :(
            // this works better than my correct code

            if ( delta.x > 0 && delta.y > 0 )
                Neighbors[0] = neighbor;
            if ( delta.x > 0 && delta.y == 0 )
                Neighbors[1] = neighbor;
            if ( delta.x > 0 && delta.y < 0 )
                Neighbors[2] = neighbor;
            if ( delta.x < 0 && delta.y < 0 )
                Neighbors[3] = neighbor;
            if ( delta.x < 0 && delta.y == 0 )
                Neighbors[4] = neighbor;
            if ( delta.x < 0 && delta.y > 0 )
                Neighbors[5] = neighbor;
        }
    }

    /// <summary>
    /// Using Axial Distance we can determine the distance in the 3 hexagonal directions with only 2 of these directions.
    /// Its different than just using unity's transform.GetDistance because that gives the distance in a circle shape rather than the tabletop's desired hexagonal shape.     
    /// </summary> 
    public float GetDistance(HexagonCell other)
    {
        // Getting vector to other from the current cell
        // Vector2 dis = new(other.CellValue[0] - CellValue[0], other.CellValue[1] - CellValue[1]);
        Vector2 dis = other.CellValue - CellValue;

        // Are these heuristics correct? lol

        // Calculating the axial distance
        float distance = Mathf.Max(
            Mathf.Abs(dis.x),
            Mathf.Abs(dis.y),
            Mathf.Abs(dis.x + dis.y)
        );

        // Draw a debug ray from this cell to the other (in world space)
        // Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.magenta, 1f);
        // Debug.Log("Distance: " + distance);

        return distance;
    }

    private bool _hovered = false;
    public void HoverCell(bool onOrOff = true)
    {
        if (_hovered == onOrOff) return;

        // Debug.Log("Hovering cell: " + this);

        if (_hovered)
            _hoverObject.SetActive(false);
        else if (!_hovered)
            _hoverObject.SetActive(true);

        _hovered = onOrOff;

        Piece?.Hover(_hovered);
    }

    public int PathStack { get; private set; } = 0;

    /// <summary>
    /// Increases the path highlight stack count, but only visually changes when transitioning from 0 to 1.
    /// </summary>
    public void PathCell()
    {
        if (PathStack == 0)
        {
            CosmeticPathCell(true);
        }

        // Debug.Log("envi path cell");
        PathStack++;
    }
    
    private void CosmeticPathCell(bool upOrDown)
    {
        // Debug.Log("Cosmetic turning: " + upOrDown + " at cell: " + this + " with piece null? " + (Piece == null));
        
        if ( upOrDown )
        {
            _pathObject.SetActive(true);
            _cosmetic.transform.Translate(Vector3.up * 0.1f);
        }
        else
        {
            _pathObject.SetActive(false);
            _cosmetic.transform.Translate(Vector3.down * 0.1f);
        }
    }

    /// <summary>
    /// Decreases the path highlight stack count, but only visually changes when transitioning from 0 to 1.
    /// </summary>
    public void StopPathCell()
    {
        PathStack--;

        if (PathStack == 0)
        {
            // Debug.Log("Stop path count: " + PathStack);
            CosmeticPathCell(false);
            
            // PathStack = 0;
        }
        
        // Debug.Log("envi un path cell");
    }

    public void SelectCell()
    {
        Piece?.Select();
    }
    public void SelectionError()
    {
        Piece?.SelectionError();
    }

    public override string ToString() => $"Hex({CellValue.x}, {CellValue.y}), W({Weight}), P({Piece} is {Piece?.gameObject.name}), N({PathStack})";

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

    public void AwakeRegion()
    {
        if ( _environmentMod != null )
        {
            _environmentMod = null;
            _temporaryMod = null;
            CosmeticModify();
        }

        _animator.enabled = true;
        _animator.Play("AwakeRegion");

        Debug.Log("level? Awakening region mod: " + Modifier);

        if ( Piece != null )
            Piece.AwakeRegion();
    }

    public void DisableAnimator()
    {
        _animator.enabled = false;
    }

    public void SleepRegion(Modifier unavailable)
    {
        SetMod(unavailable);

        // _animator.Play("SleepRegion");

        if ( Piece != null )
            Piece.SleepRegion();
    }
}
