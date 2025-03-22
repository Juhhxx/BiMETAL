using System.Collections.Generic;

public class PieceInteractive : ModifierInteractive
{
    private TabletopController _controller;
    public EnemyTabletopMovement EnemyMovement { get; protected set; }

    // Add here other piece stats like if its player or the prefab it needs to load in battle or something

    protected override void Start()
    {
        EnemyMovement = GetComponentInChildren<EnemyTabletopMovement>();
        base.Start();
        _controller = FindFirstObjectByType<TabletopController>();
        Modified = true;
        _dynamic = true;
    }

    public override void Hover(bool onOrOff = true)
    {
        base.Hover();

        if ( EnemyMovement != null )
        {
            if (onOrOff)
                EnemyMovement.FindPath();
            else
                EnemyMovement.Stop();
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
        if ( _modifier == null ) return;

        ModifyAtCell(Cell);
    }


    public override void Path( ObservableStack<HexagonCell> other = null )
    {
        if ( _modifier == null ) return;

        if ( other == null )
            Modify();

        while ( other.Count > 1 )
            other.Pop();

        ModifyAtCell(other.Pop());
    }

    private void ModifyAtCell(HexagonCell cell)
    {
        _pathfinder.FindPath(cell, null, _reach);
    }
}