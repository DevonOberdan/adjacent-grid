using FinishOne.GeneralUtilities;
using UnityEngine;

[RequireComponent(typeof(GridPiece))]
public class ArrowPiece : PieceDestroyedFeedback
{
    [SerializeField] AutoPieceMover.DIR direction;

    private void OnValidate()
    {
        transform.localEulerAngles = transform.localEulerAngles.NewY((int)direction * 90);
    }
}