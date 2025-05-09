using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EnemyPool))]
public class ArenaController : MonoBehaviour
{
    [Header("Spawning Parameters")]
    [Space(5f)]
    [SerializeField] private int        _defaulfEnemyNumber;
    [SerializeField] private EnemyPool  _enemyPool;
    public EnemyPool EnemyPool => _enemyPool;
    [SerializeField] private GameObject _gruntPrefab;
    [SerializeField] private GameObject _dummyPrefab;

    private TabletopController _tabletopController;
    private int _numberOfEnemies;
    private int _enemiesKilled;

    private void Start()
    {
        Debug.Log("ARENA CONTROLLER ACTIVE");
        _tabletopController = FindAnyObjectByType<TabletopController>();

        if (_tabletopController != null) _numberOfEnemies = _tabletopController.BattlePieces.Count - 1;
        else                             _numberOfEnemies = _defaulfEnemyNumber;

        SpawnEnemies();
    }
    private void Update()
    {
        UpdateLockState();
    }

    // Update Lock State
    private void UpdateLockState()
    {
        if (InputManager.Paused) Cursor.lockState = CursorLockMode.None;
        else                     Cursor.lockState = CursorLockMode.Locked;
    }

    // Enemy Spawning
    private void SpawnEnemies()
    {
        // Change to a foreach, enemie piece data should store the respective game object for instantiation
        for (int i = 0; i < _numberOfEnemies*2; i++)
        {
            Vector3 pos = GetRandomLocation();
            pos.y = 1.0f;
            
            GameObject newEnemy;

            if (i < _numberOfEnemies)
                newEnemy = _enemyPool.SpawnEnemy(_gruntPrefab, pos);
            else
                newEnemy = _enemyPool.SpawnEnemy(_dummyPrefab, pos);

            if ( _tabletopController != null )
            {
                SceneManager.MoveGameObjectToScene(newEnemy,
                    SceneManager.GetSceneByName(_tabletopController.BATTLEARENA));
            }

            CharController ctrl = newEnemy.GetComponent<CharController>();
            ctrl.OnDeath.AddListener(() => CheckEndBattle(true));

            Debug.Log($"ENEMY SPAWNED AT {pos}");
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

    // Check Battle Status
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
