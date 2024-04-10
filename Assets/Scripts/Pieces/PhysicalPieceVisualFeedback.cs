using DG.Tweening;
using FinishOne.GeneralUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPieceVisualFeedback : PieceVisualFeedback
{
    Tween pickupTween;
    Tween dropTween;

    private Cell currentCell;

    private float defaultHeight;

    private const float LIFTED_HEIGHT = 0.3f;
    private const float LIFT_TIME = 0.25f;
    protected override void Start()
    {
        base.Start();
        defaultHeight = 0;
    }

    // try returning Random range surrounding LIFT_TIME
    private float GetLiftTime()
    {
        return LIFT_TIME;
    }

    public override void HandleNewCell(Cell cell)
    {
        if (currentCell == null)
        {
            transform.localPosition = cell.transform.position.NewY(0);
            currentCell = cell;
            return;
        }

        currentCell = cell;


        transform.position = cell.transform.position.NewY(defaultHeight + LIFTED_HEIGHT);
        if (pickupTween.IsActive())
        {
            pickupTween.Kill();
        }

        dropTween = transform.DOLocalMoveY(defaultHeight, GetLiftTime());
    }


    public override void HandlePickup()
    {
        if (dropTween.IsActive())
        {
            dropTween.Kill();
        }

        pickupTween = transform.DOLocalMoveY(defaultHeight + LIFTED_HEIGHT, GetLiftTime());
    }

    public override void HandleDropped(bool success)
    {
        // todo
    }

    public override void HandleHovered(bool hovered)
    {
        Color newColor = hovered ? piece.PieceColor.AtNewAlpha(0.75f) : piece.PieceColor;
        piece.SetColor(newColor);
    }

    public override void HandleIndicatorMoved(Cell newCell)
    {
        //maybe don't need
    }
}