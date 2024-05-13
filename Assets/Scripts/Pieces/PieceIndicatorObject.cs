using FinishOne.GeneralUtilities;
using UnityEngine;

public class PieceIndicatorObject : PieceIndicator
{
    [SerializeField] private GameObject indicatorPrefab;

    private GameObject indicator;
    private Renderer rend;
    private Color indicatorColor;
    public Color IndicatorColor => indicatorColor;

    public override void HandleValidPlacement(bool valid)
    {
        rend.SetColor(valid ? DefaultColor : GridInputHandler.Instance.InvalidPlacementColor);
    }

    public override void SetCell(Cell cell, bool resetOldCell = true)
    {
        CurrentCell = cell;
        indicator.transform.position = cell.transform.position;
    }

    public override void SetColor(Color color)
    {
        rend.SetColor(color);
    }

    public override void Setup(Color color)
    {
        indicator = Instantiate(indicatorPrefab, transform);
        rend = indicator.GrabRenderer();
        indicator.transform.localPosition = Vector3.zero;

        DefaultColor = color.AtNewAlpha(rend.GetColor().a);
        ShowIndicator(false);
    }

    public override void ShowIndicator(bool show)
    {
        indicator.SetActive(show);
    }
}
