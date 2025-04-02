using System.Collections.Generic;
using UnityEngine;

public class TabletopController : MonoBehaviour
{
    [SerializeField] private PlayerTabletopMovement _playerInput;
    [SerializeField] private EnemyComposite _enemies;
    private int _round;
    private bool _playerRound;
    
    private void OnEnable()
    {
        _round = 0;

        if ( _playerInput == null )
            _playerInput = FindFirstObjectByType<PlayerTabletopMovement>();

        if ( _enemies == null )
            _enemies = FindFirstObjectByType<EnemyComposite>();
        
        if ( _playerInput == null || _enemies == null )
            return;

        _playerInput.PlayerTurn += ToggleRound;
        _enemies.EnemyTurn += ToggleRound;

        _playerInput.InputEnabled = true;

        _playerRound = true;
        ToggleRound();
    }

    public void ToggleRound()
    {
        // Debug.Log("Toggling round number " + _round + "  in player round: " + _playerRound);

        if ( !_playerRound )
            _enemies.StartMoving();
        
        _playerInput.InputEnabled = _playerRound;

        _round ++;
        _playerRound = ! _playerRound;
    }

    public void StartBattle(Modifier mod, List<PieceInteractive> pieces)
    {
        Debug.Log("Start battle between: " + pieces + " with mod: " + mod);

    }

    private void OnDisable()
    {
        if ( _playerInput == null || _enemies == null )
            return;

        _playerInput.PlayerTurn -= ToggleRound;
        _enemies.EnemyTurn -= ToggleRound;
    }
}
