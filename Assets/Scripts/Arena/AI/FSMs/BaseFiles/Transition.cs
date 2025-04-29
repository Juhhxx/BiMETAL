using System;

namespace Scripts.AI.FSMs.BaseFiles
{
    /// <summary>
    /// Class that defines a Transition to be used in a State Machine.
    /// </summary>
    public class Transition
    {
        // Name of the Transition.
        public string Name { get; private set; }

        // Actions to be executed when the Transition is Triggered.
        public Action Actions { get; private set; }
        // Reference to the State the Transition leads to.
        public State ToState {get; private set; }
        // Conditions for the Transition to check.
        private Func<bool> _conditions;

        public Transition(string name, Func<bool> conditions, State toState, Action actions)
        {
            Name = name;
            _conditions = conditions;
            ToState = toState;
            Actions = actions;
        }
        /// <summary>
        /// Method that checks if the transitions was triggered.
        /// </summary>
        /// <returns>If the transition was triggered.</returns>
        public bool IsTriggered()
        {
            return _conditions();
        }
    }
}
