using System.Collections.Generic;

public class PieceInteractive : ModifierInteractive
{
    private TabletopController _controller;
    protected new bool _dynamic = true;

    // Then add other piece stats like if its player or the prefab it needs to load in battle

    protected override void Start()
    {
        base.Start();
        _controller = FindFirstObjectByType<TabletopController>();
        _modified = true;
    }

    public override void Hover(bool onOrOff = true)
    {

    }

    public override void Interact(Interactive other)
    {
        UpdateCurrentCell();
        
        List<PieceInteractive> pieces = Cell.GetPieces();
        _controller.StartBattle(Cell.Modifier, pieces);
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

        while ( other.Count > 1 )
            other.Pop();

        ModifyAtCell(other.Pop());
    }

    private void ModifyAtCell(HexagonCell cell)
    {
        _pathfinder.FindPath(cell, null, _reach);
    }
}