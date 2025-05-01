using System;
using UnityEngine;

[Serializable]
public class HexagonCellState
{
    public Vector2 Position;
    public Modifier CurrentMod;
    public Modifier LastMod;
}