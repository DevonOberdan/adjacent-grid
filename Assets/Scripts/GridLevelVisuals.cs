using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLevelVisuals : MonoBehaviour
{
    [SerializeField] private PiecePlacementRippleController ripplePrefab;

    [field: SerializeField] public float BobHeight { get; private set; }
    [field: SerializeField] public float BobTime { get; private set; }
    [field: SerializeField] public int BobCount { get; private set; }

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void HandlePieceRemoved(Cell location)
    {
        if(location.TryGetComponent(out CellVisualFeedback feedback))
        {
            feedback.BobEffect(1);
        }
        //PiecePlacementRippleController ripple = Instantiate(ripplePrefab, location.transform.position, Quaternion.identity);
    }
}