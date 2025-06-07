using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private MeshDivider _meshDivider;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Transform _floor;
    [SerializeField] private PerlinGenerator[] _perlingGenerator;
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

        _meshFilter.mesh.vertices = verts;
    }
}
