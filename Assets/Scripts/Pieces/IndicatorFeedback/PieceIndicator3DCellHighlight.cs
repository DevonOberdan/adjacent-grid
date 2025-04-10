using UnityEngine;

public class PieceIndicator3DCellHighlight : PieceIndicator
{
    private Color startingColor;
    private Cell currentCell;

    private bool initialized;

    public Color StartingColor => startingColor;

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

        if (!initialized && mat != null)
        {
            startingColor = mat.color;
            initialized = true;
        }

        //set previous Cell's color back to normal
        SetColor(startingColor);

        currentCell = cell;

        if (displayNewCell)
            SetColor(DefaultColor);
    }

    public override void SetColor(Color color)
    {
        if (currentCell == null || !currentCell.CanSetIndicatorColor)
            return;

        Material material = GrabCellMat(currentCell);

        if(material != null)
        {
            material.color = color;

            if (material.HasFloat("_TimeOffset"))
                material.SetFloat("_TimeOffset", Time.time);
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