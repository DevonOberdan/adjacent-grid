using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceVisualFeedback : MonoBehaviour
{
    protected GridPiece piece;

    protected virtual void Awake()
    {
        piece = GetComponent<GridPiece>();

        piece.OnPickup += HandlePickup;
        piece.OnDropSuccessful += HandleDropped;
        piece.OnHovered += HandleHovered;
        piece.OnIndicatorMoved += HandleIndicatorMoved;
        piece.OnCellSet.AddListener(HandleNewCell);

    }

    protected virtual void Start()
    {

    }


    public abstract void HandlePickup();
    public abstract void HandleDropped(bool success);
    public abstract void HandleHovered(bool hovered);
    public abstract void HandleIndicatorMoved(Cell newCell);
    public virtual void HandleNewCell(Cell cell) => transform.position = cell.transform.position;
}
