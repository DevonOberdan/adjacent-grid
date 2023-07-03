using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class GridPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GridManager grid;
    Cell currentCell;

    SpriteRenderer sprite;
    Color pieceColor;

    bool isHeld;
    bool canPlaceOnIndicator;

    SpriteRenderer indicator;
    Cell indicatorCell;
    Color indicatorColor;

    public Cell CurrentCell 
    {
        get => currentCell;
        set 
        {
            if (currentCell != null)
                currentCell.RemovePiece(this);

            // piece cannot be added to this cell
            if (!value.AddPiece(this))
                return;

            currentCell = value;

            gameObject.SetActive(true);

            // set position.. DOTween later
            transform.position = currentCell.transform.position;   
        }
    }

    public Cell IndicatorCell 
    {
        get => indicatorCell;
        set {
            indicatorCell = value;

            indicator.transform.position = indicatorCell.transform.position;

            if (indicatorCell != CurrentCell)
            {
                //indicator.transform.position = indicatorCell.transform.position;
                ShowIndicator(true);
            }
            else
            {
                ShowIndicator(false);
            }
        }
    }

    public Color IndicatorColor => indicatorColor;
    public Color PieceColor => pieceColor;

    public bool CanPlaceOnIndicator 
    {
        get => canPlaceOnIndicator;
        set 
        {
            canPlaceOnIndicator = value;
            indicator.color = canPlaceOnIndicator ? IndicatorColor : GridInputHandler.Instance.InvalidPlacementColor;
        }
    }

    public bool IsOfSameType(GridPiece newPiece) => this.PieceColor.Equals(newPiece.PieceColor);


    private void Awake()
    {
        grid = transform.root.GetComponent<GridManager>();
        sprite = GetComponent<SpriteRenderer>();
        pieceColor = sprite.color;
    }

    void Start()
    {
        SpawnIndicator();
    }

    void SetToGrid() => CurrentCell = grid.GetClosestCell(transform);
    
    public void PlaceOnIndicator()
    {
        isHeld = false;
        indicator.enabled = false;

        if (canPlaceOnIndicator)
        {
            CurrentCell = indicatorCell;
        }
        else
        {
            // indicator effect
            indicator.color = IndicatorColor;
        }

        IndicatorCell = CurrentCell;
        canPlaceOnIndicator = true;
    }

    void SpawnIndicator()
    {
        indicator = Instantiate(GridInputHandler.Instance.VisualIndicator, transform);
        indicator.transform.localPosition = Vector3.zero;
        indicatorColor = new Color(pieceColor.r, pieceColor.g, pieceColor.b, indicator.color.a);
        ShowIndicator(false);
    }

    public void ShowIndicator(bool show) => indicator.enabled = show;

    private void Update()
    {
        if (isHeld)
        {
            //FollowMouse();
            HandleIndicator();
        }
    }

    void FollowMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.position = Vector2.Lerp(transform.position, mouseWorldPos, GridInputHandler.Instance.DragCatchUpSpeed * Time.deltaTime);
    }

    void HandleIndicator()
    {
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        Cell hoveredCell = grid.HoveredOverCell;

        if(grid.HoveredOverCell == CurrentCell || CurrentCell.AdjacentCells.Contains(grid.HoveredOverCell))
        {
            IndicatorCell = hoveredCell;
            grid.OnPieceIndicatorMoved?.Invoke(indicatorCell);

            if (grid.HoveredOverCell == CurrentCell && !grid.PointerInGrid)
            {
                CanPlaceOnIndicator = false;
                ShowIndicator(true);
            }
        }
    }

    public void SetIndicatorCell(Cell newIndicatorCell) => indicatorCell = newIndicatorCell;

    public void SetHeld(bool held) => isHeld = held;

    public void SetColor(Color color) => pieceColor = color;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        grid.OnPiecePickedUp?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlaceOnIndicator();

        grid.OnPieceDropped?.Invoke(this);
    }

    #region Editor Functions
    private void OnValidate()
    {
        HandleDroppedInPieces();
    }

    void HandleDroppedInPieces()
    {
#if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage() == null && !PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            GameObject parent = GameObject.FindGameObjectWithTag("GridPieceParent");
            if (parent != null)
                transform.parent = parent.transform;
        }
#endif
    }
    #endregion
}