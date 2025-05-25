using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class MissilePool : MonoBehaviour
{
    [Header("Pool Parameters")]
    [Space(5f)]
    [SerializeField] private float _defaultSpawnNumber;
    [SerializeField] private GameObject _missilePrefab;

    private Stack<GameObject> _missilePool;

    [Space(10f)]
    [Header("Debug Values")]
    [Space(5f)]
    [ReadOnly] public int PoolMaxSize;
    [ReadOnly] public int PoolCurrentSize;
    [ReadOnly][SerializeField] private List<GameObject> _missileList;
    public List<GameObject> EnemiesList => _missileList;
    
    private void Awake()
    {
        _missilePool = new Stack<GameObject>();
        _missileList = new List<GameObject>();

        CreatePools();
    }

    private GameObject CreateObject()
    {
        GameObject newObj       = Instantiate(_missilePrefab, Vector3.zero, Quaternion.identity);
        MissileController ctrl  = newObj.GetComponent<MissileController>();

        _missilePool.Push(newObj);
        _missileList.Add(newObj);
        ctrl?.SetPool(this);
        newObj.SetActive(false);

        PoolMaxSize++;
        PoolCurrentSize++;

        return newObj;
    }
    private void CreatePools()
    {
        for (int i = 0; i < _defaultSpawnNumber; i++)
        {
            CreateObject();
        }
    }

    public GameObject SpawnMissile(Vector3 position, Quaternion rotation)
    {
        GameObject spawn;

        spawn = (_missilePool.Count == 0)   ? CreateObject() 
                                            : _missilePool.Pop();

        spawn.transform.position = position;
        spawn.transform.rotation = rotation;

        spawn.SetActive(true);

        PoolCurrentSize--;

        Debug.LogWarning("SPAWN MISSILE");

        return spawn;
    }
    public void DespawnMissile(GameObject enemy)
    {
        if (_missilePool.Contains(enemy)) return;
        
        enemy.SetActive(false);
        _missilePool.Push(enemy);

        PoolCurrentSize++;
        Debug.LogWarning("DESPAWN MISSILE");
    }
}
