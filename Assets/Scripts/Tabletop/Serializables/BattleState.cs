using System;
using System.Collections.Generic;

[Serializable]
public class BattleState
{
    public BattleState()
    {
        CellStates = new();
        PieceStates = new();
        ModifierStates = new();
    }
    public List<HexagonCellState> CellStates;
    public List<TabletopBaseState> PieceStates;
    public List<ModifierState> ModifierStates;
}