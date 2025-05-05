using System;
using System.Collections.Generic;
using UnityEngine;
using AI.FSMs.BaseFiles;

namespace AI.FSMs.UnityIntegration
{
    /// <summary>
    /// Scriptable Object that creates State Machines that can be edited through the Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "StateMachine", menuName = "State Machines/StateMachine")]
    public class StateMachineCreator : ScriptableObject
    {
        // Reference to the State Machine that is created.
        private StateMachine _stateMachine = null;
        // Reference to the Game Object that the State Machine affects.
        private GameObject _objectReference;
        // Reference to the Initial State of the State Machine.
        private StateAbstract _initialState;
        
        // List of references to State Transitions.
        [Tooltip("List with all the States, and respective Transitions, for the State Machine.\nTip : The 1st State on the list will be the initial State.")]
        public List<StateTransition> StateTransitions;
        
        /// <summary>
        /// Method that instantiates all States and Transitions from the State Machine.
        /// </summary>
        private void InstantiateStates()
        {
            // Instantiate all States.
            foreach (StateTransition st in StateTransitions)
            {
                // Debug.Log($"Initializing State {st.State.name} {st.State.State == null}");
                st.State.SetObjectReference(_objectReference);
                st.State.SetParent(this);
                st.State.InstantiateState();
                // Debug.Log($"Initialized State {st.State.name} {st.State.State == null}");
            }
            // Instantiate all Transitions and add them to their respective States.
            foreach (StateTransition st in StateTransitions)
            {
                foreach (TransitionAbstract t in st.Transitions)
                {
                    t.SetObjectReference(_objectReference);
                    t.SetParent(this);
                    if (t.Transition == null) t.InstantiateTransition();
                    st.State.AddTransitions(t);
                }
            }
        }

        /// <summary>
        /// Method for defining what Game Object the State Machine will affect.
        /// </summary>
        /// <param name="go">The Game Object that will be affected.</param>
        public void SetObjectReference(GameObject go)
        {
            _objectReference = go;
        }
        /// <summary>
        /// method for instantiating the State Machine.
        /// </summary>
        public void InstantiateStateMachine()
        {
            InstantiateStates();
            _initialState = StateTransitions[0].State;
            _stateMachine = new StateMachine(_initialState.State);
        }
        /// <summary>
        /// Method for resetting the State Machine.
        /// </summary>
        public void ResetStateMachine()
        {
            _stateMachine.ResetStateMachine();
        }
        /// <summary>
        /// Method for running the State Machine.
        /// </summary>
        public void Run()
        {
            _stateMachine.Update()?.Invoke();
        }
        /// <summary>
        /// Method for creating a copy of the State Machine.
        /// </summary>
        /// <returns>A copy of the State Machine.</returns>
        public StateMachineCreator CreateStateMachine()
        {
            // Dictionary for storing the references to all objects that have been instantiated.
            Dictionary<string,ScriptableObject> instances = new Dictionary<string,ScriptableObject>();

            // New State Machine Asset.
            StateMachineCreator newSM = Instantiate(this);

            // Create a new List of StateTransitions.
            newSM.StateTransitions = new List<StateTransition>();

            // Setup the new List.
            foreach (StateTransition st in StateTransitions)
            {
                // Create new State and List of Transitions.
                StateAbstract state;
                List<TransitionAbstract> transitions = new List<TransitionAbstract>();

                // If the State has already been copied, use the reference stored in instances,
                // if not, create a copy of the requested State and store is in instances Dict.
                if (instances.ContainsKey(st.State.name))
                {
                    state = instances[st.State.name] as StateAbstract;
                }
                else
                {
                    state = st.State.CreateState();
                    instances.Add(st.State.name,state);
                }

                // Check all Transitions in the StateTransition
                foreach (TransitionAbstract t in st.Transitions)
                {
                    // If the Transition has already been copied, use the reference stored in instances,
                    // if not, create a copy of the requested Transition and store it in instances Dict.
                    if (instances.ContainsKey(t.name))
                    {
                        transitions.Add(instances[t.name] as TransitionAbstract);
                    }
                    else
                    {
                        TransitionAbstract transition = t.CreateTransition();
                        
                        // If the Transition ToState has already been copied, use the reference stored in instances,
                        // if not, create a copy of the requested State and store it in instances Dict.
                        if (instances.ContainsKey(t.ToState.name))
                        {
                            transition.ToState = instances[t.ToState.name] as StateAbstract;
                        }
                        else
                        {
                            instances.Add(t.ToState.name,t.ToState.CreateState());
                            transition.ToState = instances[t.ToState.name] as StateAbstract;
                        }

                        transitions.Add(transition);
                    }
                }

                // Create new StateTransition with the copied information amd Add it to the new List.
                StateTransition newST = new StateTransition(state,transitions);
                newSM.StateTransitions.Add(newST);
            }

            return newSM;
        }

        // New Implementations

        private Dictionary<string,Component> _components = new Dictionary<string, Component>();

        public T GetComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            string key = component.ToString();

            if (!_components.ContainsKey(key)) _components.Add(key, component);

            return _components[key] as T;
        }

        private Dictionary<string,GameObject> _objects = new Dictionary<string, GameObject>();

        public GameObject FindObjectByType<T>() where T : Component
        {
            GameObject obj = FindAnyObjectByType<T>().gameObject;

            string key = obj.name + "_" + obj.GetHashCode();

            if (!_objects.ContainsKey(key)) _objects.Add(key, obj);

            return _objects[key];
        }
    }
}

