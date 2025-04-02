using System.Collections.Generic;
using UnityEngine;

public class PieceInteractive : ModifierInteractive
{
    private TabletopController _controller;
    public bool IsEnemy { get; protected set; }
    // [SerializeField] protected PathfinderType _rangeType;
    public EnemyTabletopMovement EnemyMovement { get; protected set; }

    // Add here other piece stats like if its player or the prefab it needs to load in battle or something

    protected override void Start()
    {
        base.Start();

        _controller = FindFirstObjectByType<TabletopController>();

        IsEnemy = _base is EnemyTabletopMovement;

        if ( IsEnemy )
        {
            EnemyMovement = _base as EnemyTabletopMovement;

            if ( HasModifier )
                Modify();
        }
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
                _modPathfinder.Stop();
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
                piece._modPathfinder.Stop();

        _controller.StartBattle(Cell.Modifier, pieces);
    }

    public override void Select()
    {
        if ( ! IsEnemy ) return;
        base.Select();
    }

    public override void Modify()
    {
        if ( ! HasModifier ) return;

        _modPathfinder.Stop();
        _modPathfinder.FindPath(Cell, null, _reach);
    }

    public void Stop()
    {
        if ( ! HasModifier ) return;

        Debug.Log("Stopping mod in: " + gameObject.name);

        _modPathfinder.Stop();
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
            _modPathfinder.FindPath(Cell, null, _reach);
            return;
        }
        
        // remove mods from any piece "going into battle"
        foreach (PieceInteractive piece in pieces)
            if ( piece.IsEnemy )
                piece._modPathfinder.Stop();*/
        
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
        _modPathfinder.FindPath(Cell, null, _reach);
    }*/
}