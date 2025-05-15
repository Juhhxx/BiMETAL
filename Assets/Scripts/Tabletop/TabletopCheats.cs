using UnityEngine;

public class TabletopCheats : MonoBehaviour
{
    [SerializeField] private PlayerTabletopMovement _playerInput;
    [SerializeField] private TabletopController _controller;

    private void Start()
    {
        if ( _controller == null )
            _controller = FindFirstObjectByType<TabletopController>();
    }

    private void Update()
    {
        if ( Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.G) && _playerInput.Hovered != null )
        {
            if ( _playerInput.Hovered == null ) return;

            _playerInput.SetCurrent( _playerInput.Hovered );
        }


        if ( Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.UpArrow) && _playerInput.Hovered != null )
        {
            if ( _playerInput.Hovered == null ) return;
            if ( _playerInput.Hovered.Piece == null ) return;
            
            TabletopMovement move = _playerInput.Hovered.Piece.Base as TabletopMovement;
            if ( move != null )
                move.Points++;
        }

        if ( Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.DownArrow) && _playerInput.Hovered != null )
        {
            if ( _playerInput.Hovered == null ) return;
            if ( _playerInput.Hovered.Piece == null ) return;

            TabletopMovement move = _playerInput.Hovered.Piece.Base as TabletopMovement;

            if ( move != null && move.Points > 0 )
                move.Points--;
        }


        if ( Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.D) && _playerInput.Hovered != null )
        {
            if ( _playerInput.Hovered == null ) return;
            if ( _playerInput.Hovered.Piece == null ) return;

            PieceInteractive piece = _playerInput.Hovered.Piece as PieceInteractive;

            if ( piece != null && _playerInput.Hovered.Piece != _playerInput.Interactive )
                piece.Die( _playerInput.CurrentCell.CellValue );
            _controller.CheckGame();
        }
    }
}
