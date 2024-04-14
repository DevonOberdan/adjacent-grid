using UnityEngine;

public abstract class PieceVisualFeedback : MonoBehaviour
{
    protected virtual void Awake()
    {

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
