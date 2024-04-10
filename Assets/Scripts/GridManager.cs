using FinishOne.GeneralUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int width, height;

    [field: SerializeField] public GameObject Board { get; private set; }

    [SerializeField] private GridPuzzleConfigSO puzzleConfig;

    [Header("Containers")]
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform pieceParent;

    [Space]
    [Header("Grid Generation")]
    [SerializeField] private List<GridPiece> piecePrefabs;


    private List<Cell> cells;
    private List<GridPiece> gridPieces;

    private List<GridPiece> gridPiecePool;
    private Vector2 POOL_POSITION = new Vector2(100, 100);

    private GridPiece selectedPiece;
    private bool previouslyInGrid;

    public Action<GridPiece> OnPiecePickedUp;
    public Action<GridPiece, bool> OnPieceDropped;
    public Action<GridPiece, bool> OnPieceHovered;

    public Action OnGridChanged;
    public Action OnGridReset;

    public Action<Cell> OnPieceIndicatorMoved;
    public Action OnPointerLeftGrid;

    #region Getter Properties
    public List<GridPiece> PiecePrefabs => piecePrefabs;

    public Transform CellParent => cellParent;
    public Transform PieceParent => pieceParent;

    public List<Cell> Cells => cells;
    public List<GridPiece> Pieces => gridPieces;

    public Cell HoveredCell { get; set; }
    public GridPiece SelectedPiece => selectedPiece;

    public int PieceCount => gridPieces.Where(piece => piece.Interactable).Count();

    public int Width => width;
    public int Height => height;

    public bool PointerInGrid { get; set; }
    public bool HoldingPiece => SelectedPiece != null;
    #endregion

    public Cell CurrentHoveredCell()
    {
        Cell hoveredCell = null;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (PointerInGrid)
            hoveredCell = GetClosestCell(mouseWorldPos);

        return hoveredCell;
    }

    public GridPuzzleConfigSO PuzzleConfig
    {
        get => puzzleConfig;
        set
        {
            puzzleConfig = value;

            SetupCells();

            ClearPieces();
            GenerateFromList(puzzleConfig.Pieces);

            OnGridReset?.Invoke();
            SetPiecesToGrid();
            SetupPieceEvents();
        }
    }

    private void Awake()
    {
        Instance = this;

        SetupCells();
        gridPieces = new();
    }

    private void Start()
    {
        if (Pieces.Count == 0)
            GrabPieces();

        SetPiecesToGrid();
        SetupPieceEvents();

        OnPiecePickedUp += PickedUpPiece;
        OnPieceDropped += (piece, canDrop) => DroppedPiece(piece);
    }

    private void SetupPieceEvents()
    {
        foreach(GridPiece piece in gridPieces)
        {
            GridPiece gridPiece = piece;
            piece.OnPickup += () => OnPiecePickedUp?.Invoke(piece);
            piece.OnDropped += (value) => OnPieceDropped?.Invoke(piece, value);
            piece.OnHovered += (hovered) => OnPieceHovered?.Invoke(piece, hovered);
            piece.OnIndicatorMoved += (cell) => OnPieceIndicatorMoved?.Invoke(cell);
        }
    }


    private void Update()
    {
       // HandlePointerInGrid();
    }

    private void HandlePointerInGrid()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 boardMousePos = Board.transform.InverseTransformDirection(mousePos);

        bool validX = boardMousePos.x.Between(-Width / 2, Width / 2);
        bool validY = boardMousePos.y.Between(-Height / 2, Height / 2);

        PointerInGrid = validX && validY;

        if (!PointerInGrid && previouslyInGrid)
            OnPointerLeftGrid?.Invoke();

        previouslyInGrid = PointerInGrid;

        if (!PointerInGrid)
        {
            HoveredCell = null;
        }
    }

    #region Pieces

    public void SetPiecesToGrid()
    {
        gridPieces.ForEach(piece => piece.CurrentCell = GetClosestCell(piece.transform));
        OnGridChanged?.Invoke();
    }

    private void PickedUpPiece(GridPiece piece)
    {
        selectedPiece = piece;

        // selecting cancels hovering
        OnPieceHovered?.Invoke(piece, false);
    }

    private void DroppedPiece(GridPiece piece)
    {
        selectedPiece = null;
    }

    public void AddPiece(GridPiece piece)
    {
        if (!gridPieces.Contains(piece))
            gridPieces.Add(piece);
    }

    public void RemovePiece(GridPiece piece)
    {
        gridPieces.Remove(piece);

        piece.transform.position = POOL_POSITION;
        piece.gameObject.SetActive(false);
    }

    #endregion

    private Cell GetClosestCell(Transform givenTransform) => GetClosestCell(givenTransform.position);

    private Cell GetClosestCell(Vector3 pos)
    {
        Cell closestCell = cells.OrderBy(cell => Vector3.Distance(pos, cell.transform.position)).First();
        return closestCell;
    }

    public void ResetToConfig()
    {
        PuzzleConfig = puzzleConfig;
    }

    private void GenerateFromList(List<GridPiece> gridList)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            GridPiece pieceToSpawn = gridList[i];

            if (pieceToSpawn != null)
            {
                GridPiece newPiece = CustomMethods.Instantiate(pieceToSpawn, PieceParent);

                newPiece.CurrentCell = cells[i];
                //newPiece.transform.position = cells[i].transform.position;
                gridPieces.Add(newPiece);
                gridPiecePool.Add(newPiece);
            }
        }
    }

    #region Grid Setup
    public void ClearPieces()
    {
        if (gridPieces == null)
        {
            GrabPieces();
        }

        for (int i = pieceParent.childCount - 1; i >= 0; i--)
        {
            GridPiece piece = pieceParent.GetChild(i).GetComponent<GridPiece>();

            RemoveListeners(piece);

            gridPieces.Remove(piece);
            DestroyImmediate(piece.gameObject);
        }

        gridPieces = new();
        gridPiecePool = new();
    }

    private void RemoveListeners(GridPiece piece)
    {
       // piece.OnPickup -= OnPiecePickedUp;
    }

    public void GrabCells()
    {
        cells = new();
        for (int i = 0; i < cellParent.childCount; i++)
        {
            Cell cell = cellParent.GetChild(i).GetComponent<Cell>();
            cells.Add(cell);
            cell.Init(this, i);
        }
    }

    private void SetupCells()
    {
        if (cells == null || cells.Count == 0 || Board == null)
        {
            if (cellParent.childCount > 0)
                GrabCells();
            else
                this.enabled = false;
        }
    }

    public void GrabPieces()
    {
        gridPieces = new List<GridPiece>();
        gridPiecePool = new List<GridPiece>();

        for (int i = 0; i < pieceParent.childCount; i++)
        {
            GridPiece piece = pieceParent.GetChild(i).GetComponent<GridPiece>();
            gridPieces.Add(piece);
            gridPiecePool.Add(piece);
        }
    }
    #endregion


    #region Editor Functions
    private void OnValidate()
    {
        if (puzzleConfig != null)
        {
            bool sameAsExisting = true;

            SetupCells();
            GrabPieces();
            SetPiecesToGrid();

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].Occupied != (puzzleConfig.Pieces[i] != null))
                {
                    sameAsExisting = false;
                    break;
                }

                if (cells[i].Occupied && (cells[i].CurrentPiece.PieceColor != puzzleConfig.Pieces[i].PieceColor))
                {
                    sameAsExisting = false;
                    break;
                }
            }

#if UNITY_EDITOR
            if (!sameAsExisting)
            {
                print("different config!");
                UnityEditor.EditorApplication.delayCall += ConfigOnValidate;
            }
#endif
        }
    }

    private void ConfigOnValidate()
    {
        if (Application.isPlaying)
            return;

        for (int i = pieceParent.childCount - 1; i >= 0; i--)
        {
            GridPiece piece = pieceParent.GetChild(i).GetComponent<GridPiece>();
            gridPieces.Remove(piece);

            DestroyImmediate(piece.gameObject);
        }
        gridPieces = new();
        gridPiecePool = new();

        GenerateFromList(puzzleConfig.Pieces);
    }
    #endregion
}