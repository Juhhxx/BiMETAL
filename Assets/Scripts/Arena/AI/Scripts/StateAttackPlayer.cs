using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "StateAttackPlayer", menuName = "State Machines/StateAttackPlayer")]
public class StateAttackPlayer : StateAbstract
{
    private GameObject  gameObject;
    private Transform   transform;

    [SerializeField] private float _attackTimer;

    private float       _timer = 0;
    private EnemyAttack _enemyAttack;
    private Transform   _target;
    

    protected override void EntryAction()
    {
        Debug.Log("Entering State Attack");
    }

    protected override void StateAction()
    {
        Debug.Log("State Attack");

        transform.LookAt(_target);

        if (_timer < _attackTimer)
        {
            _timer += Time.deltaTime;
        }
        else if (_timer >= _attackTimer)
        {
            _enemyAttack.Attack();
            _timer = 0.0f;
        }
    }

    protected override void ExitAction()
    {
        Debug.Log("Exit State Attack");
    }
    public override void InstantiateState()
    {
        gameObject = base.objectReference;
        transform = gameObject.transform;
        _enemyAttack = GetComponent<EnemyAttack>(gameObject);
        _target = FindObjectByType<PlayerMovement>().transform;

        base.state = new State(base.Name, EntryAction, StateAction, ExitAction);
    }
}