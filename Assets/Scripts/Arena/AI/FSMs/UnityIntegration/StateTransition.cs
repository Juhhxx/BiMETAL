using System;
using System.Collections.Generic;
using NaughtyAttributes;

namespace AI.FSMs.UnityIntegration
{
    /// <summary>
    /// Struct for defining a State and it's Transitions.
    /// </summary>
    [Serializable]
    public struct StateTransition
    {
        [Expandable]
        public StateAbstract State;
        [Expandable]
        public List<TransitionAbstract> Transitions;

        public StateTransition(StateAbstract state, List<TransitionAbstract> trans)
        {
            State = state;
            Transitions = trans;
        }
    }
}