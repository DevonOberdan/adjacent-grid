using UnityEngine;
using UnityEngine.EventSystems;
using FinishOne.GeneralUtilities;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class GridPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GridManager grid;
    private Cell currentCell, indicatorCell;

    private Renderer rend;

    private SpriteRenderer sprite;
    private GameObject indicator;
    private Color pieceColor, indicatorColor;
    private bool canPlaceOnIndicator;

    #region Properties

    public bool IsHeld { get; private set; }
    public Color IndicatorColor => indicatorColor;
    public Color PieceColor => pieceColor;

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

            indicator.transform.position = indicatorCell.transform.position;
            ShowIndicator(true);
        }
    }

    public bool CanPlaceOnIndicator
    {
        get => canPlaceOnIndicator;
        set
        {
            canPlaceOnIndicator = value;
            indicator.SetColor(canPlaceOnIndicator ? IndicatorColor : GridInputHandler.Instance.InvalidPlacementColor);
        }
    }
    #endregion

    #region Callbacks


    private void Awake()
    {
        grid = transform.root.GetComponent<GridManager>();
        //sprite = GetComponent<SpriteRenderer>();

        rend = GrabRenderer();

        //pieceColor = rend.;
    }

    private void Start()
    {
        SpawnIndicator();
    }

    private void Update()
    {
        if (IsHeld)
        {
            HandleIndicator();
        }
    }

    #endregion

    private Renderer GrabRenderer()
    {
        if(!TryGetComponent(out Renderer renderer))
        {
            renderer = GetComponentInChildren<Renderer>();

            if(renderer == null)
            {
                Debug.LogError("No Renderer in GridPiece", gameObject);
            }
        }

        return renderer;
    }

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

    public void Highlight(bool highlight) => sprite.color = highlight ? IndicatorColor : pieceColor;
    public bool IsOfSameType(GridPiece newPiece) => this.PieceColor.Equals(newPiece.PieceColor);

    public void SetColor(Color color) => pieceColor = color;

    #endregion

    #region Private Helper Methods

    private void SpawnIndicator()
    {
        indicator = Instantiate(GridInputHandler.Instance.VisualIndicator, transform);
        indicator.transform.localPosition = Vector3.zero;

        //indicatorColor = pieceColor.AtNewAlpha(indicator.color.a);
        ShowIndicator(false);
    }

    private void HandleIndicator()
    {
        Cell hoveredCell = grid.CurrentHoveredCell();

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
        indicator.gameObject.SetActive(false);

        if (CanPlaceOnIndicator)
            CurrentCell = indicatorCell;
        else
            indicator.SetColor(IndicatorColor);


        IndicatorCell = CurrentCell;
        ShowIndicator(false);
    }

    public void ShowIndicator(bool show)
    {
        if (indicator == null)
            return;

        indicator.gameObject.SetActive(false);
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
        IsHeld = true;
        grid.OnPiecePickedUp?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlaceOnIndicator();
        grid.OnPieceDropped?.Invoke(this, CanPlaceOnIndicator);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandleHover(false);
    }

    private void HandleHover(bool hover)
    {
        if (grid.SelectedPiece != null)
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