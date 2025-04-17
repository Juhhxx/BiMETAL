using System.Collections.Generic;
using UnityEngine;

public class TabletopController : MonoBehaviour
{
    [SerializeField] private PlayerTabletopMovement _playerInput;
    [SerializeField] private EnemyComposite _enemies;
    [SerializeField] private int _baseHealth;
    private int _round;
    private int _health;
    private bool _playerRound;
    
    private void OnEnable()
    {
        _health = _baseHealth;
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

    private List<PieceInteractive> _battlePieces;
    public void StartBattle(Modifier mod, List<PieceInteractive> pieces)
    {
        _battlePieces = pieces;
        Debug.Log("Start battle between: " + pieces + " with mod: " + mod);
    }

    public void EndBattle(bool playerWon)
    {
        if ( ! playerWon )
        {
            _health --;
            // foreach
            // _battlePieces.ResetPlacements();
        }
        else
        {
            // foreach
            // not player
            // _battlePieces.Die();
        }

        // should the tabletop check if the player died or should the arena? Then, GameOver.

        Debug.Log("End battle between");
    }

    private void OnDisable()
    {
        if ( _playerInput == null || _enemies == null )
            return;

        _playerInput.PlayerTurn -= ToggleRound;
        _enemies.EnemyTurn -= ToggleRound;
    }
}
