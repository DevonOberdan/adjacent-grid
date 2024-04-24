using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceIndicator : MonoBehaviour
{
    public Color DefaultColor { get; protected set; }

    public Cell CurrentCell { get; set; }

    private Color currentColor;

    public Color CurrentColor 
    {
        get => currentColor;
        set 
        {
            currentColor = value;
        }
    }

    public abstract void Setup(Color color);

    public virtual void ResetIndicator()
    {
        SetColor(DefaultColor);
    }

    public abstract void SetCell(Cell cell);

    public abstract void SetColor(Color color);

    public abstract void HandleValidPlacement(bool valid);

    public abstract void ShowIndicator(bool show);
}
