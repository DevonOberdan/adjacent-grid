using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceVisualFeedback : MonoBehaviour
{
    //private GridPiece piece;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    public virtual void HandleNewCell(Cell cell) => transform.position = cell.transform.position;

    public abstract void HandlePickup();
    public abstract void HandleDropped(Cell cell);
    public abstract void HandleHovered(bool hovered);
    public abstract void HandleIndicatorMoved(Cell newCell);
}
