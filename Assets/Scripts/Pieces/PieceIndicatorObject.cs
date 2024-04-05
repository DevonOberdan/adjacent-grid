using FinishOne.GeneralUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceIndicatorObject : PieceIndicator
{
    [SerializeField] private GameObject indicatorPrefab;

    private GameObject indicator;
    private new Renderer renderer;
    private Color indicatorColor;
    public Color IndicatorColor => indicatorColor;

    public override void HandleValidPlacement(bool valid)
    {
        renderer.SetColor(valid ? DefaultColor : GridInputHandler.Instance.InvalidPlacementColor);
    }

    public override void SetCell(Cell cell)
    {
        CurrentCell = cell;
        indicator.transform.position = cell.transform.position;
    }

    public override void SetColor(Color color)
    {
        renderer.SetColor(color);
    }

    public override void Setup(Color color)
    {
        indicator = Instantiate(indicatorPrefab, transform);
        renderer = indicator.GrabRenderer();
        indicator.transform.localPosition = Vector3.zero;

        DefaultColor = color.AtNewAlpha(renderer.GetColor().a);
        ShowIndicator(false);
    }

    public override void ShowIndicator(bool show)
    {
        indicator.SetActive(show);
    }
}
