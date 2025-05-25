using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EnemyPool))]
public class ArenaController : MonoBehaviour
{
    [Header("Spawning Parameters")]
    [Space(5f)]
    [SerializeField] private EnemyPool  _enemyPool;
    public EnemyPool EnemyPool => _enemyPool;

    [Space(10f)]
    [Header("Testing Parameters")]
    [Space(5f)]
    [SerializeField] private int        _defaulfEnemyNumber;
    [SerializeField] private GameObject _gruntPrefab;
    [SerializeField] private GameObject _dummyPrefab;
    [SerializeField] private bool _doSpawn;
    [SerializeField] private bool _simulateModifier;
    public bool SimulateModifier => _simulateModifier;
    [ShowIf("SimulateModifier")][SerializeField] private ArenaModifierAbstract _arenaModifier;

    private TabletopController _tabletopController;
    private int _numberOfEnemies;
    private int _enemiesKilled;

    private void Start()
    {
        Debug.Log("ARENA CONTROLLER ACTIVE");
        _tabletopController = FindAnyObjectByType<TabletopController>();

        if (_tabletopController != null) _numberOfEnemies = _tabletopController.BattlePieces.Count - 1;
        else                             _numberOfEnemies = _defaulfEnemyNumber;

        if (_tabletopController != null)
        {
            _arenaModifier = _tabletopController.BattleCell.Modifier?.ArenaModifier;
            _arenaModifier?.ActivateModifier();
        }
        else if (_simulateModifier) _arenaModifier.ActivateModifier();

        if (_doSpawn) SpawnEnemies();
    }
    private void Update()
    {
        UpdateLockState();
        _arenaModifier?.UpdateModifier();
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
        if (_tabletopController == null)
        {
            for (int i = 0; i < _numberOfEnemies * 2; i++)
            {
                Vector3 pos = GetRandomLocation();
                pos.y = 1.0f;

                GameObject newEnemy;

                if (i < _numberOfEnemies)
                    newEnemy = _enemyPool.SpawnEnemy(_gruntPrefab, pos);
                else
                    newEnemy = _enemyPool.SpawnEnemy(_dummyPrefab, pos);

                CharController ctrl = newEnemy.GetComponent<CharController>();
                ctrl.OnDeath.AddListener(() => CheckEndBattle(true));

                Debug.Log($"ENEMY SPAWNED AT {pos}");
            }
        }
        else
        {
            foreach (Character c in _tabletopController.BattleCharacters)
            {
                if (c.Name == "Player") continue;
                
                Vector3 pos = GetRandomLocation();
                pos.y = 1.0f;

                GameObject prefab = c.CharacterPrefab;

                GameObject newEnemy = _enemyPool.SpawnEnemy(prefab, pos);

                SceneManager.MoveGameObjectToScene(newEnemy,
                SceneManager.GetSceneByName(_tabletopController.BATTLEARENA));

                CharController ctrl = newEnemy.GetComponent<CharController>();
                ctrl.OnDeath.AddListener(() => CheckEndBattle(true));

                Debug.Log($"ENEMY SPAWNED AT {pos}");
            }
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
            _tabletopController?.EndBattle(false);
        else
        {
            _enemiesKilled++;
            if (_enemiesKilled == _numberOfEnemies)
                _tabletopController?.EndBattle(true);
        }
    }
    public void WinBattle() => _tabletopController.EndBattle(true);
}
