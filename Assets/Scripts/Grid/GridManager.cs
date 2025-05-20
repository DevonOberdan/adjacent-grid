using FinishOne.GeneralUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private GridPuzzleConfigSO puzzleConfig;

    [Space]
    [Header("Input System")]
    [SerializeField] private InputActionAsset gameActions;
    private InputAction pointerAction;

    [Header("Containers")]
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform pieceParent;

    [Space]
    [Header("Events")]
    [SerializeField] private UnityEvent OnPieceConsumed;

    [Space]
    [Header("Grid Generation")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [field: SerializeField] public GameObject Board { get; private set; }

    [SerializeField] private List<GridPiece> piecePrefabs;

    [field: SerializeField] public float CellSpacing { get; private set; }
    [field: SerializeField] public Cell DefaultCellPrefab { get; private set; }

    public float CellParentPositionValue => -1 * CellSpacing * 1.5f;

    private List<Cell> cells;
    private List<GridPiece> gridPieces;

    private Vector2 POOL_POSITION = new Vector2(100, 100);

    private GridPiece selectedPiece;

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

    public bool Interactable { get; set; } = true;

    private int setupCellCount = 0;

    public GridPuzzleConfigSO PuzzleConfig
    {
        get => puzzleConfig;
        set
        {
            puzzleConfig = value;

            StopAllCoroutines();
            setupCellCount = 0;

            ClearPieces();
            ClearCells();
            GeneratePuzzle();

            OnGridReset?.Invoke();
            SetPiecesToGrid();
            SetupPieceEvents();
        }
    }

    private void Awake()
    {
        Instance = this;

        gridPieces ??= new();

        if(cells == null || cells.Count == 0 && cellParent.childCount > 0)
        {
            GrabCells();
        }

        gameActions.Enable();
        pointerAction = gameActions.FindActionMap("Gameplay").FindAction("Hover");
    }

    private void Start()
    {
        if (Pieces.Count == 0)
            GrabPieces();

        OnPiecePickedUp += PickedUpPiece;
        OnPieceDropped += DroppedPiece;
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
            piece.OnPieceMoved += () => OnPieceConsumed.Invoke();
        }
    }

    public void SetCellInitialized()
    {
        if (++setupCellCount == cells.Count)
        {
            setupCellCount = 0;
            StartCoroutine(FinalizePuzzleSetup());
        }
    }

    private IEnumerator FinalizePuzzleSetup()
    {
        yield return new WaitForSeconds(0.025f);

        foreach(Cell cell in cells.Where(c => c != null))
        {
            cell.CalculateAdjacentCells();
        }

        OnGridChanged?.Invoke();
    }

    private void Update()
    {
        FindHoveredCell(pointerAction.ReadValue<Vector2>());
    }

    private void FindHoveredCell(Vector2 pointerPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (!hit.collider.TryGetComponent(out Cell cell))
                continue;

            if (HoveredCell != null && HoveredCell != cell)
                HoveredCell.Hovered = false;

            HoveredCell = cell;
        }
    }

    #region Pieces
    public void SetPiecesToGrid()
    {
        if (gridPieces == null || gridPieces.Count == 0 || cells == null || cells.Count==0)
            return;

        gridPieces.ForEach(piece => piece.CurrentCell = GetClosestCell(piece.transform));
    }

    public void PickedUpPiece(GridPiece piece)
    {
        selectedPiece = piece;
        OnPieceHovered?.Invoke(piece, false);
    }

    private void DroppedPiece(GridPiece piece, bool canDrop)
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
    }
    #endregion

    private Cell GetClosestCell(Transform givenTransform) => GetClosestCell(givenTransform.position);

    private Cell GetClosestCell(Vector3 pos)
    {
        return cells.OrderBy(cell => Vector3.Distance(pos, cell.transform.position)).First();
    }

    public void ResetToConfig()
    {
        PuzzleConfig = puzzleConfig;
    }

    private void GeneratePuzzle()
    {
        if(puzzleConfig == null)
        {
            Debug.LogError("puzzleConfig is null");
            return;
        }

        GenerateCells();

        for (int i=0; i< cells.Count; i++)
        {
            if (cells[i] == null || puzzleConfig.Pieces[i] == null)
            {
                continue;
            }

            GridPiece newPiece = CustomMethods.Instantiate(puzzleConfig.Pieces[i], PieceParent);
            newPiece.gameObject.name = puzzleConfig.Pieces[i].gameObject.name;
            newPiece.CurrentCell = cells[i];

            gridPieces.Add(newPiece);
        }
    }

    private void GenerateCells()
    {
        if (puzzleConfig.CellConfig != null && puzzleConfig.CellConfig.Count > 0)
        {
            for (int i = 0; i < puzzleConfig.CellConfig.Count; i++)
            {
                CellConfigData cellConfig = puzzleConfig.CellConfig[i];

                Cell newCell = CustomMethods.Instantiate(cellConfig.Prefab, CellParent);
                newCell.transform.SetLocalPositionAndRotation(cellConfig.Pos, cellConfig.Rot);

                cells.Add(newCell);
                newCell.Init(this, i);
            }
        }
        else if (DefaultCellPrefab != null)
        {
            for (int i = 0; i < Width * Height; i++)
            {
                Cell newCell = CustomMethods.Instantiate(DefaultCellPrefab, CellParent);
                cells.Add(newCell);
                newCell.Init(this, i);
            }

            SpaceOutCells();
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

        foreach(Cell cell in cellParent.GetComponentsInChildren<Cell>().Where(c => c.CurrentPiece != null))
        {
            cell.RemovePiece(cell.CurrentPiece);
        }

        for (int i = pieceParent.childCount - 1; i >= 0; i--)
        {
            if(pieceParent.GetChild(i).TryGetComponent(out GridPiece piece))
            {
                gridPieces.Remove(piece);
                DestroyGameObject(piece.gameObject);
            }
        }

        gridPieces.Clear();
        gridPieces = new();
    }

    private void DestroyGameObject(GameObject obj)
    {
        obj.SetActive(false);

        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else if (obj.scene.IsValid())
        {
            DestroyImmediate(obj);
        }
    }

    public void GrabCells()
    {
        cells = new();
        for (int i = 0; i < cellParent.childCount; i++)
        {
            Cell cell = cellParent.GetChild(i).GetComponent<Cell>();
            cell.gameObject.name = "Tile"+(i+1);
            cells.Add(cell);
            cell.Init(this, i);
        }
    }

    private void ClearCells()
    {
        if(cellParent == null)
        {
            return;
        }

        for (int i = cellParent.childCount - 1; i >= 0; i--)
        {
            if (cellParent.GetChild(i).TryGetComponent(out Cell cell))
            {
                cells.Remove(cell);
                DestroyGameObject(cell.gameObject);
            }
        }

        cells.Clear();
    }

    public void GrabPieces()
    {
        gridPieces = new List<GridPiece>();

        for (int i = 0; i < pieceParent.childCount; i++)
        {
            GridPiece piece = pieceParent.GetChild(i).GetComponent<GridPiece>();
            gridPieces.Add(piece);
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
                cells[(i * Height) + j].transform.localPosition = new Vector3(width, 0, height);
            }
        }
    }
    #endregion

    #region Editor Functions
    private void OnValidate()
    {
        //if (cells != null && cells.Count >0 && CellParentPositionValue != cellParent.transform.localPosition.x)
        //{
        //    SpaceOutCells();
        //}
        //cellParent.transform.localPosition = new Vector3(CellParentPositionValue, 0, CellParentPositionValue);
        //transform.eulerAngles = Vector3.zero;

        if (puzzleConfig != null)
        {
            bool sameAsExisting = false;

            if(cellParent.childCount > 0)
            {
                GrabCells();
                GrabPieces();
                SetPiecesToGrid();

                sameAsExisting = GridSameAsConfig();
            }

#if UNITY_EDITOR
            if (!sameAsExisting || cells == null || cells.Count == 0)
            {
                print("different config!");
                UnityEditor.EditorApplication.delayCall += ConfigOnValidate;
            }
#endif
        }
    }

    public bool GridSameAsConfig()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            //empty when it should have piece, or vice-versa
            if (cells[i].Occupied != (puzzleConfig.Pieces[i] != null))
            {
                return false;
            }

            //wrong type
            if (cells[i].Occupied && !cells[i].CurrentPiece.IsOfSameType(puzzleConfig.Pieces[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool PieceCanLandOnCell(GridPiece piece, Cell cell)
    {
        GridPiece cellPiece = cell.CurrentPiece;

        // Grouped piece trying to move "out" of the grid left or right 
        if (!(cell == piece.CurrentCell || piece.IndicatorCell.AdjacentCells.Contains(cell)))
        {
            return false;
        }
        // new cell has blocking piece OR a piece of the same type
        else if (cellPiece != null && (cellPiece.Consumable == false || cellPiece.IsOfSameType(piece)))
        {
            Debug.Log("Piece would land on invalid piece, ", piece.gameObject);
            return false;
        }

        return true;
    }

    private void ConfigOnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        ClearPieces();

        if (puzzleConfig == null || puzzleConfig.Pieces == null || puzzleConfig.Pieces.Count == 0)
        {
            Debug.Log("No pieces defined in config");
            return;
        }

        ClearCells();

        GeneratePuzzle();
    }
    #endregion
}