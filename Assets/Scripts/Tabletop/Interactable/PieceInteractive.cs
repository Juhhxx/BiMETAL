using System.Collections;
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

        IsEnemy = _base is EnemyTabletopMovement;

        if ( IsEnemy )
            EnemyMovement = _base as EnemyTabletopMovement;

        _controller = FindFirstObjectByType<TabletopController>();
        Modified = true;
        _dynamic = true;


        if (_modifier != null)
            _modPathfinder.FindPath(Cell, null, _reach);
    }

    public override void Hover(bool onOrOff = true)
    {
        base.Hover(onOrOff);

        if ( IsEnemy )
        {
            if (onOrOff)
            {
                EnemyMovement.FindPath();
                // Path(EnemyMovement.Path);
                if ( HasModifier )
                {
                    Debug.Log("De-modifying. ");
                    _modPathfinder.Stop();
                }

            }
            else
            {
                EnemyMovement.Stop();
                // Path();
                if ( HasModifier )
                    _modPathfinder.FindPath(Cell, null, _reach);
            }
        }
    }

    public override void Interact(Interactive other = null)
    {
        List<PieceInteractive> pieces = Cell.GetPieces();
        _controller.StartBattle(Cell.Modifier, pieces);
    }

    public override void Select()
    {
        throw new System.NotImplementedException();
    }

    public override void Modify()
    {
        if (_modifier == null) return;

        // StartCoroutine(ModifyAtCell());
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"> other here seems to be getting passed as a reference? </param>
    public override void Path(ObservableStack<HexagonCell> other = null)
    {
        if ( ! HasModifier ) return;

        if ( other == null || other.Count <= 0 )
        {
            Modify();
            return;
        }
        
        // remove for now because he are using a bfs so there is no definite objective
        // StartCoroutine(ModifyAtCell(other.Peek()));
    }

    private IEnumerator ModifyAtCell()
    {
        yield return new WaitUntil(() => EnemyMovement.Pathfinder.Done);

        // here the pathfinder should get the ranged enemies range
        _modPathfinder.FindPath(Cell, null, _reach);
    }
}