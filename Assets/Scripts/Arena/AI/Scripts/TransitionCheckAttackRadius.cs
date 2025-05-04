using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "TransitionCheckAttackRadius", menuName = "State Machines/TransitionCheckAttackRadius")]
public class TransitionCheckAttackRadius : TransitionAbstract
{
    private GameObject  gameObject;
    private Transform   transform;

    [SerializeField] private float _radius;
    [SerializeField] private bool _checkIfInside;
    private Transform _target;

    protected override void Action()
    {
        Debug.Log($"Transition Attack Radius");
    }
    protected override bool Condition()
    {
        return (Vector3.Distance(transform.position, _target.position) < _radius) == _checkIfInside;
    }
    public override void InstantiateTransition()
    {
        gameObject = base.objectReference;
        transform = gameObject.transform;
        _target = FindAnyObjectByType<PlayerMovement>().transform;

        base.transition = new Transition(base.Name, Condition, base.ToState.State, Action);
    }
}