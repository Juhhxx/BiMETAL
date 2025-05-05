using UnityEngine;
using NaughtyAttributes;

namespace AI.FSMs.UnityIntegration
{
    public class StateMachineRunner : MonoBehaviour
    {
        private bool InPlaymode => Application.isPlaying;
        [HideIf("InPlaymode")][Expandable][SerializeField] private StateMachineCreator _stateMachineModel;
        [ShowIf("InPlaymode")][Expandable][SerializeField] private StateMachineCreator _stateMachine;
        public StateMachineCreator StateMachine => _stateMachine;

        private void Start()
        {
            // Get copy of the State Machine.
            _stateMachine = _stateMachineModel.CreateStateMachine();
            
            // Set the Object Reference of the State Machine to this Game Object.
            _stateMachine.SetObjectReference(gameObject);
            // Instantiate the State Machine.
            _stateMachine.InstantiateStateMachine();
        }
        private void Update()
        {
            _stateMachine.Run();
        }
        public void Reset()
        {
            _stateMachine.ResetStateMachine();
        }
    }
}
