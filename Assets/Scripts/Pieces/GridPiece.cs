using UnityEngine;
using UnityEngine.EventSystems;
using FinishOne.GeneralUtilities;
using UnityEngine.Events;
using DG.Tweening;
using System;




#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class GridPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public bool Interactable { get; private set; } = true;

    [SerializeField] private UnityEvent<Cell> OnCellSet;

    private PieceIndicator indicatorHandler;
    private PieceVisualFeedback visualFeedback;

    private GridManager grid;
    private Cell currentCell, indicatorCell;

    private Renderer rend;
    private new Collider collider;

    private SpriteRenderer sprite;
    private Color pieceColor;
    private bool canPlaceOnIndicator;

   // private float defaultHeight;

    #region Properties

   // public float DefaultHeight => defaultHeight;

    public bool IsHeld { get; private set; }
    public Color PieceColor => pieceColor;

    public Action OnPickup;
    public Action<bool> OnDropped, OnHovered;
    public Action<Cell> OnIndicatorMoved;

    public Renderer GetRenderer()
    {
        if(rend == null)
            rend = gameObject.GrabRenderer();

        return rend;
    }

    public Cell CurrentCell
    {
        get => currentCell;
        set
        {
            if (currentCell != null)
                currentCell.RemovePiece(this);

            if (value.Occupied)
            {
                InteractWithPiece(value.CurrentPiece);
            }

            value.AddPiece(this);

            ShowIndicator(value != currentCell);
            currentCell = value;
            gameObject.SetActive(true);
            OnCellSet.Invoke(currentCell);

            HandleNewCell();
        }
    }

    public Cell IndicatorCell
    {
        get => indicatorCell;
        set
        {
            indicatorCell = value;

            indicatorHandler.SetCell(indicatorCell);
            ShowIndicator(true);
        }
    }

    public bool CanPlaceOnIndicator
    {
        get => canPlaceOnIndicator;
        set
        {
            canPlaceOnIndicator = value;
            indicatorHandler.HandleValidPlacement(canPlaceOnIndicator);
        }
    }

    #endregion

    #region Callbacks
    private void Awake()
    {
        grid = transform.root.GetComponent<GridManager>();

        collider = GetComponent<BoxCollider>();
        if(collider == null)
            collider = GetComponentInChildren<Collider>();

        rend = gameObject.GrabRenderer();

        pieceColor = rend.GetColor();

        collider.enabled = Interactable;

        if(TryGetComponent(out indicatorHandler))
        {
            indicatorHandler.Setup(pieceColor);
        }

        visualFeedback = GetComponent<PieceVisualFeedback>();

        OnHovered += HandleHover;
    }

    private void Start()
    {
       // defaultHeight = transform.localPosition.y;
    }

    private void Update()
    {
        if (IsHeld)
        {
            HandleIndicator();
        }
    }
    #endregion

    public void Destroy()
    {
        grid.RemovePiece(this);
    }

    public void InteractWithPiece(GridPiece otherPiece)
    {
        if (!otherPiece.IsOfSameType(this))
            otherPiece.Destroy();
    }

    #region Public Methods

    public void HandleNewCell()
    {
        visualFeedback = GetComponent<PieceVisualFeedback>();
        if (visualFeedback != null)
            visualFeedback.HandleNewCell(CurrentCell);
        else
            transform.localPosition = CurrentCell.transform.position;
    }

    public bool IsOfSameType(GridPiece newPiece) => this.PieceColor.Equals(newPiece.PieceColor);

    public void SetColor(Color color) => pieceColor = color;

    #endregion

    #region Private Helper Methods
    private void HandleIndicator()
    {
        //Cell hoveredCell = grid.CurrentHoveredCell();
        Cell hoveredCell = grid.HoveredCell;

        if (ValidCell(hoveredCell) && hoveredCell != IndicatorCell)
        {
            IndicatorCell = hoveredCell;
           //grid.OnPieceIndicatorMoved?.Invoke(indicatorCell);
            OnIndicatorMoved?.Invoke(indicatorCell);

            if(visualFeedback != null)
            {
                visualFeedback.HandleIndicatorMoved(IndicatorCell);
            }

            if (hoveredCell == CurrentCell && !grid.PointerInGrid)
            {
                CanPlaceOnIndicator = false;
                ShowIndicator(true);
            }
        }
    }

    private bool ValidCell(Cell cell)
    {
        if (cell == null)
            return false;

        return cell == CurrentCell || CurrentCell.AdjacentCells.Contains(cell);
    }
    #endregion

    #region Indicator Methods

    public void PickupVisual()
    {
        ShowIndicator(true);

        if(visualFeedback != null)
            visualFeedback.HandlePickup();
    }

    public void HandleHover(bool hover)
    {
        if (visualFeedback != null)
        {
            visualFeedback.HandleHovered(hover);
        }
    }

    public void PlaceOnIndicator()
    {
        IsHeld = false;

        if (CanPlaceOnIndicator)
            CurrentCell = indicatorCell;
        else
            indicatorHandler.SetColor(indicatorHandler.DefaultColor);

        if (visualFeedback != null)
        {
            visualFeedback.HandleDropped(CurrentCell);
        }

        IndicatorCell = CurrentCell;
        ShowIndicator(false);
    }

    public void ShowIndicator(bool show)
    {
        if(indicatorHandler != null)
        {
            indicatorHandler.ShowIndicator(show);
        }
    }

    public void MarkIndicatorCellInvalid()
    {
        CanPlaceOnIndicator = false;
        IndicatorCell = CurrentCell;
        ShowIndicator(true);
    }
    #endregion

    public bool UserDropPiece()
    {
        //bool dropped = CanPlaceOnIndicator;
        PlaceOnIndicator();
        //grid.OnPieceDropped?.Invoke(this, CanPlaceOnIndicator);
        OnDropped?.Invoke(CanPlaceOnIndicator);
        return CanPlaceOnIndicator;
    }

    #region Input Callbacks

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!Interactable) return;

        IsHeld = true;
        //grid.OnPiecePickedUp?.Invoke(this);
        OnPickup?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) return;

        UserDropPiece();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (grid.SelectedPiece != null || !Interactable)
            return;

        //grid.OnPieceHovered?.Invoke(this, true);
        OnHovered?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (grid.SelectedPiece != null || !Interactable) 
            return;

        //grid.OnPieceHovered?.Invoke(this, false);
        OnHovered?.Invoke(false);
    }
    #endregion

    #region Editor Functions
    private void OnValidate()
    {
        HandleDroppedInPieces();
    }

    /// <summary>
    /// Editor code that allows devs to drag prefabs into the Scene view and have them properly parented.
    /// </summary>
    private void HandleDroppedInPieces()
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