using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArenaController : MonoBehaviour
{
    [SerializeField] private int _defaulfEnemyNumber;
    [SerializeField] private GameObject _gruntPrefab;

    private TabletopController _tabletopController;
    private int _numberOfEnemies;
    private int _enemiesKilled;
    [SerializeField] private IList<GameObject> _enemiesList;

    private void Start()
    {
        Debug.Log("ARENA CONTROLLER ACTIVE - DESTROYED");
        _tabletopController = FindAnyObjectByType<TabletopController>();
        _enemiesList = new List<GameObject>();

        if (_tabletopController != null) _numberOfEnemies = _tabletopController.BattlePieces.Count - 1;
        else                             _numberOfEnemies = _defaulfEnemyNumber;

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        // Change to a foreach, enemie piece data should store the respective game object for instantiation
        for (int i = 0; i < _numberOfEnemies; i++)
        {
            Vector3 pos = GetRandomLocation();
            pos.y = 1.0f;

            GameObject newEnemy = Instantiate(_gruntPrefab, pos, Quaternion.identity);

            _enemiesList.Add(newEnemy);

            CharController ctrl = newEnemy.GetComponent<CharController>();
            ctrl.OnDeath.AddListener(() => CheckEndBattle(true));
        }
        _enemiesKilled = 0;
    }
    private Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        int t = Random.Range(0, navMeshData.indices.Length-3);
        
        // Select a random point on it
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t+1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t+2]], Random.value);

        return point;
    }
    public void CheckEndBattle(bool checkPlayerWin)
    {
        if (!checkPlayerWin)
            _tabletopController.EndBattle(false);
        else
        {
            _enemiesKilled++;
            if (_enemiesKilled == _numberOfEnemies)
                _tabletopController.EndBattle(true);
        }
    }
    public void WinBattle() => _tabletopController.EndBattle(true);
}
