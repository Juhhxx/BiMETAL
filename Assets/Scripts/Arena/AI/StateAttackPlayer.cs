using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "StateAttackPlayer", menuName = "State Machines/StateAttackPlayer")]
public class StateAttackPlayer : StateAbstract
{
    private GameObject  gameObject;

    [SerializeField] private float _attackTimer;
    private float _timer = 0;
    private EnemyAttack _enemyAttack;
    

    protected override void EntryAction()
    {
        Debug.Log("Entering Empty");
    }

    protected override void StateAction()
    {
        Debug.Log("State Empty");

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
        Debug.Log("Exit Empty");
    }
    public override void InstantiateState()
    {
        gameObject = base.objectReference;
        _enemyAttack = gameObject.GetComponent<EnemyAttack>();

        base.state = new State(base.Name, EntryAction, StateAction, ExitAction);
    }
}