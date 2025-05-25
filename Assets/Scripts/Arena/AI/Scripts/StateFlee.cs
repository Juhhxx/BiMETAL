using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "StateFlee", menuName = "State Machines/States/StateFlee")]
public class StateFlee : StateAbstract
{
    GameObject gameObject;
    Transform transform;

    EnemyMovement _movement;
    Transform _target;

    protected override void EntryAction()
    {
        Debug.Log("Entering Flee");
    }

    protected override void StateAction()
    {
        Debug.Log("State Flee");

        Vector3 dir = _target.position - transform.position;

        dir = -dir.normalized;

        Vector3 destination = (dir * 8) + transform.position;

        _movement.SetDestination(destination);
    }

    protected override void ExitAction()
    {
        Debug.Log("Exit Flee");
    }
    public override void InstantiateState()
    {
        gameObject  = base.objectReference;
        transform   = gameObject.transform;
        _movement   = GetComponent<EnemyMovement>(gameObject);
        _target     = FindObjectByType<PlayerMovement>().transform;

        base.state = new State(base.Name, EntryAction, StateAction, ExitAction);
    }
}
