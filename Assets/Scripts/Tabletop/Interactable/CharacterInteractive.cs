using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInteractive : Interactive
{
    private OverworldController _controller;
    [SerializeField] private string _levelSceneName;
    public string LevelName => _levelSceneName;
    [SerializeField] private GameObject _confirmScreen;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _notConfirmButton;
    [SerializeField] private List<DialogQueue> _dialogList;
    private Queue<Queue<(CharacterID, Queue<string>)>> _dialogQueues;
    private SpeechControl _speechControl;
    private bool _talking = false;

    [field:SerializeField] public bool Completed { get; private set; } = false;

    [SerializeField] private HexagonCell[] _unlockedRegionCells;


    public void SetCurrent(bool cur, Modifier unavailable)
    {
        // Debug.Log("level starting as " + cur);
        Completed = cur;
        if ( Completed ) // awake region
        {
            Debug.Log("next level On");
            StartCoroutine(AwakeCells());
        }
        else // turn off region
        {
            Debug.Log("next level Off");
            foreach ( HexagonCell cell in _unlockedRegionCells )
                cell.SleepRegion(unavailable);
        }
    }

    private IEnumerator AwakeCells()
    {
        YieldInstruction wfs = new WaitForSeconds(0.03f);

        _unlockedRegionCells = _unlockedRegionCells
            .OrderBy(t => t.GetDistance(Cell))
            .ToArray();

        foreach ( HexagonCell cell in _unlockedRegionCells )
        {
            cell.AwakeRegion();
            yield return wfs;
        }
    }


    private void Awake()
    {
        _controller = FindFirstObjectByType<OverworldController>();
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
    
    public override void Interact(Interactive other = null)
    {
        // Debug.Log("talking");
        if (_talking) return;

        _talking = true;
        
        Queue<(CharacterID, Queue<string>)> updatedQueue =
            _speechControl.ShowDialogs(_dialogQueues.Peek());

        if (updatedQueue != null)
        {
            _dialogQueues.Dequeue();
            _dialogQueues.Enqueue(updatedQueue);
        }

        StartCoroutine(WaitForSpeech());
    }

    private void Update()
    {
        if ( _talking && InputManager.Skip() )
            _speechControl.EndDialog();
    }

    public override void Select()
    {
        base.Select();
    }

    public IEnumerator WaitForSpeech()
    {
        yield return new WaitUntil( () => _speechControl.ShowingSpeech() );
        yield return new WaitWhile( () => _speechControl.ShowingSpeech() );

        Debug.Log("Showing confirmation menu. ");

        _talking = false;

        _confirmScreen.SetActive( true );

        _confirmButton.onClick.AddListener(StartLevel);
        _notConfirmButton.onClick.AddListener(Continue);
    }

    public void StartLevel()
    {
        Continue();
        _controller.StartLevel(_levelSceneName);
    }

    public void Continue()
    {
        _confirmButton.onClick.RemoveListener(StartLevel);
        _notConfirmButton.onClick.RemoveListener(Continue);

        _confirmScreen.SetActive( false );
    }

    /*public void NextSpeech()
    {
        if (_dialogQueues.Count > 1)
            _dialogQueues.Dequeue();
    }*/

    public override void AwakeRegion()
    {
        gameObject.SetActive(true);

        // give it a thump at awake, rigidbody should align it
        transform.Translate(Vector3.up * 0.5f);

        Debug.Log("level? modifying dialog queue");

        if (Completed)
            while (_dialogQueues.Count > 1)
                _dialogQueues.Dequeue();

        base.AwakeRegion();
    }

    public override void SleepRegion()
    {
        gameObject.SetActive(false);

        base.SleepRegion();
    }
}