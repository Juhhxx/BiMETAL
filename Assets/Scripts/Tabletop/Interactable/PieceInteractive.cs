using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInteractive : ModifierInteractive
{
    private TabletopController _controller;
    public EnemyTabletopMovement EnemyMovement { get; protected set; }

    // Add here other piece stats like if its player or the prefab it needs to load in battle or something

    protected override void Start()
    {
        EnemyMovement = GetComponentInChildren<EnemyTabletopMovement>();
        _controller = FindFirstObjectByType<TabletopController>();
        Modified = true;
        _dynamic = true;
    }

    public override void Hover(bool onOrOff = true)
    {
        base.Hover(onOrOff);

        if (EnemyMovement != null)
        {
            if (onOrOff)
            {
                EnemyMovement.FindPath();
                // Path(EnemyMovement.Path);
            }
            else
            {
                EnemyMovement.Stop();
                // Path();
            }
        }
    }

    public override void Interact(Interactive other = null)
    {
        UpdateCurrentCell();

        List<PieceInteractive> pieces = Cell.GetPieces();
        _controller.StartBattle(_modifier, pieces);
    }

    public override void Select()
    {
        throw new System.NotImplementedException();
    }

    public override void Modify()
    {
        if (_modifier == null) return;

        StartCoroutine(ModifyAtCell(Cell));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"> other here seems to be getting passed as a reference? </param>
    public override void Path(ObservableStack<HexagonCell> other = null)
    {
        if (_modifier == null) return;

        if ( other == null || other.Count <= 0 )
        {
            Modify();
            return;
        }
        
        StartCoroutine(ModifyAtCell(other.Peek()));
    }

    private IEnumerator ModifyAtCell(HexagonCell cell)
    {
        yield return new WaitUntil(() => EnemyMovement.Pathfinder.Done);

        // here the pathfinder should get the ranged enemies range
        // _pathfinder.FindPath(cell, null, _reach);
    }
}