using System;
using UnityEngine;
using Scripts.AI.FSMs.BaseFiles;

namespace Scripts.AI.FSMs.UnityIntegration
{
    /// <summary>
    /// Abstract Scriptable Object that defines everything a Transition needs.
    /// </summary>
    public abstract class TransitionAbstract : ScriptableObject
    {
        // Name for the Transition that is created.
        public string Name;
        // Reference for the Transition that is created.
        protected Transition transition = null;
        public Transition Transition => transition;
        // Reference to the State this Transition goes to.
        public StateAbstract ToState;
        // Reference to the Game Object this Transition affects.
        protected GameObject objectReference;

        /// <summary>
        /// Method for checking if the Transition was triggered.
        /// </summary>
        /// <returns>If the Transition has been triggered.</returns>
        public bool IsTriggered() => transition.IsTriggered();
        /// <summary>
        /// Method for defining what Game Object the Transition will affect.
        /// </summary>
        /// <param name="go">The Game Object that will be affected.</param>
        public void SetObjectReference(GameObject go) => objectReference = go;

        /// <summary>
        /// Abstract method for defining the Transitions Actions.
        /// </summary>
        protected abstract void Action();
        /// <summary>
        /// Abstract method for defining the Transitions conditions.
        /// </summary>
        /// <returns>If the conditions have been met.</returns>
        protected abstract bool Condition();
        /// <summary>
        /// Abstract method for instantiating a Transition.
        /// </summary>
        public abstract void InstantiateTransition();
        /// <summary>
        /// Method that returns a copy of the Transition.
        /// </summary>
        /// <returns>Copy of the Transition.</returns>
        public TransitionAbstract CreateTransition() => Instantiate(this);
    }
}
