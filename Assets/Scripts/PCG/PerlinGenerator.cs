using System;
using UnityEngine;

[Serializable]
public class PerlinGenerator
{
    [field:SerializeField] public bool Active { get; private set; } = true;
    [SerializeField] private float _tileSize = 1f;
    [SerializeField] private HeightFactor _heightFactor;
    [SerializeField] private float _maxHeight = 1f;

    public Vector3[] Generate(Vector3[] vertices, Vector3[] normals, Vector2 offset)
    {
        // map 2D perlin noise to 3D
        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].y > 0.99f ) // check if it's top face
            {
                vertices[i].y += Mathf.PerlinNoise(
                    _tileSize * (vertices[i].x + offset.x),
                    _tileSize * (vertices[i].z + offset.y));

                if  ( _heightFactor == HeightFactor.Scale )
                    vertices[i].y *= _maxHeight;
            }
        }

        if ( _heightFactor == HeightFactor.Normalize )
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            for (int i = 0; i < vertices.Length; i++)
            {
                if ( normals[i].y < 0.99f ) continue;

                if (vertices[i].y < min) min = vertices[i].y;
                else if (vertices[i].y > max) max = vertices[i].y;
            }

            float range = max - min;
            if (range != 0) // Avoid divide by zero
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    if ( normals[i].y < 0.99f ) continue;
                        vertices[i].y =
                            _maxHeight * (vertices[i].y - min) / range;
                }
            }
        }
        
        float minVal = float.PositiveInfinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].y < 0.99f ) continue;
            if (vertices[i].y < minVal) minVal = vertices[i].y;
        }
        
        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].y < 0.99f ) continue;
            vertices[i].y -= minVal;
        }

        return vertices;
    }

    private enum HeightFactor
    {
        Scale,
        Normalize
    }
}
