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

    //public enum AXES { XY, XZ };
    //[SerializeField] private AXES axes;

    [field: SerializeField] public float CellSpacing { get; private set; }

    public float CellParentPositionValue => -1 * CellSpacing * 1.5f;

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

    private bool PiecesNotSet => gridPieces.Any(piece => piece.CurrentCell == null);

   // public AXES GridAxes => axes;
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

        if(gridPieces == null)
            gridPieces = new();
    }

    private void Start()
    {
        if (Pieces.Count == 0)
            GrabPieces();

        if (PiecesNotSet)
        {
            SetPiecesToGrid();
            SetupPieceEvents();
        }

        OnPiecePickedUp += PickedUpPiece;
        OnPieceDropped += (piece, canDrop) => DroppedPiece(piece);
    }

    private void SetupPieceEvents()
    {
        foreach(GridPiece piece in gridPieces)
        {
            GridPiece gridPiece = piece;
            piece.OnPickup += () => OnPiecePickedUp?.Invoke(piece);
            piece.OnDropSuccessful += (value) => OnPieceDropped?.Invoke(piece, value);
            piece.OnHovered += (hovered) => OnPieceHovered?.Invoke(piece, hovered);
            piece.OnIndicatorMoved += (cell) => OnPieceIndicatorMoved?.Invoke(cell);
        }
    }

    private void Update()
    {
       //HandlePointerInGrid();

        //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //RaycastHit[] hits = Physics.RaycastAll()
    }

    private void HandlePointerInGrid()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 boardMousePos = pieceParent.transform.InverseTransformDirection(mousePos);

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
        if(piece.CurrentCell != null)
        {
            piece.CurrentCell.RemovePiece(piece);
        }

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
            if (cells[i] == null)
                continue;

            GridPiece pieceToSpawn = gridList[i];

            if (pieceToSpawn != null)
            {
                GridPiece newPiece = CustomMethods.Instantiate(pieceToSpawn, PieceParent);
                newPiece.gameObject.name = pieceToSpawn.gameObject.name;

                newPiece.CurrentCell = cells[i];
                gridPieces.Add(newPiece);
                gridPiecePool.Add(newPiece);
            }
        }
    }

    #region Grid Setup
    public void ClearPieces()
    {
        if(cellParent == null || pieceParent == null)
            return;

        if (gridPieces == null)
        {
            GrabPieces();
        }

        foreach(Cell cell in cellParent.GetComponentsInChildren<Cell>())
        {
            if(cell.CurrentPiece != null)
            {
                cell.RemovePiece(cell.CurrentPiece);
            }
        }

        for (int i = pieceParent.childCount - 1; i >= 0; i--)
        {
            if(pieceParent.GetChild(i).TryGetComponent(out GridPiece piece))
            {
                gridPieces.Remove(piece);

                if(Application.isPlaying)
                    Destroy(piece.gameObject);
                else
                    DestroyImmediate(piece.gameObject);
            }
        }

        gridPieces.Clear();
        gridPieces = new();
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
        if (cells != null && cells.Count >0 && CellParentPositionValue != cellParent.transform.localPosition.x)
        {
            SpaceOutCells();
        }

        //if (GridAxes == AXES.XY)
        //{
        //    cellParent.transform.localPosition = new Vector2(CellParentPositionValue, CellParentPositionValue);
        //    transform.eulerAngles = transform.eulerAngles.NewX(-90);
        //}
        //else
        //{
        //    cellParent.transform.localPosition = new Vector3(CellParentPositionValue, 0, CellParentPositionValue);
        //    transform.eulerAngles = Vector3.zero;
        //}

        cellParent.transform.localPosition = new Vector3(CellParentPositionValue, 0, CellParentPositionValue);
        transform.eulerAngles = Vector3.zero;


        if (puzzleConfig != null)
        {
            SetupCells();
            GrabPieces();
            SetPiecesToGrid();

            bool sameAsExisting = GridSameAsConfig();

#if UNITY_EDITOR
            if (!sameAsExisting)
            {
                print("different config!");
                UnityEditor.EditorApplication.delayCall += ConfigOnValidate;
            }
#endif
        }
    }

    public void SpaceOutCells()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                float width = j * CellSpacing;
                float height = i * CellSpacing;

                cells[(i*Height)+j].transform.localPosition = new Vector3(width, 0, height);
            }
        }
    }

    public bool GridSameAsConfig()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Occupied != (puzzleConfig.Pieces[i] != null))
            {
                return false;
            }

            if (cells[i].Occupied && !cells[i].CurrentPiece.IsOfSameType(puzzleConfig.Pieces[i]))
            {
                return false;
            }
        }

        return true;
    }

    private void ConfigOnValidate()
    {
        if (Application.isPlaying)
            return;

        ClearPieces();

        if (puzzleConfig == null || puzzleConfig.Pieces == null || puzzleConfig.Pieces.Count == 0)
            return;

        GenerateFromList(puzzleConfig.Pieces);
    }
    #endregion
}