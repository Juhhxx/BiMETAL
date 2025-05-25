using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MissilePool))]
public class MissileWeapon : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    private Transform _target;
    private MissilePool _pool;
    private CharController _owner;

    private void Start()
    {
        _pool = GetComponent<MissilePool>();
        _owner = GetComponentInParent<CharController>();
        _target = FindAnyObjectByType<PlayerMovement>().transform;
    }

    public void Shoot()
    {
        MissileController ctrl = _pool.SpawnMissile(_spawnPoint.position,
                                        _spawnPoint.rotation)
                                        .GetComponent<MissileController>();

        ctrl.SetTarget(_target);
        ctrl.SetOwner(_owner);
    }
}
