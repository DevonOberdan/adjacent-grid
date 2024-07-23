using FinishOne.GeneralUtilities;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridPiece))]
public class ArrowPiece : PieceDestroyedFeedback
{
    [SerializeField] AutoPieceMover.DIR direction;

    private AutoPieceMover pieceMover;

    private const float MOVE_DELAY = 0.2f;

    public override void HandleDestroyed(GridPiece destroyer)
    {
        // move piece in the established direction, and its whole group too
        pieceMover.MovePieceAllTheWay(destroyer, direction);
        //StartCoroutine(MoveToEnd(destroyer));
    }

    private IEnumerator MoveToEnd(GridPiece piece)
    {
        piece.Grid.OnPiecePickedUp?.Invoke(piece);

        Cell nextCell = piece.CurrentCell.AdjacentCells[(int)direction];

        if(nextCell == null)
        {
            yield break;
        }

        piece.IndicatorCell = nextCell;
        piece.Grid.OnPieceIndicatorMoved?.Invoke(piece.IndicatorCell);
        piece.CanPlaceOnIndicator = true;

        yield return new WaitForSeconds(MOVE_DELAY);


        if (piece.UserDropPiece())
        {
            // let events run, make new CurrentGroups
            yield return new WaitForSeconds(MOVE_DELAY);
            yield return StartCoroutine(MoveToEnd(piece));
        }
    }

    private void Awake()
    {
        pieceMover = GetComponentInParent<AutoPieceMover>();
    }

    private void OnValidate()
    {
        transform.localEulerAngles = transform.localEulerAngles.NewY((int)direction * 90);
    }
}