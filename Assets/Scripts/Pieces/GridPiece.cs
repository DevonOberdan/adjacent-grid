using UnityEngine;
using UnityEngine.EventSystems;
using FinishOne.GeneralUtilities;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class GridPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public bool Interactable { get; private set; } = true;

    private PieceIndicator indicatorHandler;

    private GridManager grid;
    private Cell currentCell, indicatorCell;

    private Renderer rend;
    private new Collider collider;

    private SpriteRenderer sprite;
    private Renderer indicator;
    private Color pieceColor;
    private bool canPlaceOnIndicator;

    #region Properties

    public bool IsHeld { get; private set; }
   // public Color IndicatorColor => indicatorColor;
    public Color PieceColor => pieceColor;

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
        }
    }

    public Cell IndicatorCell
    {
        get => indicatorCell;
        set
        {
            indicatorCell = value;

            indicatorHandler.SetCell(indicatorCell);
            //indicator.transform.position = indicatorCell.transform.position;
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
            //indicator.SetColor(canPlaceOnIndicator ? IndicatorColor : GridInputHandler.Instance.InvalidPlacementColor);
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

        indicatorHandler = GetComponent<PieceIndicator>();
        indicatorHandler.Setup(pieceColor);
    }

    private void Start()
    {
        //SpawnIndicator();

        //indicatorHandler.Setup(pieceColor);
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

    public void Highlight(bool highlight) => sprite.color = highlight ? indicatorHandler.DefaultColor : pieceColor;
    public bool IsOfSameType(GridPiece newPiece) => this.PieceColor.Equals(newPiece.PieceColor);

    public void SetColor(Color color) => pieceColor = color;

    #endregion

    #region Private Helper Methods

    //private void SpawnIndicator()
    //{
    //    indicator = Instantiate(GridInputHandler.Instance.VisualIndicator, transform).GrabRenderer();
    //    indicator.transform.localPosition = Vector3.zero;

    //    indicatorColor = pieceColor.AtNewAlpha(indicator.GetColor().a);
    //    ShowIndicator(false);
    //}

    private void HandleIndicator()
    {
        //Cell hoveredCell = grid.CurrentHoveredCell();
        Cell hoveredCell = grid.HoveredCell;

        if (ValidCell(hoveredCell) && hoveredCell != IndicatorCell)
        {
            IndicatorCell = hoveredCell;
            grid.OnPieceIndicatorMoved?.Invoke(indicatorCell);

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

    public void PlaceOnIndicator()
    {
        IsHeld = false;
        //indicator.gameObject.SetActive(false);

        if (CanPlaceOnIndicator)
            CurrentCell = indicatorCell;
        else
            indicatorHandler.SetColor(indicatorHandler.DefaultColor);

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

    #region Input Callbacks
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!Interactable) return;

        IsHeld = true;
        grid.OnPiecePickedUp?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) return;

        PlaceOnIndicator();
        grid.OnPieceDropped?.Invoke(this, CanPlaceOnIndicator);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;

        HandleHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;

        HandleHover(false);
    }

    private void HandleHover(bool hover)
    {
        if (grid.SelectedPiece != null || !Interactable)
            return;

        grid.OnPieceHovered?.Invoke(this, hover);
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