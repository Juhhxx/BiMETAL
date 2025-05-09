using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;

public class EnemyMovement : MonoBehaviour
{
    [OnValueChanged("UpdateStats")] [SerializeField] private float _maxSpeed;
    [OnValueChanged("UpdateStats")] [SerializeField] private float _maxChaseSpeed;
    [OnValueChanged("UpdateStats")] [SerializeField] private float _maxAngularSpeed;
    [OnValueChanged("UpdateStats")] [SerializeField] private float _targetAcceptanceRadius;

    public float MaxSpeed => _maxSpeed;
    public float MaxChaseSpeed => _maxChaseSpeed;
    public float AcceptanceRadius => _targetAcceptanceRadius;

    private NavMeshAgent _agent;
    private EnemyPool _enemyPool;

    private void UpdateStats()
    {
        if(Application.isPlaying)
        {
            _agent.speed            = _maxSpeed;
            _agent.angularSpeed     = _maxAngularSpeed;
            _agent.stoppingDistance = _targetAcceptanceRadius;
        }
    }
    public void SetPool(EnemyPool pool) => _enemyPool = pool;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed            = _maxSpeed;
        _agent.angularSpeed     = _maxAngularSpeed;
        _agent.stoppingDistance = _targetAcceptanceRadius;
        _agent.updateRotation = true;
    }

    public void SetDestination(Vector3 target)
    {
        // _agent.updateRotation = (target - transform.position).normalized;
        _agent.destination = target;
    }
    public void SetSpeed(float speed) => _agent.speed = speed;
    public void SetAcceptanceRadius(float radius) => _agent.stoppingDistance = radius;
    public void DestroyEnemy()
    {
        _enemyPool.DespawnEnemy(gameObject);
    }
}
