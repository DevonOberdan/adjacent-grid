using FinishOne.GeneralUtilities;
using UnityEngine;

[RequireComponent(typeof(GridPiece))]
public class ArrowPiece : PieceDestroyedFeedback
{
    [SerializeField] AutoPieceMover.DIR direction;

    private AutoPieceMover pieceMover;

    private void Awake()
    {
        pieceMover = GetComponentInParent<AutoPieceMover>();
    }

    public override void HandleDestroyed(GridPiece destroyer)
    {
        pieceMover.MovePieceAllTheWay(destroyer, direction);
    }

    private void OnValidate()
    {
        transform.localEulerAngles = transform.localEulerAngles.NewY((int)direction * 90);
    }
}