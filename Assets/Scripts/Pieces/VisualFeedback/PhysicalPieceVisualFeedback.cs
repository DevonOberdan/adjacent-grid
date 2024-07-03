using DG.Tweening;
using FinishOne.GeneralUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPieceVisualFeedback : PieceVisualFeedback
{
    [SerializeField] private bool doNotTween;

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

    private void OnDestroy()
    {
        if (pickupTween.IsActive())
            pickupTween.Kill();

        if (dropTween.IsActive())
            dropTween.Kill();
    }

    // TODO: try returning Random range surrounding LIFT_TIME
    private float GetLiftTime()
    {
        return LIFT_TIME;
    }

    private void LowerPiece()
    {
        // ensure this never runs from editor-time code
        if (!Application.isPlaying)
            return;

        if (pickupTween.IsActive())
        {
            pickupTween.Kill();
        }

        dropTween = transform.DOLocalMoveY(defaultHeight, GetLiftTime());
        dropTween.ForceInit();
    }

    public override void HandleNewCell(Cell cell)
    {
        if (currentCell == null || doNotTween)
        {
            transform.localPosition = cell.transform.position.NewY(defaultHeight);
            currentCell = cell;
            return;
        }

        currentCell = cell;


        transform.position = cell.transform.position.NewY(defaultHeight + LIFTED_HEIGHT);
        LowerPiece();
    }

    public override void HandlePickup()
    {
        if (dropTween.IsActive())
        {
            dropTween.Kill();
        }

        pickupTween = transform.DOLocalMoveY(defaultHeight + LIFTED_HEIGHT, GetLiftTime());
        pickupTween.ForceInit();
    }

    public override void HandleDropped(bool success)
    {
        LowerPiece();
    }

    public override void HandleHovered(bool hovered)
    {
        
    }

    public override void HandleIndicatorMoved(Cell newCell)
    {
        //maybe don't need
    }
}