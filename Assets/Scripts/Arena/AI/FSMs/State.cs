using System;
using System.Collections.Generic;

[Serializable]
public class State
{
    public string Name {get; private set; }

    public Action EntryActions { get; private set; }
    public Action StateActions { get; private set; }
    public Action ExitActions { get; private set; }

    public IEnumerable<Transition> Transitions => _transitions;
    private IList<Transition> _transitions;

    public State(string name, Action entryActions, Action stateActions, Action exitActions)
    {
        Name = name;
        EntryActions    = entryActions;
        StateActions    = stateActions;
        ExitActions     = exitActions;
        _transitions    = new List<Transition>();
    }

    public void AddTransition(Transition transition)
    {
        _transitions.Add(transition);
    }
    
}
