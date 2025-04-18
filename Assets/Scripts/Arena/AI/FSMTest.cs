using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FSMTest : MonoBehaviour
{
    StateMachine FSM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        State A = new State("A",
                            () => Debug.Log("Entering State A"),
                            () => Debug.Log("Executing State A"),
                            () => Debug.Log("Exiting State A"));
        State B = new State("B",
                            () => Debug.Log("Entering State B"),
                            () => Debug.Log("Executing State B"),
                            () => Debug.Log("Exiting State B"));
        State C = new State("A",
                            () => Debug.Log("Entering State C"),
                            () => Debug.Log("Executing State C"),
                            () => Debug.Log("Exiting State C"));

        Transition clickSpace = new Transition( () => Input.GetKeyDown(KeyCode.Space),
                                                A,
                                                () => Debug.Log("Cliked Space"));
        Transition clickEnter = new Transition( () => Input.GetKeyDown(KeyCode.Return),
                                                B,
                                                () => Debug.Log("Cliked Enter"));
        Transition clickShift = new Transition( () => Input.GetKeyDown(KeyCode.LeftShift) | Input.GetKeyDown(KeyCode.RightShift),
                                                C,
                                                () => Debug.Log("Cliked Shift"));
        
        A.AddTransition(clickShift);
        A.AddTransition(clickEnter);

        B.AddTransition(clickSpace);
        B.AddTransition(clickShift);

        C.AddTransition(clickSpace);
        C.AddTransition(clickEnter);

        FSM = new StateMachine(A);
    }

    // Update is called once per frame
    void Update()
    {
        // FSM.Update()?.Invoke();
    }
    public void DebugPrint(string text) => Debug.Log(text);
}
