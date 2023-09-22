using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class GridPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GridManager grid;
    private Cell currentCell;
    private SpriteRenderer sprite;
    private Color pieceColor;
    private bool isHeld;
    private bool canPlaceOnIndicator;
    private SpriteRenderer indicator;
    private Cell indicatorCell;
    private Color indicatorColor;




    public Cell CurrentCell {
        get => currentCell;
        set {
            if (currentCell != null)
                currentCell.RemovePiece(this);

            // piece cannot be added to this cell
            if (!value.AddPiece(this))
                return;


            ShowIndicator(value != currentCell);

            currentCell = value;

            gameObject.SetActive(true);

            // set position.. DOTween later
            transform.position = currentCell.transform.position;
        }
    }

    public Cell IndicatorCell {
        get => indicatorCell;
        set {
            indicatorCell = value;

            indicator.transform.position = indicatorCell.transform.position;

            ShowIndicator(true);//indicatorCell != CurrentCell);
        }
    }

    public Color IndicatorColor => indicatorColor;
    public Color PieceColor => pieceColor;

    public bool CanPlaceOnIndicator {
        get => canPlaceOnIndicator;
        set {
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

    private void Start()
    {
        SpawnIndicator();
    }

    private void SetToGrid() => CurrentCell = grid.GetClosestCell(transform);

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
        ShowIndicator(false);
        //canPlaceOnIndicator = true;
    }

    private void SpawnIndicator()
    {
        indicator = Instantiate(GridInputHandler.Instance.VisualIndicator, transform);
        indicator.transform.localPosition = Vector3.zero;
        indicatorColor = new Color(pieceColor.r, pieceColor.g, pieceColor.b, indicator.color.a);
        ShowIndicator(false);
    }

    public void ShowIndicator(bool show)
    {
        if (indicator == null)
            return;

        indicator.enabled = show;
    }

    public void Highlight(bool highlight)
    {
        Debug.Log("Highlight: " + highlight);
        sprite.color = highlight ? IndicatorColor : pieceColor;
    }


    private void Update()
    {
        if (isHeld)
        {
            //FollowMouse();
            HandleIndicator();
        }
    }

    private void FollowMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.position = Vector2.Lerp(transform.position, mouseWorldPos, GridInputHandler.Instance.DragCatchUpSpeed * Time.deltaTime);
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

    private bool ValidCell(Cell cell) => cell != null && cell == CurrentCell || CurrentCell.AdjacentCells.Contains(cell);

    public void SetIndicatorCell(Cell newIndicatorCell) => indicatorCell = newIndicatorCell;

    public void SetHeld(bool held) => isHeld = held;

    public void SetColor(Color color) => pieceColor = color;

    public void MarkIndicatorCellInvalid()
    {
        CanPlaceOnIndicator = false;
        IndicatorCell = CurrentCell;
        ShowIndicator(true);
    }

    #region Input Callbacks

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        grid.OnPiecePickedUp?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlaceOnIndicator();
        grid.OnPieceDropped?.Invoke(this, CanPlaceOnIndicator);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (grid.SelectedPiece != null)
            return;

        grid.OnPieceHovered?.Invoke(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (grid.SelectedPiece != null)
            return;

        grid.OnPieceHovered?.Invoke(this, false);

    }
    #endregion

    #region Editor Functions
    private void OnValidate()
    {
        HandleDroppedInPieces();
    }

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