using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceIndicator3DCellHighlight : PieceIndicator
{
    private Color startingColor;
    private Cell currentCell;

    private bool startingColorSet;

    private void ResetCurrentCell() 
    {
        if (currentCell == null) return;
        SetColor(startingColor);
    }

    private Material GrabCellMat(Cell cell)
    {
        if (cell == null)
            return null;

        Renderer rend = cell.gameObject.GrabRenderer();
        if (rend != null && rend.materials.Length > 1)
        {
            return rend.materials[1];
        }

        return null;
    }

    public override void HandleValidPlacement(bool valid)
    {
        SetColor(valid ? DefaultColor : GridInputHandler.Instance.InvalidPlacementColor);
    }

    public override void SetCell(Cell cell, bool displayNewCell = true)
    {
        Material mat = GrabCellMat(cell);

        if (!startingColorSet && mat != null)
        {
            startingColor = mat.color;
            startingColorSet = true;
        }

        //set previous Cell's color back to normal
        SetColor(startingColor);

        currentCell = cell;

        if (displayNewCell)
            SetColor(DefaultColor);
    }

    public override void SetColor(Color color)
    {
        if (currentCell == null) return;

        Material material = GrabCellMat(currentCell);

        if(material != null)
        {
            material.color = color;
        }
    }

    public override void Setup(Color color)
    {
        DefaultColor = color;
    }

    public override void ShowIndicator(bool show)
    {
        SetColor(show ? DefaultColor : startingColor);
    }
}
