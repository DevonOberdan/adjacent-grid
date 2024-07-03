using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using FinishOne.GeneralUtilities;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class GridPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public bool Interactable { get; private set; } = true;
    [field: SerializeField] public bool Consumable { get; private set; } = true;

    public UnityEvent<Cell> OnCellSet;

    public Action OnPickup, OnSelected;
    public Action<bool> OnDropSuccessful, OnHovered;
    public Action OnPieceMoved;
    public Action<Cell> OnIndicatorMoved;

    private PieceIndicator indicatorHandler;
    private PieceVisualFeedback visualFeedback;
    private PieceDestroyedFeedback destroyedFeedback;

    private GridManager grid;
    private Cell currentCell, indicatorCell, previouslyHoveredCell;

    private Renderer rend;
    private Collider pieceCollider;

    [SerializeField] private Color pieceColor;
    private bool canPlaceOnIndicator;

    #region Properties
    public bool IsHeld { get; private set; }
    public bool IsHovered { get; private set; }
    public Color PieceColor => pieceColor;

    public GridManager Grid => grid;

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

            //IndicatorCell = value;
           // ShowIndicator(currentCell != null && value != currentCell);

            currentCell = value;
            gameObject.SetActive(true);
            OnCellSet.Invoke(currentCell);

            IndicatorCell = currentCell;
           // CanPlaceOnIndicator = true;
            ShowIndicator(false);
            HandleNewCell();
        }
    }

    public Cell IndicatorCell
    {
        get => indicatorCell;
        set
        {
            indicatorCell = value;

            if(indicatorHandler != null)
                indicatorHandler.SetCell(indicatorCell);

            ShowIndicator(true);
        }
    }

    public void ResetIndicator()
    {
        indicatorCell = currentCell;

        if (indicatorHandler != null)
            indicatorHandler.SetCell(indicatorCell, false);

        CanPlaceOnIndicator = false;
        ShowIndicator(false);
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

        if(!TryGetComponent(out pieceCollider))
            pieceCollider = GetComponentInChildren<Collider>();

        rend = gameObject.GrabRenderer();
       // pieceColor = rend.GetColor();
        pieceCollider.enabled = Interactable;

        if(TryGetComponent(out indicatorHandler))
        {
            indicatorHandler.Setup(pieceColor);
        }

        visualFeedback = GetComponent<PieceVisualFeedback>();
        destroyedFeedback = GetComponent<PieceDestroyedFeedback>();

        OnHovered += HandleHover;
    }

    private void Update()
    {
        if (IsHeld)
        {
            HandleIndicator();
        }
    }
    #endregion

    public void Destroy(GridPiece destroyer = null)
    {
        if (grid == null)
            grid = transform.root.GetComponent<GridManager>();

        grid.RemovePiece(this);

        if (destroyedFeedback)
            destroyedFeedback.HandleDestroyed(destroyer);
    }

    public void InteractWithPiece(GridPiece otherPiece)
    {
        if (!otherPiece.IsOfSameType(this))
        {
            otherPiece.Destroy(this);
        }
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

    public bool IsOfSameType(GridPiece newPiece) => this.gameObject.name.Equals(newPiece.gameObject.name);

    public void SetColor(Color color) => pieceColor = color;

    #endregion

    #region Private Helper Methods
    private void HandleIndicator()
    {
        //Cell hoveredCell = grid.CurrentHoveredCell();
        Cell hoveredCell = grid.HoveredCell;

        if (ValidCell(hoveredCell) && hoveredCell != previouslyHoveredCell)
        {
            IndicatorCell = hoveredCell;
            OnIndicatorMoved?.Invoke(indicatorCell);

            if (visualFeedback != null)
                visualFeedback.HandleIndicatorMoved(IndicatorCell);

            if (hoveredCell == CurrentCell && !grid.PointerInGrid)
            {
                CanPlaceOnIndicator = false;
                ShowIndicator(true);
            }
        }

        previouslyHoveredCell = hoveredCell;
    }

    private bool ValidCell(Cell cell)
    {
        if (cell == null)
            return false;

        return cell == CurrentCell || CurrentCell.AdjacentCells.Contains(cell);
    }
    #endregion

    #region Indicator Methods
    public void HandlePickup()
    {
        IndicatorCell = CurrentCell;
        ShowIndicator(true);

        if (visualFeedback != null)
            visualFeedback.HandlePickup();

        CanPlaceOnIndicator = true;
    }

    public void HandleHover(bool hovered)
    {
        Color newColor = hovered ? PieceColor.AtNewAlpha(0.75f) : PieceColor;
        SetColor(newColor);

        if (visualFeedback != null)
            visualFeedback.HandleHovered(hovered);
    }

    public void PlaceOnIndicator()
    {
        IsHeld = false;

        if (CanPlaceOnIndicator)
        {
            CurrentCell = indicatorCell;
        }
        else
        {
            indicatorHandler.SetColor(indicatorHandler.DefaultColor);
        }

        if (visualFeedback != null)
            visualFeedback.HandleDropped(CurrentCell);

        //CanPlaceOnIndicator = true;
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
        IndicatorCell = CurrentCell;
        CanPlaceOnIndicator = false;
        //ShowIndicator(true);
    }
    #endregion

    public bool UserDropPiece()
    {
        if(CanPlaceOnIndicator && CurrentCell != indicatorCell)
            OnPieceMoved.Invoke();

        PlaceOnIndicator();
        OnDropSuccessful?.Invoke(CanPlaceOnIndicator);
        return CanPlaceOnIndicator;
    }

    public void PlaceOnCell(Cell cell)
    {
        IsHeld = false;

        CurrentCell = cell;

        indicatorHandler.SetColor(indicatorHandler.DefaultColor);

        if (visualFeedback != null)
            visualFeedback.HandleDropped(CurrentCell);

        ShowIndicator(false);
    }

    #region Input Callbacks
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!Interactable || !grid.Interactable) return;

        IsHeld = true;
        HandlePickup();
        OnPickup?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable || !grid.Interactable) return;

        UserDropPiece();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (grid.SelectedPiece != null || !Interactable || !grid.Interactable)
            return;

        IsHovered = true;
        OnHovered?.Invoke(IsHovered);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (grid.SelectedPiece != null || !Interactable || !grid.Interactable) 
            return;

        IsHovered = false;
        OnHovered?.Invoke(IsHovered);
    }
    #endregion

    private void OnDestroy()
    {
        grid.RemovePiece(this);
    }

    #region Editor Functions
    private void OnValidate()
    {
        HandleDroppedInPieces();
        //transform.localPosition = transform.localPosition.NewY(0);
    }

    /// <summary>
    /// Editor code that allows devs to drag prefabs into the Scene view and have them properly parented.
    /// </summary>
    private void HandleDroppedInPieces()
    {
#if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage() == null && !PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            if (transform.parent != null && transform.parent.CompareTag("GridPieceParent"))
                return;

            GameObject parent = GameObject.FindGameObjectWithTag("GridPieceParent");
            if (parent != null)
            {
                transform.parent = parent.transform;
            }
        }
#endif
    }
    #endregion
}