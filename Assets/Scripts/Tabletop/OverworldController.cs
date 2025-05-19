using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldController : Controller
{
    [SerializeField] private PlayerTabletopMovement _player;
    [SerializeField] private HexagonTabletop _tabletop;
    [SerializeField] private CharacterInteractive[] _levels;
    [SerializeField] private Modifier _unavailableMod;

    private string _levelName;

    private RestoreFlag _restoreFlag;

    private void Enable()
    {
        Debug.Log("starting");

        _player = FindFirstObjectByType<PlayerTabletopMovement>(FindObjectsInactive.Include);
        _tabletop = FindFirstObjectByType<HexagonTabletop>(FindObjectsInactive.Include);
        _levels = FindObjectsByType<CharacterInteractive>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (_player == null || _tabletop == null)
        {
            Debug.LogWarning("One or more core components not found.");
            return;
        }

        _player.PlayerTurn += TakeSnapshot;

        Debug.Log("started");
        _player.InputEnabled = true;
        enabled = true;
    }

    private void Start()
    {
        if (this == null) return;

        Enable();

        Debug.Log("Starting levels");

        foreach ( CharacterInteractive level in _levels )
        {
            level.SetCurrent( false, _unavailableMod );
        }
    }

    public void TakeSnapshot()
    {
        // save last infos before each turn

        // Debug.Log("Setting last");
        // _player.SetLast(); // dont need to set last because player doesnt need rollback in overworld
        // neither do levels cuz they get completed only at overworld load

        _tabletop.ResetPaths();
    }

    [field:SerializeField] public OverworldState Snapshot { get; private set; }

    public void StartLevel(string levelName)
    {
        Debug.Log("Starting battle");

        SaveSnapshot();

        SceneLoader.Load(levelName);

        Debug.Log("Setting? Player cells: "+ _player.CurrentCell + " l: " + _player.LastCell);

        enabled = false;
    }

    public void EndBattle(string levelName, bool playerWon)
    {
        _levelName = levelName;

        Debug.Log("Calling end level won? " + playerWon);

        _restoreFlag = new RestoreFlag();
        SceneLoader.Load("Overworld", _restoreFlag);

        StartCoroutine( RestoreAfterSceneLoad( playerWon ) );
    }

    private IEnumerator RestoreAfterSceneLoad( bool won)
    {
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Overworld").isLoaded);

        Enable();

        yield return new WaitUntil( () => enabled );

        yield return new WaitUntil( () => _tabletop.Done );

        Debug.Log("started restore");


        foreach (LevelState levelState in Snapshot.LevelStates)
        {
            foreach ( CharacterInteractive level in _levels )
            {
                if ( level.gameObject.name == levelState.LevelID )
                    level.SetCurrent( levelState.Completed, _unavailableMod);
            }
        }

        _player.SetCurrent( _tabletop.CellDict[ Snapshot.Player.CurrentCell ] );


        Debug.Log("start finished restore");

        _restoreFlag.IsRestored = true;


        // do this after complete load like in the die coroutine for pieces

        yield return new WaitUntil( () => ! SceneLoader.IsLoading );

        Debug.Log("setting level " + _levelName + " as " + won);

        foreach ( CharacterInteractive level in _levels )
        {
            if ( level.LevelName == _levelName )
                level.SetCurrent( won, _unavailableMod );
        }
    }

    public void SaveSnapshot()
    {
        Snapshot = new OverworldState();

        foreach ( CharacterInteractive level in _levels )
        {
            Snapshot.LevelStates.Add(new LevelState
            {
                LevelID = level.gameObject.name,
                Completed = level.Completed
            });
        }

        Snapshot.Player = new TabletopBaseState
        {
            PieceID = _player.gameObject.name,
            CurrentCell = _player.CurrentCell.CellValue,
            LastCell = _player.LastCell,
            Dead = ! _player.gameObject.activeSelf
        };
    }


    private void OnDisable()
    {
        if ( _player == null )
            return;

        _player.PlayerTurn -= TakeSnapshot;
        Debug.Log("Disabling OW controller");
    }
}


// player cur and last pos
// level completion (then I don't need to also save what regions to turn on/off)