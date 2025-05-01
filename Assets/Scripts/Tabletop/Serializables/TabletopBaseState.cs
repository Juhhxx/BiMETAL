using System;
using UnityEngine;

[Serializable]
public class TabletopBaseState
{
    public string PieceID;
    public Vector2 CurrentCell;
    public Vector2 LastCell;
    public bool Dead;
}