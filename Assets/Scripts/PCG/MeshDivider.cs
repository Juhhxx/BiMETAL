using System.Collections.Generic;
using UnityEngine;

public class MeshDivider : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private int _numSubdivisions = 1;

    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<Vector2> _uvs;

    private Dictionary<uint,int> _newVectices;

    public void Generate()
    {
        for ( int i = 0 ; i < _numSubdivisions ; i++ )
            Subdivide();
    }

    private void Subdivide()
    {
        Mesh mesh = _meshFilter.mesh;

        _newVectices = new Dictionary<uint,int>();
        List<int> newTriangles = new List<int>();

        _vertices = new List<Vector3>(mesh.vertices);
        _normals = new List<Vector3>(mesh.normals);
        _uvs = new List<Vector2>(mesh.uv);
        int[] triangles = mesh.triangles;
        
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // gets 3 adjacent vertices to from the triangle
            int i1 = triangles[i + 0];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            if ( CheckNormal(_vertices[i1], _vertices[i2], _vertices[i3]) ) // subdivide into 3 more triangles
            {
                int a = GetNewVertex(i1, i2);
                int b = GetNewVertex(i2, i3);
                int c = GetNewVertex(i3, i1);

                newTriangles.Add(i1);   newTriangles.Add(a);   newTriangles.Add(c);
                newTriangles.Add(i2);   newTriangles.Add(b);   newTriangles.Add(a);
                newTriangles.Add(i3);   newTriangles.Add(c);   newTriangles.Add(b);
                newTriangles.Add(a );   newTriangles.Add(b);   newTriangles.Add(c);
            }
            else // keep original triangle
            {
                newTriangles.Add(i1);
                newTriangles.Add(i2);
                newTriangles.Add(i3);
            }
        }
        mesh.vertices = _vertices.ToArray();
        mesh.normals = _normals.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = _uvs.ToArray();

        _vertices.Clear();
        _normals.Clear();
        _newVectices.Clear();
        _uvs.Clear();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateUVDistributionMetrics();
        _meshFilter.mesh = mesh;
    }

    private bool CheckNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // cant use triangle because it can give slanted triangles as top side
        Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

        return normal.y > 0.99f;
    }

    private int GetNewVertex(int i1, int i2)
    {
        // We have to test both directions since the edge
        // could be reversed in another triangle

        // this means that the vertices may store i1 -> i2
        // and therefore we would run this in subdivide for that connection
        // but they may also be storing i2 -> i1
        // so we have to make sure to not create double midpoints for this connection
        // for that we have _newVectices to keep track of the midpoints that have already been made

        uint t1 = ((uint)i1 << 16) | (uint)i2;
        uint t2 = ((uint)i2 << 16) | (uint)i1;

        if (_newVectices.ContainsKey(t2))
            return _newVectices[t2];
        if (_newVectices.ContainsKey(t1))
            return _newVectices[t1];
        
        // Keep track of new midpoint in _newVectices
        int newIndex = _vertices.Count;
        _newVectices.Add(t1,newIndex);

        // calculate new vertex
        _vertices.Add((_vertices[i1] + _vertices[i2]) * 0.5f); // vertice is a Vector3, calculate midpoint position
        _uvs.Add((_uvs[i1] + _uvs[i2]) * 0.5f);
        _normals.Add((_normals[i1] + _normals[i2]).normalized);

        return newIndex;
    }
}