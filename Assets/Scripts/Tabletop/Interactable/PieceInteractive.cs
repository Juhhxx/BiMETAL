using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInteractive : ModifierInteractive
{
    [field:SerializeField] public Character Character { get; private set; }
    private TabletopController _controller;
    public bool IsEnemy { get; protected set; }
    // [SerializeField] protected PathfinderType _rangeType;
    public TabletopMovement Movement { get; protected set; }
    public EnemyTabletopMovement EnemyMovement { get; protected set; }
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Rigidbody _rigidBody;

    public bool Dead { get; private set; } = false;

    // Add here other piece stats like if its player or the prefab it needs to load in battle or something

    protected override void Start()
    {
        base.Start();

        if ( _base == null )
            Debug.Log("Base is null on init");

        EnemyMovement = _base as EnemyTabletopMovement;
        if ( EnemyMovement != null )
        {
            IsEnemy = true;
            Movement = EnemyMovement;
            Debug.Log("Init Movement in piece " + gameObject.name);

            if (HasModifier)
                Modify();
        }
        else
        {
            PlayerTabletopMovement baseP = _base as PlayerTabletopMovement;
            if (baseP != null)
            {
                Movement = baseP;
                Debug.Log("Init Movement in piece " + gameObject.name);
            }
        }

        _rigidBody.constraints =
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ;
    }

    public override void Hover(bool onOrOff = true)
    {
        if ( ! IsEnemy ) return;
        base.Hover(onOrOff);

        if (onOrOff)
        {
            EnemyMovement.FindPath();

            if ( HasModifier )
            {
                Debug.Log("De-modifying. ");
                ModPathfinder.Stop();
            }

        }
        else
        {
            EnemyMovement.Stop();

            if ( HasModifier )
                Modify();
        }
    }

    public override void Interact(Interactive other = null)
    {
        List<PieceInteractive> pieces = Cell.GetPieces();
        
        // remove mods from any piece going into battle
        foreach (PieceInteractive piece in pieces)
            if ( piece.IsEnemy && piece.HasModifier )
                piece.ModPathfinder.Stop();

        _controller = FindFirstObjectByType<TabletopController>();
        _controller.StartBattle(Cell, pieces);
    }

    public override void Select()
    {
        if ( ! IsEnemy ) return;
        base.Select();
    }

    public override void Modify()
    {
        if ( ! HasModifier ) return;

        ModPathfinder.Stop();
        ModPathfinder.FindPath(Cell, null, Reach);
    }

    public void Stop()
    {
        if ( ! HasModifier ) return;

        Debug.Log("Stopping mod in: " + gameObject.name);

        ModPathfinder.Stop();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"> other here seems to be getting passed as a reference? </param>
    public override void Path(ObservableStack<HexagonCell> other = null)
    {
        if ( ! HasModifier ) return;


        /*List<PieceInteractive> pieces = Cell.GetPieces();
        
        // remove mods from any piece going into battle
        foreach (PieceInteractive piece in pieces)
            if ( piece.IsEnemy )
                piece.Modify();

        if ( other == null || other.Count <= 0 )
        {
            ModPathfinder.FindPath(Cell, null, _reach);
            return;
        }
        
        // remove mods from any piece "going into battle"
        foreach (PieceInteractive piece in pieces)
            if ( piece.IsEnemy )
                piece.ModPathfinder.Stop();*/
        
        // remove for now because he are using a bfs so there is no definite objective
        // StartCoroutine(ModifyAtCell(other.Peek()));
    }

    /*
    This only exists if we decide to let the player view the exact path of the enemy
    when they are hovered
    
    private IEnumerator ModifyAtCell()
    {
        yield return new WaitUntil(() => EnemyMovement.Pathfinder.Done);

        // here the pathfinder should get the ranged enemies range
        ModPathfinder.FindPath(Cell, null, _reach);
    }*/

    public void Die(Vector2 awayPos)
    {
        Dead = true;

        StartCoroutine( DieRoutine(awayPos) );
    }

    public void Hurt()
    {
        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        if (_renderer == null)
            _renderer = GetComponentInChildren<Renderer>();

        Color original = _renderer.material.color;
        Color noColor = new Color(1f, 1f, 1f, 0f);

        yield return new WaitUntil( () => ! SceneLoader.IsLoading );

        for (int i = 0; i < 6f; i++)
        {
            _renderer.material.color = noColor;
            yield return new WaitForSeconds( 0.1f );
            _renderer.material.color = original;
            yield return new WaitForSeconds( 0.1f );
        }
    }

    private IEnumerator DieRoutine(Vector2 awayPos)
    {
        yield return new WaitUntil( () => ! SceneLoader.IsLoading );

        _rigidBody.constraints = RigidbodyConstraints.None;

        Vector3 forceDirection = new Vector3(awayPos.x, -1f, awayPos.y).normalized;
        _rigidBody.AddForce(forceDirection * 9f, ForceMode.Impulse);

        yield return StartCoroutine(HurtRoutine());

        gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Dead = true;
    }
}