using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private MeshDivider _meshDivider;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Transform _floor;
    [SerializeField] private PerlinGenerator[] _perlingGenerator;
    [SerializeField] private NavMeshSurface _navmesh;

    [SerializeField, Range(0f, 1f)] private float _outsideFalloffStart;
    [SerializeField, Range(0f, 1f)] private float _outsideFalloffEnd;
    [SerializeField, Range(0f, 1f)] private float _outsideEuclideanAxial = 0.5f;

    [SerializeField, Range(0f, 1f)] private float _insideFalloffStart;
    [SerializeField, Range(0f, 1f)] private float _insideFalloffEnd;
    private void Awake()
    {
        if ( _meshDivider == null )
            _meshDivider = GetComponent<MeshDivider>();
    }

    private void Start()
    {
        if ( _navmesh == null )
            return;
        
        _meshDivider.Generate();
        Vector3[] verts = _meshFilter.mesh.vertices;
        Vector3[] norms = _meshFilter.mesh.normals;
        Vector2 offset = new Vector2 ( _floor.localScale.x, _floor.localScale.z);

        foreach (PerlinGenerator perlin in _perlingGenerator)
        {
            if ( perlin.Active )
                verts = perlin.Generate(verts, norms, offset);
        }

        verts = GenerateBorderFalloff(verts, norms);
        verts = GenerateBorderFalloff(verts, norms);
        _meshFilter.mesh.vertices = GenerateNavFalloff(verts, norms);
    }

    private Vector3[] GenerateBorderFalloff(Vector3[] vertices, Vector3[] normals)
    {
        float s = float.NegativeInfinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].z < 0.99f ) continue;
            float distance = new Vector2(vertices[i].x, vertices[i].z).magnitude;

            if (distance > s)
               s = distance;
        }

        float min = s * _outsideFalloffStart;
        float max = s * _outsideFalloffEnd;

        // float maxAxial = float.MinValue;
        // float maxEucl = float.MinValue;

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

            // if (distance > maxAxial)
            //    maxAxial = distance;

            float adjustedAxial = distance * 0.675f;

            // Lerp from 0 to 1 in the farthest outside fallout to nearest outside fallout
            float t1 = Mathf.InverseLerp(min, max, adjustedAxial);
            float axial = 1f - Mathf.SmoothStep(0f, 1f, t1);


            distance = new Vector2(vertices[i].x, vertices[i].z).magnitude;

            // if (distance > maxEucl)
            //    maxEucl = distance;

            float t2 = Mathf.InverseLerp(min, max, distance); // 0 inside,1 at edge
            float euclidean = 1f - Mathf.SmoothStep(0f, 1f, t2);

            vertices[i].y *= Mathf.Lerp(euclidean, axial, _outsideEuclideanAxial);
        }
        // Debug.Log("Max eucl: " + maxEucl + " max axial: " + maxAxial + " max: " + max);

        return vertices;
    }

    private Vector3[] GenerateNavFalloff(Vector3[] vertices, Vector3[] normals)
    {
        float s = float.NegativeInfinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].z < 0.99f ) continue;
            float distance = new Vector2(vertices[i].x, vertices[i].z).magnitude;

            if (distance > s)
               s = distance;
        }

        Vector3 worldScale = _floor.lossyScale;
        float scale = Mathf.Max(worldScale.x, worldScale.y, worldScale.z);

        float min = s * scale * _insideFalloffStart;
        float max = s * scale * _insideFalloffEnd;
        
        // create an array of nav mesh points (based on _insideFalloffStart)
        // for each vertices iterate to see which nav mesh border points are close enough
        // use euclidean distance to apply all their height corrections

        for (int i = 0; i < vertices.Length; i++)
        {
            if ( normals[i].y < 0.99f ) continue;

            Vector3 worldPos = _floor.TransformPoint(vertices[i]);
            worldPos.y = _navmesh.navMeshData.position.y;

            if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            {
                worldPos.y = hit.position.y;
                float distance = Vector3.Distance(worldPos, hit.position);

                float t = Mathf.InverseLerp(min, max, distance); // 0 inside,1 outside
                Debug.Log("init corrected pos: " + worldPos + " hit pos: " + hit.position + " distance: " + distance + " t(minmax): " + t + " i/x: " + min + " " + max + " result: " + (1f - Mathf.SmoothStep(1f, 0f, t)) );
                vertices[i].y *= 1f - Mathf.SmoothStep(1f, 0f, t);
            }
        }

        return vertices;
    }
}
