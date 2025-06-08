using System;
using UnityEngine;

[Serializable]
public abstract class Generator
{
    [field:SerializeField] public bool Active { get; private set; } = true;
    [SerializeField] protected float _tileSize = 1f;
    [SerializeField] protected HeightFactor _heightFactor;
    [SerializeField] protected float _maxHeight = 1f;

    public abstract Vector3[] Generate(Vector3[] vertices, Vector3[] normals, Vector2 offset);

    protected enum HeightFactor
    {
        Scale,
        Normalize
    }
}
