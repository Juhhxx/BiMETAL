using System.Collections.Generic;
using UnityEngine;

public class TabletopController : MonoBehaviour
{
    [SerializeField] private PlayerTabletopMovement _playerInput;
    [SerializeField] private EnemyComposite _enemies;
    private int _round;
    private bool _playerRound;
    
    private void Start()
    {
        _round = 0;
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

    }
}
