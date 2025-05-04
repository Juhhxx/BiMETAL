using System.Collections.Generic;
using UnityEngine;

public class CharacterSpeech : MonoBehaviour
{
    [SerializeField] private List<DialogQueue> _dialogList;
    private Queue<Queue<(CharacterID, Queue<string>)>> _dialogQueues;
    private SpeechControl _speechControl;
    private bool _stop = false;
    
    private void Awake()
    {
        _speechControl = FindFirstObjectByType<SpeechControl>();

        if (_speechControl == null)
        {
            Debug.Log("Speech control not found in scene. ");
            return;
        }

        _dialogQueues = new Queue<Queue<(CharacterID, Queue<string>)>>();

        foreach (DialogQueue dialogQueue in _dialogList)
        {
            Queue<(CharacterID, Queue<string>)> innerQueue
                = new Queue<(CharacterID, Queue<string>)>();

            foreach (CharacterDialog characterDialog in dialogQueue.characterDialogs)
            {
                innerQueue.Enqueue((characterDialog.characterID,
                    new Queue<string>(characterDialog.dialogLines)));
            }

            _dialogQueues.Enqueue(innerQueue);
        }
    }

    public void StartSpeech()
    {
        // Debug.Log("talking");
        if (_stop) return;
        
        Queue<(CharacterID, Queue<string>)> updatedQueue =
            _speechControl.ShowDialogs(_dialogQueues.Peek());

        if (updatedQueue != null)
        {
            _dialogQueues.Dequeue();
            _dialogQueues.Enqueue(updatedQueue);
        }
    }
    public void NextSpeech()
    {
        if (_dialogQueues.Count > 1)
            _dialogQueues.Dequeue();
    }

    public void StopSpeech()
    {
        _stop = true;
    }
}
