using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MissilePool))]
public class MissileWeapon : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _target;
    [SerializeField] private float _spawnRate;
    private MissilePool _pool;
    private YieldInstruction _wfs;

    private void Start()
    {
        _pool = GetComponent<MissilePool>();
        _wfs = new WaitForSeconds(_spawnRate);

        StartCoroutine(ShootMissiles());
    }
    private IEnumerator ShootMissiles()
    {
        while (true)
        {
            MissileController ctrl = _pool.SpawnMissile(_spawnPoint.position,
                                                    _spawnPoint.rotation)
                                            .GetComponent<MissileController>();

            ctrl.SetTarget(_target);

            yield return _wfs;
        }
    }
}
