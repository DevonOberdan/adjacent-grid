using UnityEngine;

public class PieceIndicator3DMatSwap : PieceIndicator
{
    [SerializeField] private Material activeMatPrefab;

    private bool initialized;
    private Material startMat;
    private Cell currentCell;
    private Material activeMat;

    private void Awake()
    {
        activeMat = Instantiate(activeMatPrefab);
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

    private void SetMaterial(Material mat)
    {
        if (currentCell == null)
            return;

        Renderer rend = currentCell.gameObject.GrabRenderer();
        if (rend != null && rend.materials.Length > 1)
        {
            rend.materials[1] = mat;
        }
    }

    public override void HandleValidPlacement(bool valid)
    {
        if(GrabCellMat(currentCell) != activeMat)
        {
            SetMaterial(activeMat);
        }

        SetColor(valid ? DefaultColor : GridInputHandler.Instance.InvalidPlacementColor);
    }

    public override void SetCell(Cell cell, bool displayNewCell = true)
    {
        Material mat = GrabCellMat(cell);

        if (!initialized && mat != null)
        {
            startMat = mat;
            initialized = true;
        }

        //set previous Cell's color back to normal
        SetMaterial(startMat);

        currentCell = cell;

        if (displayNewCell)
            SetColor(DefaultColor);
    }

    public override void SetColor(Color color)
    {
        if (currentCell == null || !currentCell.CanSetIndicatorColor)
            return;

        Material material = GrabCellMat(currentCell);

        if(color == DefaultColor)
        {
            SetMaterial(startMat);
        }
        else if (material != null)
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
        if (show)
        {
            activeMat.color = DefaultColor;
            SetMaterial(activeMat);
        }
        else
        {

            SetMaterial(startMat);
        }
    }
}