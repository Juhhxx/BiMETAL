using System;
using System.Collections.Generic;

[Serializable]
public class Transition
{
    public Action Actions { get; private set; }
    public State ToState {get; private set; }
    private Func<bool> _conditions;

    public Transition( Func<bool> conditions, State toState, Action actions)
    {
        _conditions = conditions;
        ToState = toState;
        Actions = actions;
    }
    public bool IsTriggered()
    {
        return _conditions();
    }
}
