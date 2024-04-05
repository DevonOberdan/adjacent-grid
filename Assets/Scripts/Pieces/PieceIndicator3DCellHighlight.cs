using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceIndicator3DCellHighlight : PieceIndicator
{
    //[SerializeField] private Material indicatorMat;

    private Color startingIndicatorColor = Color.black;
    private Cell currentCell;

    private void Awake()
    {
        
    }

    private void ResetCurrentCell() 
    {
        if (currentCell == null) return;
        SetColor(startingIndicatorColor);
    }

    public override void HandleValidPlacement(bool valid)
    {
        SetColor(valid ? DefaultColor : GridInputHandler.Instance.InvalidPlacementColor);
    }

    public override void SetCell(Cell cell)
    {
        SetColor(startingIndicatorColor);
        currentCell = cell;
        SetColor(DefaultColor);
    }

    public override void SetColor(Color color)
    {
        if (currentCell == null) return;

        Renderer rend = currentCell.gameObject.GrabRenderer();
        if (rend != null && rend.materials.Length > 1)
        {
            rend.materials[1].color = color;
        }
    }

    public override void Setup(Color color)
    {
        DefaultColor = color;
    }

    public override void ShowIndicator(bool show)
    {
        SetColor(show ? DefaultColor : startingIndicatorColor);
    }
}
