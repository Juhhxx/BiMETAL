using NaughtyAttributes;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    [SerializeField] private CollisionDetector _explosionCollider;
    [SerializeField] private Transform _target;
    [SerializeField] private float _linearVelocity;
    [SerializeField] private float _angularVelocity;
    [SerializeField] private float _targetAcceptanceRadius;
    [SerializeField][ReadOnly] private float _velocity;
    private Vector3 _destination;

    private MissilePool _pool;

    private void Start()
    {
        _velocity = _linearVelocity;
    }
    private void OnEnable()
    {
        _explosionCollider.OnCollisionEnter += Explode;
    }
    private void Update()
    {
        if (_target != null)
        {
            _destination = _target.position;
        }

        float dist = Vector3.Distance(transform.position, _destination);

        if (dist < _targetAcceptanceRadius)
        {
            _velocity = (dist / _targetAcceptanceRadius) * _linearVelocity;
        }
        else _velocity = _linearVelocity;

        Vector3 delta = _destination - transform.position;

        Vector3 forward = transform.forward;
        forward = Vector3.Slerp(forward, delta.normalized, _angularVelocity * Time.deltaTime);
        transform.forward = forward;

        Vector3 movement = transform.forward;
        movement *= _velocity;
        transform.position += movement * Time.deltaTime;
    }

    public void SetTarget(Transform target) => _target = target;
    private void Explode(object sender, OnCollisionEventArgs e)
    {
        Debug.Log($"Missile Detected Collision with {e.other.name}");
        Destroy();
    }

    public void SetPool(MissilePool pool) => _pool = pool;
    public void Destroy() => _pool?.DespawnMissile(gameObject);
}
