using FinishOne.GeneralUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private SpriteRenderer board;

    [SerializeField] private GridPuzzleConfigSO puzzleConfig;

    [Header("Containers")]
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform pieceParent;

    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private Button historyButton;

    [Space]
    [Header("Grid Generation")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private List<GridPiece> piecePrefabs;


    private List<Cell> cells;
    private List<GridPiece> gridPieces;
    private List<GridPiece[]> gridHistory;

    private List<GridPiece> gridPiecePool;
    private Vector2 POOL_POSITION = new Vector2(100, 100);

    public List<GridPiece> PiecePrefabs => piecePrefabs;

    public Transform PieceParent => pieceParent;

    public List<Cell> Cells => cells;
    public List<GridPiece> Pieces => gridPieces;

    public int ActivePieces => gridPieces.Count;

    public int Width => width;
    public int Height => height;

    public bool PointerInGrid { get; set; } //board.GetComponent<Collider2D>().bounds.Contains(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    //public bool PointInGrid(Vector3 mousePos) => board.GetComponent<Collider2D>().bounds.Contains(mousePos);

    public Action<GridPiece> OnPiecePickedUp;
    public Action<GridPiece, bool> OnPieceDropped;
    public Action<Cell> OnPieceIndicatorMoved;
    public Action OnPointerLeftGrid;

    public Cell HoveredOverCell { get; set; }

    private GridPiece selectedPiece;
    public GridPiece SelectedPiece => selectedPiece;

    public Cell CurrentHoveredCell()
    {
        Cell hoveredCell = null;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (PointerInGrid)
            hoveredCell = GetClosestCell(mouseWorldPos);

        return hoveredCell;
    }

    public GridPuzzleConfigSO PuzzleConfig {
        get => puzzleConfig;
        set {
            puzzleConfig = value;

            SetupCells();

            ClearPieces();
            GenerateList(puzzleConfig.Pieces);

            SetPiecesToGrid();
            ResetHistory();
            RecordPiecePlacement();
        }
    }

    private void Awake()
    {
        Instance = this;

        SetupCells();

        gridPieces = new();
        ResetHistory();
    }

    private void Start()
    {
        if (Pieces.Count == 0)
            GrabPieces();

        SetPiecesToGrid();
        RecordPiecePlacement();

        OnPiecePickedUp += (piece) => selectedPiece = piece;
    }

    public void SetPuzzleConfig(GridPuzzleConfigSO newConfig)
    {
        ClearPieces();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);//, gridLayer.value);

        if (hit.collider != null)
            PointerInGrid = hit.collider.CompareTag("Background");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 boardMousePos = board.transform.InverseTransformDirection(mousePos);

        bool validX = boardMousePos.x.Between(-Width / 2, Width / 2);
        bool validY = boardMousePos.y.Between(-Height / 2, Height / 2);

        if (PointerInGrid && (!validX || !validY))
            OnPointerLeftGrid?.Invoke();

        PointerInGrid = validX && validY;

        if (!PointerInGrid)
            HoveredOverCell = null;

        if (historyButton != null)
            historyButton.interactable = gridHistory.Count > 1;
    }

    public void GrabCells()
    {
        cells = new();
        //List<Cell> existingCells = new List<Cell>();
        for (int i = 0; i < cellParent.childCount; i++)
        {
            Cell cell = cellParent.GetChild(i).GetComponent<Cell>();
            cells.Add(cell);
            cell.Init(this, i);
        }
        //return cells;
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

        //if(gridPieces.Count == 0)
        //{
        //    GenerateRandomPieces();
        //}
    }

    public void SetPiecesToGrid()
    {
        gridPieces.ForEach(piece => piece.CurrentCell = GetClosestCell(piece.transform));
    }

    private void SetupCells()
    {
        if (cells == null || cells.Count == 0 || board == null)
        {
            if (cellParent.childCount > 0)
                GrabCells();
            else
                GenerateGrid();
        }
    }

    private void ResetData()
    {
        if (gridHistory == null) gridHistory = new();
        else gridHistory.Clear();

        if (gridPieces == null) gridPieces = new();
        else gridPieces.Clear();

        if (gridPiecePool == null) gridPiecePool = new();
        else gridPiecePool.Clear();
    }



    public void RemovePiece(GridPiece piece)
    {
        gridPieces.Remove(piece);

        piece.transform.position = POOL_POSITION;
        piece.gameObject.SetActive(false);
    }

    public Cell GetClosestCell(Transform givenTransform) => GetClosestCell(givenTransform.position);

    public Cell GetClosestCell(Vector3 pos)
    {
        Cell closestCell = cells.OrderBy(cell => Vector2.Distance(pos, cell.transform.position)).First();
        return closestCell;
    }

    #region Grid History

    public void ResetHistory()
    {
        if (gridHistory == null)
            gridHistory = new();
        else
            gridHistory.Clear();

        if (historyButton != null)
            historyButton.interactable = false;
    }
    public void RecordPiecePlacement()
    {
        // add current configuration to list of lists
        GridPiece[] currentCells = new GridPiece[Cells.Count];

        for (int i = 0; i < Cells.Count; i++)
        {
            currentCells[i] = Cells[i].CurrentPiece;
        }

        if (!SameAsLastState(currentCells))
            gridHistory.Add(currentCells);
    }

    private bool SameAsLastState(GridPiece[] newPieceConfig)
    {
        if (gridHistory.Count == 0)
            return false;

        for (int i = 0; i < gridHistory[^1].Length; i++)
        {
            if (newPieceConfig[i] != gridHistory[^1][i])
                return false;
        }

        return true;
    }


    public void RewindGrid()
    {
        if (gridHistory.Count <= 1)
            return;

        gridHistory.RemoveAt(gridHistory.Count - 1);

        GridPiece[] previousGridConfig = gridHistory[^1];

        for (int i = 0; i < Cells.Count; i++)
        {
            if (previousGridConfig[i] != null)
            {
                if (!gridPieces.Contains(previousGridConfig[i]))
                    gridPieces.Add(previousGridConfig[i]);

                previousGridConfig[i].CurrentCell = Cells[i];
                previousGridConfig[i].IndicatorCell = previousGridConfig[i].CurrentCell;
            }
        }
    }
    #endregion


    public void ClearPieces()
    {
        if (gridPieces == null)
        {
            GrabPieces();
        }

        for (int i = pieceParent.childCount - 1; i >= 0; i--)
        {
            GridPiece piece = pieceParent.GetChild(i).GetComponent<GridPiece>();
            gridPieces.Remove(piece);
            DestroyImmediate(piece.gameObject);
        }
        gridPieces = new();
        gridPiecePool = new();
    }

    public void GenerateRandomPieces()
    {
        ClearPieces();

        if (cells == null || cells.Count == 0)
        {
            GrabCells();
        }

        foreach (Cell cell in cells)
        {
            int randomPiece = Random.Range(0, piecePrefabs.Count);
            GridPiece pieceToSpawn = piecePrefabs[Random.Range(0, piecePrefabs.Count)];

            if (pieceToSpawn != null)
            {
                GridPiece spawnedPiece = Instantiate(pieceToSpawn, pieceParent);
                spawnedPiece.CurrentCell = cell;

                gridPieces.Add(spawnedPiece);
                gridPiecePool.Add(spawnedPiece);
            }
        }
    }

    public void GenerateList(List<GridPiece> gridList)
    {
        print("Generating from config");
        for (int i = 0; i < cells.Count; i++)
        {
            GridPiece pieceToSpawn = gridList[i];

            if (pieceToSpawn != null)
            {
                GridPiece newPiece = Instantiate(gridList[i], PieceParent);
                newPiece.transform.position = cells[i].transform.position;
                gridPieces.Add(newPiece);
                gridPiecePool.Add(newPiece);
            }
        }
    }

    private void ClearGrid()
    {
        for (int i = cellParent.childCount - 1; i >= 0; i--)
        {
            GridPiece cell = cellParent.GetChild(i).GetComponent<GridPiece>();
            gridPieces.Remove(cell);
            DestroyImmediate(cell.gameObject);
        }

        cells = new List<Cell>();

        if (board != null)
        {
            DestroyImmediate(board);
            board = null;
        }
    }

    public void GenerateGrid()
    {
        ClearGrid();

        Vector2 center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        transform.position = -center;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Cell newCell = Instantiate(cellPrefab, cellParent);
                newCell.transform.localPosition = new Vector2(j, i);
                cells.Add(newCell);
            }
        }

        SpriteRenderer board = Instantiate(boardPrefab, transform);
        board.transform.localPosition = center;
        board.size = new Vector2(width, height) + new Vector2(0.1f, 0.1f);
    }


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

        GenerateList(puzzleConfig.Pieces);
    }
}