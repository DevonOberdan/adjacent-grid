using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePlacementRippleController : MonoBehaviour
{
    private const int SIZE_CAP = 25;

    [SerializeField] private int growthTime = 1;

    private Tween scaleTween;

    private void Awake()
    {
        
    }

    private void Start()
    {
        Grow();
    }

    private void Update()
    {
        
    }

    private void ResetRipple()
    {
        transform.localScale = Vector3.one;
        scaleTween.Kill();
        Destroy(gameObject);
    }

    public void Grow()
    {
        transform.DOScale(SIZE_CAP, growthTime).OnComplete(() => ResetRipple());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CellVisualFeedback feedback))
        {
            Debug.Log("Feedback on Tile " +other.gameObject.name);
            feedback.BobEffect(1- transform.localScale.x/SIZE_CAP);
        }
    }
}