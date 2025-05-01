using TMPro;
using UnityEngine;

public class CellInfo : MonoBehaviour
{
    [SerializeField] private GameObject _cellGO;
    [SerializeField] private GameObject _pieceGO;
    [SerializeField] private TMP_Text _cellType;
    [SerializeField] private TMP_Text _cellWeight;
    [SerializeField] private TMP_Text _pieceType;
    [SerializeField] private TMP_Text _pieceRange;
    [SerializeField] private TMP_Text _pieceMod;

    public void Hover(HexagonCell cell = null)
    {
        if ( cell == null )
        {
            _cellGO.SetActive(false);
            _pieceGO.SetActive(false);

            _pieceRange.gameObject.SetActive(false);
            _pieceMod.gameObject.SetActive(false);

            return;
        }

        _cellGO.SetActive(true);

        _cellType.text = "Cell";
        if ( cell.Modifier != null )
            _cellType.text = cell.Modifier.name + " Modified " + _cellType.text;

        _cellWeight.text = "Movement Cost: " + cell.Weight + "  path stack: " + cell.PathStack;

        if ( cell.Piece != null )
        {
            _pieceGO.SetActive(true);

            _pieceType.text = cell.Piece.Name + " Piece";

            PieceInteractive piece = cell.Piece as PieceInteractive;
            if (piece != null)
            {
                if ( piece.HasModifier )
                {
                    _pieceMod.gameObject.SetActive(true);
                    _pieceRange.text = "Modifier: " + piece.Modifier.name;
                }
                _pieceRange.gameObject.SetActive(true);
                _pieceRange.text = "Movement Points: " + piece.Movement.Points;
            }
        }
    }

}
