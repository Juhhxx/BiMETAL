using UnityEngine;
using System.Collections.Generic;
using System;
using NaughtyAttributes;

public class EnemyPool : MonoBehaviour
{
    [Header("Pool Parameters")]
    [Space(5f)]
    [SerializeField] private float _defaultSpawnNumber;
    [SerializeField] private List<GameObject> _enemyPrefabs;
    private Dictionary<string, Stack<GameObject>> _enemyPools;

    [Space(10f)]
    [Header("Debug Values")]
    [Space(5f)]
    [ReadOnly] public List<PoolDebugVisualizer> Pools = new List<PoolDebugVisualizer>();
    [ReadOnly][SerializeField] private List<GameObject> _enemiesList;
    public List<GameObject> EnemiesList => _enemiesList;
    
    [Serializable]
    public class PoolDebugVisualizer
    {
        public PoolDebugVisualizer(string name) => PoolName = name;

        public string PoolName;
        public int PoolMaxSize;
        public int PoolCurrentSize;
    }

    private void Awake()
    {
        _enemyPools = new Dictionary<string, Stack<GameObject>>();
        _enemiesList = new List<GameObject>();

        CreatePools();
    }

    private PoolDebugVisualizer GetPoolDebuger(string key)
    {
        foreach (PoolDebugVisualizer pd in Pools)
        {
            if (pd.PoolName == key) return pd;
        }
        return null;
    }
    private GameObject CreateObject(string poolKey, GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        EnemyMovement mov = newObj.GetComponent<EnemyMovement>();

        _enemyPools[poolKey].Push(newObj);
        _enemiesList.Add(newObj);
        mov?.SetPool(this);
        newObj.SetActive(false);

        PoolDebugVisualizer pd = GetPoolDebuger(poolKey);
        pd.PoolMaxSize++;
        pd.PoolCurrentSize++;

        return newObj;
    }
    private void CreatePools()
    {
        foreach (GameObject e in _enemyPrefabs)
        {
            string key = e.name;

            Debug.Log($"INITIALIZING POOL FOR {key}");

            if (!_enemyPools.ContainsKey(key))
            {
                Stack<GameObject> enemyPool = new Stack<GameObject>();
                _enemyPools.Add(key, enemyPool);
                Pools.Add(new PoolDebugVisualizer(key));
            }
            
            for (int i = 0; i < _defaultSpawnNumber; i++)
            {
                CreateObject(key, e);
            }
        }
    }

    public GameObject SpawnEnemy(GameObject type, Vector3 position)
    {
        string key = type.name;
        GameObject spawn;

        spawn = (_enemyPools[key].Count == 0)   ? CreateObject(key, type) 
                                                : _enemyPools[key].Pop();

        spawn.transform.position = position;
        spawn.SetActive(true);

        GetPoolDebuger(key).PoolCurrentSize--;

        return spawn;
    }
    public void DespawnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        _enemyPools[enemy.name].Push(enemy);

        GetPoolDebuger(enemy.name).PoolCurrentSize++;
    }
}
