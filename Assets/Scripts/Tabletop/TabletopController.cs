using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TabletopController : Controller
{
    [SerializeField] private OverworldController _overworld;

    [SerializeField] private PlayerTabletopMovement _playerInput;
    [SerializeField] private EnemyComposite _enemies;
    [SerializeField] private HexagonTabletop _tabletop;
    [SerializeField] private int _baseHealth;
    [SerializeField] private Transform _healthParent;
    [SerializeField] private TMP_Text _roundText;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private GameObject _winOver;

    public string BATTLEARENA = "BattleArena";
    private int _round;
    private int _health;
    private bool _playerRound;

    private string _currentTabletop;

    [SerializeField] private TabletopBase[] _pieces;
    [SerializeField] private EnvironmentInteractive[] _mods;

    private RestoreFlag _restoreFlag;

    private void Enable()
    {
        Debug.Log("starting");

        if ( _canvas != null )
        _canvas.SetActive(true);

        _playerInput = FindFirstObjectByType<PlayerTabletopMovement>(FindObjectsInactive.Include);
        _enemies = FindFirstObjectByType<EnemyComposite>(FindObjectsInactive.Include);
        _tabletop = FindFirstObjectByType<HexagonTabletop>(FindObjectsInactive.Include);

        if (_playerInput == null || _enemies == null || _tabletop == null)
        {
            Debug.LogWarning("One or more core components not found.");
            return;
        }

        List<TabletopBase> bases = FindObjectsByType<EnemyTabletopMovement>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Cast<TabletopBase>()
            .ToList();
        bases.Add(_playerInput);
        _pieces = bases.ToArray();


        _mods = FindObjectsByType<EnvironmentInteractive>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        _playerInput.PlayerTurn += ToggleRound;
        _enemies.EnemyTurn += ToggleRound;

        Debug.Log("started");

        _playerInput.InputEnabled = true;

        enabled = true;
    }

    private void Start()
    {
        if (this == null) return;

        Enable();

        _gameOver.SetActive(false);
        _winOver.SetActive(false);

        _currentTabletop = SceneLoader.SceneToLoad;

        _overworld = FindFirstObjectByType<OverworldController>();

        Debug.Log("tabletop to load is: " + _currentTabletop);

        _health = _baseHealth;
        _round = 0;

        for ( int i = 1; i < _baseHealth; i++)
        {
            Instantiate(_healthParent.GetChild(0), _healthParent);
        }

        StartNewTabletop();
    }

    public void CheckGame()
    {
        StartCoroutine(CheckForGame());
    }

    private IEnumerator CheckForGame()
    {
        Debug.Log("commence");
        yield return null;

        if ( _health <= 0 )
        {
            yield return new WaitForSeconds(3f);
            _gameOver.SetActive(true);
        }
        else if ( _pieces.Where( p => p != _playerInput ).All( p => p.Interactive is PieceInteractive { Dead: true }) )
        {
            yield return new WaitForSeconds(3f);
            _winOver.SetActive(true);
        }

        StartNewTabletop();
    }

    public void Retry()
    {
        SceneLoader.Load(_currentTabletop);
        Destroy(gameObject);
    }

    public void OverWorld()
    {
        if ( _overworld != null )
            _overworld.EndBattle(_currentTabletop, _health > 0 );
        else
            SceneLoader.Load("Overworld");

        // SceneLoader.Load("Overworld");
        Destroy(gameObject);
    }

    private void StartNewTabletop()
    {
        _playerRound = true;
        ToggleRound();
    }

    public void ToggleRound()
    {
        // Debug.Log("Toggling round number " + _round + "  in player round: " + _playerRound);

        // save last infos before each turn

        if ( !_playerRound )
        {
            _enemies.StartMoving();
        }
        else // Only calls this if player did not enter battle, so it saves from the player's last step
        {
            Debug.Log("Setting last");
            foreach ( HexagonCell cell in _tabletop.Cells )
                cell.SetLast();
            foreach ( TabletopBase piece in _pieces )
                piece.SetLast();
            foreach ( EnvironmentInteractive mod in _mods )
                mod.SetLast();
        }
        
        _playerInput.InputEnabled = _playerRound;

        _tabletop.ResetPaths();

        _round ++;
        _roundText.text = _round.ToString();
        _playerRound = ! _playerRound;
    }

    private List<PieceInteractive> _battlePieces;

    public List<Character> BattleCharacters { get; private set; }
    public List<PieceInteractive> BattlePieces {
        get {
            Debug.Log("DESTROYED? Battle pieces num: " + _battlePieces.Count);
            return _battlePieces;
        }
        }
    public List<string> BattleIDs { get; private set; }
    public HexagonCell BattleCell { get; private set; }

    /// <summary>
    /// Starts an arena battle and saves information to be reloaded when it comes back
    /// </summary>
    /// <param name="mod"> The modifier to modify the next battle's arena </param>
    /// <param name="pieces"> The pieces that are going into battle </param>
    [field:SerializeField] public BattleState Snapshot { get; private set; }

    public void StartBattle(HexagonCell cell, List<PieceInteractive> pieces)
    {
        _battlePieces = pieces;
        BattleIDs = new();
        BattleCharacters = new();

        foreach ( PieceInteractive piece in BattlePieces)
            BattleIDs.Add(piece.gameObject.name);
        
        foreach ( PieceInteractive piece in BattlePieces)
            BattleCharacters.Add(piece.Character);

        BattleCell = cell;

        Debug.Log("Starting battle");

        SaveSnapshot();

        if ( _currentTabletop == null || _currentTabletop == "" )
        {
            _currentTabletop = SceneManager.GetActiveScene().name;
            Debug.Log("detected editor time tabletop load, new name: " + _currentTabletop);
        }

        SceneLoader.Load(BATTLEARENA);

        Debug.Log("Setting? Player cells: "+ _playerInput.CurrentCell + " l: " + _playerInput.LastCell);

        enabled = false;
    }

    public void EndBattle(bool playerWon)
    {
        Debug.Log("Calling end battle. ");

        _restoreFlag = new RestoreFlag();
        SceneLoader.Load(_currentTabletop, _restoreFlag);

        StartCoroutine( RestoreAfterSceneLoad( playerWon ) );
    }

    /// <summary>
    /// Restores data from the scriptable called Snapshot ( give snapshot save system values and then call this on enable )
    /// Restore additional tabletop controller data only after running this
    /// Keep in mind you need to wait for tabletop controller Start to end to run this, plus unity loading is async, use a Restire flag to wait until the correct scene is loaded and then use it to make the scene loader continue on saying "loading" to make teh save system wait until everything is finished, if you didnt get the last line, then welp
    /// </summary>
    /// <param name="won"> If it won a battle
    /// ( determines if its supposed to roll back or not, for save system say true,
    /// tabletop pieces will not be killed, since the variable is null on game load )</param>
    private IEnumerator RestoreAfterSceneLoad(bool won)
    {
        yield return new WaitUntil(() => SceneManager.GetSceneByName(_currentTabletop).isLoaded);

        Enable();

        yield return new WaitUntil( () => enabled );

        yield return new WaitUntil( () => _tabletop.Done );

        Debug.Log("started restore");

        if ( ! won )
        {
            _healthParent.GetChild(_health-1).gameObject.SetActive(false);
            _health--;

            RestoreSnapshot( false );

            PieceInteractive piece = _playerInput.Interactive as PieceInteractive;
            if ( piece != null )
            {
                piece.Hurt();
            }
        }
        else
        {
            RestoreSnapshot( true );

            foreach ( PieceInteractive piece in BattlePieces )
            {
                if ( piece.IsEnemy )
                    piece.Die(BattleCell.CellValue);
            }
        }

        StartCoroutine(CheckForGame());

        Debug.Log("start finished restore");

        _restoreFlag.IsRestored = true;
    }

    /// <summary>
    /// Here values are saved, to be used by the save system and persistence, the scriptable Snapshot
    /// It's called every time a battle starts for now, should be called before save system saves data from the scriptable Snapshot as well
    /// The save system should additionally save controller values like health and round number
    /// </summary>
    public void SaveSnapshot()
    {
        Snapshot = new BattleState();

        foreach ( HexagonCell cell in _tabletop.Cells)
        {
            Snapshot.CellStates.Add(new HexagonCellState
            {
                Position = cell.CellValue,
                CurrentMod = cell.EnvironmentMod,
                LastMod = cell.LastMod
            });
        }

        Debug.Log("pieces null?" + _pieces);

        foreach ( TabletopBase piece in _pieces)
        {
            Snapshot.PieceStates.Add(new TabletopBaseState
            {
                PieceID = piece.gameObject.name,
                CurrentCell = piece.CurrentCell.CellValue,
                LastCell = piece.LastCell,
                Dead = ! piece.gameObject.activeSelf
            });
        }

        foreach (EnvironmentInteractive mod in _mods)
        {
            Snapshot.ModifierStates.Add(new ModifierState
            {
                ModID = mod.gameObject.name,
                CurrentModified = mod.Modified,
                LastModified = mod.LastModified
            });
        }
    }

    private void RestoreSnapshot( bool current )
    {
        if ( current )
        {
            Debug.Log("Setting reload as current values");
            foreach (HexagonCellState cellState in Snapshot.CellStates)
            {
                HexagonCell cell = _tabletop.CellDict[cellState.Position];
                cell.SetMod(cellState.CurrentMod);
            }

            foreach (TabletopBaseState pieceState in Snapshot.PieceStates)
            {
                foreach ( TabletopBase p in _pieces )
                {
                    if ( p.gameObject.name != pieceState.PieceID ) continue;

                    if ( pieceState.Dead )
                    {
                        p.gameObject.SetActive(false);
                        continue;
                    }

                    p.SetCurrent( _tabletop.CellDict[pieceState.CurrentCell] );
                }
            }

            foreach (ModifierState modState in Snapshot.ModifierStates)
            {
                foreach ( EnvironmentInteractive m in _mods )
                {
                    if ( m.gameObject.name != modState.ModID ) continue;

                    m.SetModified(modState.CurrentModified);
                }
            }
        }
        else
        {
            Debug.Log("Setting reload as last values");

            foreach (HexagonCellState cellState in Snapshot.CellStates)
            {
                HexagonCell cell = _tabletop.CellDict[cellState.Position];
                cell.SetMod(cellState.LastMod);
            }

            foreach (TabletopBaseState pieceState in Snapshot.PieceStates)
            {
                foreach ( TabletopBase p in _pieces )
                {
                    if ( p.gameObject.name != pieceState.PieceID ) continue;

                    if ( pieceState.Dead )
                    {
                        p.gameObject.SetActive(false);
                        continue;
                    }

                    p.SetCurrent( _tabletop.CellDict[pieceState.LastCell] );
                }
            }

            foreach (ModifierState modState in Snapshot.ModifierStates)
            {
                foreach ( EnvironmentInteractive m in _mods )
                {
                    if ( m.gameObject.name != modState.ModID ) continue;

                    m.SetModified(modState.LastModified);
                }
            }
        }

        _battlePieces = new List<PieceInteractive>();

        foreach ( string id in BattleIDs )
        {
            foreach (TabletopBase basePiece in _pieces)
            {
                if ( basePiece.gameObject.name == id )
                {
                    PieceInteractive piece = basePiece.Interactive as PieceInteractive;
                    if ( piece != null )
                        _battlePieces.Add( piece );

                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        _canvas.SetActive(false);

        if ( _playerInput == null || _enemies == null )
            return;

        Debug.Log("Disabling controller");

        _playerInput.PlayerTurn -= ToggleRound;
        _enemies.EnemyTurn -= ToggleRound;
    }
}
