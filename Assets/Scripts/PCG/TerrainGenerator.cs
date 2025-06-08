using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private MeshDivider _meshDivider;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Transform _floor;
    [SerializeField] private PerlinGenerator[] _perlingGenerator;

    [SerializeField, Range(0f, 1f)] private float _outsideFalloffStart;
    [SerializeField, Range(0f, 1f)] private float _outsideFalloffEnd;

    [SerializeField, Range(0f, 1f)] private float _insideFalloffStart;
    [SerializeField, Range(0f, 1f)] private float _insideFalloffEnd;
    private void Awake()
    {
        if ( _meshDivider == null )
            _meshDivider = GetComponent<MeshDivider>();
    }

    private void Start()
    {
        _meshDivider.Generate();
        Vector3[] verts = _meshFilter.mesh.vertices;
        Vector3[] norms = _meshFilter.mesh.normals;
        Vector2 offset = new Vector2 ( _floor.localScale.x, _floor.localScale.z);

        foreach (PerlinGenerator perlin in _perlingGenerator)
        {
            if ( perlin.Active )
                verts = perlin.Generate(verts, norms, offset);
        }

        _meshFilter.mesh.vertices = GenerateFalloff(verts, norms);
    }

    private Vector3[] GenerateFalloff(Vector3[] vertices, Vector3[] normals)
    {
        float s = float.NegativeInfinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].z < 0.99f ) continue;
            if (vertices[i].z > s) s = vertices[i].z;
        }

        float min = s * _outsideFalloffStart;
        float max = s * _outsideFalloffEnd;

        Debug.Log("out min and max:  " + max + " " + min);

        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].y < 0.99f ) continue;

            Vector3 v = vertices[i];
            
            // Axial distance
            float q = (Mathf.Sqrt(3f)/3f * v.x - 1f/3f * v.z) / s;
            float r = 2f/3f * v.z / s;

            float cubeX = q;
            float cubeZ = r;
            float cubeY = -cubeX - cubeZ;

            float distance = Mathf.Max(
                Mathf.Abs(cubeX),
                Mathf.Abs(cubeY),
                Mathf.Abs(cubeZ)
            );

            // Lerp from 0 to 1 in the farthest outside fallout to nearest outside fallout
            float xlaw = distance - min;
            if ( xlaw >= 0 )
            {
                xlaw /= max;
                vertices[i].y *= 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(xlaw));
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].y < 0.99f ) continue;
        }

        return vertices;
    }
}
