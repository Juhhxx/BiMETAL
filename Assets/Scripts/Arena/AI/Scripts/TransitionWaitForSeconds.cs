using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "TransitionWaitForSeconds", menuName = "State Machines/Transitions/TransitionWaitForSeconds")]
public class TransitionWaitForSeconds : TransitionAbstract
{
    [SerializeField] private float _seconds;
    private float _timeElapsed = 0f;
    protected override void Action()
    {
        Debug.Log($"{_seconds} seconds passed!");
    }
    protected override bool Condition()
    {
        if (_timeElapsed <= _seconds)
        {
            _timeElapsed += Time.deltaTime;
            return false;
        }
        else
        {
            _timeElapsed = 0;
            return true;
        }
    }
    public override void InstantiateTransition()
    {
        base.transition = new Transition(base.Name, Condition, base.ToState.State, Action);
    }
}