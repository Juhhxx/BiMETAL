using System;
using UnityEngine;
using AI.FSMs.BaseFiles;

namespace AI.FSMs.UnityIntegration
{
    /// <summary>
    /// Abstract Scriptable Object that defines everything a State needs.
    /// </summary>
    public abstract class StateAbstract : ScriptableObject
    {
        // Name for the State that is created.
        public string Name;
        // Reference to the State that is created.
        protected State state = null;
        public State State => state;
        // Reference to the Game Object whom this State affects.
        protected GameObject objectReference;

        /// <summary>
        /// Method for adding Transitions to the State.
        /// </summary>
        /// <param name="transAbstract">Transition to be added to the State.</param>
        public void AddTransitions(TransitionAbstract transAbstract) => 
        state.AddTransition(transAbstract.Transition);
        /// <summary>
        /// Method for defining what Game Object the State will affect.
        /// </summary>
        /// <param name="go">The Game Object that will be affected.</param>
        public void SetObjectReference(GameObject go) => objectReference = go;

        /// <summary>
        /// Abstract method for defining the States Entry Actions.
        /// </summary>
        protected abstract void EntryAction();
        /// <summary>
        /// Abstract method for defining the States "Update" Actions.
        /// </summary>
        protected abstract void StateAction();
        /// <summary>
        /// Abstract method for defining the States Exit Actions.
        /// </summary>
        protected abstract void ExitAction();
        /// <summary>
        /// Abstract method for instantiating a State.
        /// </summary>
        public abstract void InstantiateState();
        /// <summary>
        /// Method that gives a copy of the State.
        /// </summary>
        /// <returns>Copy of the State.</returns>
        public StateAbstract CreateState() => Instantiate(this);
    }
}
 