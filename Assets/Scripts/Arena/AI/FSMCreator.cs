using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FSMCreator : MonoBehaviour
{
    private StateMachine FSM;
    [SerializeField] private StateCreator[] states;

    [Serializable]
    private class StateCreator
    {
        public string Name;
        public UnityEvent EntryActions;
        public UnityEvent StateActions;
        public UnityEvent ExitActions;
        public List<TransitionCreator> Transitions;
    }

    [Serializable]
    private class TransitionCreator
    {
        public Action Actions { get; private set; }
        public State ToState {get; private set; }
        private Func<bool> _conditions;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Action entry = null;

        // Type thisType = states[0]?.EntryActions?.GetPersistentTarget(0).GetType();
        // MethodInfo theMethod = thisType.GetMethod(states[0]?.EntryActions?.GetPersistentMethodName(0));
        // theMethod.Invoke(thisType, args);

        // entry += () => 

        // State a = new State("a", null, entry, null);

        // FSM = new StateMachine(a);
    }

    // Update is called once per frame
    void Update()
    {
        FSM.Update()?.Invoke();

    }
}
