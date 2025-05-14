using System;
using System.Collections.Generic;

[Serializable]
public class OverworldState
{
    public OverworldState()
    {
        LevelStates = new();
        Player = new();
    }
    public List<LevelState> LevelStates;
    public TabletopBaseState Player;
}