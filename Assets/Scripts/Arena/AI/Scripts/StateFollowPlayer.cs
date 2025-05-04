using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "StateFollowPlayer", menuName = "State Machines/StateFollowPlayer")]
public class StateFollowPlayer : StateAbstract
{
    private GameObject      gameObject;
    private EnemyMovement    _movement;
    private Transform       _target;
    
    protected override void EntryAction()
    {
        Debug.Log("Entering Pursue");
        _movement.SetSpeed(_movement.MaxChaseSpeed);
        _movement.SetAcceptanceRadius(_movement.AcceptanceRadius);
    }

    protected override void StateAction()
    {
        Debug.Log("Pursuing");
        _movement.SetDestination(_target.position);
    }

    protected override void ExitAction()
    {
        Debug.Log("Exit Pursue");
    }
    public override void InstantiateState()
    {
        gameObject = base.objectReference;
        _movement = gameObject.GetComponent<EnemyMovement>();
        _target = FindAnyObjectByType<PlayerMovement>().transform;

        base.state = new State(base.Name, EntryAction, StateAction, ExitAction);
    }
}
