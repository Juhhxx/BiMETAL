using System;
using System.Collections.Generic;

namespace Scripts.AI.FSMs.BaseFiles
{
    /// <summary>
    /// Class that defines a State to be used in a State Machine.
    /// </summary>
    public class State
    {
        // Name of the State.
        public string Name {get; private set; }

        // Actions to be executed in the beginning, during and end of this State.
        public Action EntryActions { get; private set; }
        public Action StateActions { get; private set; }
        public Action ExitActions { get; private set; }

        // List of all Transitions that come from this State. 
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

        /// <summary>
        /// Method for adding a Transition to the State.
        /// </summary>
        /// <param name="transition">Transition to be added.</param>
        public void AddTransition(Transition transition)
        {
            _transitions.Add(transition);
        }
    }
}
