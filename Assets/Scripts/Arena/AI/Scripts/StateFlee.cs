using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "StateFlee", menuName = "State Machines/States/StateFlee")]
public class StateFlee : StateAbstract
{

    protected override void EntryAction()
    {
        Debug.Log("Entering Flee");
    }

    protected override void StateAction()
    {
        Debug.Log("State Flee");
    }

    protected override void ExitAction()
    {
        Debug.Log("Exit Flee");
    }
    public override void InstantiateState()
    {
        base.state = new State(base.Name, EntryAction, StateAction, ExitAction);
    }
}
