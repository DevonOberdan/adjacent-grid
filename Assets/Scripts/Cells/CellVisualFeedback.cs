using DG.Tweening;
using FinishOne.GeneralUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellVisualFeedback : MonoBehaviour
{
   // [SerializeField] private float bobDistance;
   // [SerializeField] private float bobTime;

    private float startHeight;

    private Tween bobTween;

    private GridLevelVisuals gridVisuals;

    private void Awake()
    {
        startHeight = transform.position.y;
        gridVisuals = GetComponentInParent<GridLevelVisuals>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void BobEffect(float factor)
    {
        if (bobTween.IsActive())
        {
            return;
        }

        //factor = Mathf.Clamp(factor, 0.4f,1);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(startHeight + (gridVisuals.BobHeight * factor), (gridVisuals.BobTime * factor) / 2)).SetEase(Ease.InOutSine);
       // seq.Append(transform.DOMoveY(startHeight - (gridVisuals.BobHeight * factor), (gridVisuals.BobTime * factor))).SetEase(Ease.InOutSine);
        seq.Append(transform.DOMoveY(startHeight, (gridVisuals.BobTime * factor) / 2)).SetEase(Ease.InOutSine);

        seq.SetLoops(gridVisuals.BobCount, LoopType.Restart);
                            
    }
}