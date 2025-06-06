using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharController))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Colliders Parameters")]
    [Space(5)]
    [SerializeField] private CollisionDetector  _attackCollider;
    [SerializeField] private Animator  _animator;

    [Space(10)]
    [Header("Attack Events")]
    [Space(5)]
    public UnityEvent OnSlash;

    // Game Object Components 
    private CharController  _character;

    private void Start()
    {
        _character = GetComponent<CharController>();
    }
   
    private void OnEnable()
    {
        if (_attackCollider != null) _attackCollider.OnCollisionEnter += DoAttack;
    }
    private void OnDisable()
    {
        if (_attackCollider != null) _attackCollider.OnCollisionEnter -= DoAttack;
    }
    public void Attack()
    {
        _animator.SetTrigger("Attack");
        OnSlash?.Invoke();
        // Debug.Log("BUMDA");
    }
    private void DoAttack(object sender, OnCollisionEventArgs e)
    {
        Collider other = e.other;
        CharController otherChar = other.gameObject.GetComponent<CharController>();

        if (otherChar != null)
        {
            _character.GiveDamage(otherChar, _character.Character.BaseAttack);
        }
    }
}
