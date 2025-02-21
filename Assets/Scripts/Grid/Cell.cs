using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private GridPiece piece;
    private GridManager grid;
    private List<Cell> adjacentCells;

    private Vector3 startPosition;
    private Quaternion startRotation;

    public GridPiece CurrentPiece => piece;
    public List<Cell> AdjacentCells => adjacentCells ??= GrabAdjacentCells();
    public bool Occupied => piece != null;
    public int IndexInGrid { get; private set; }
    public bool CanSetIndicatorColor { get; set; } = true;
    public bool Hovered { get; set; }

    public void Init(GridManager manager, int index)
    {
        grid = manager;
        IndexInGrid = index;
    }

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Start()
    {
        grid.OnGridReset += () => ResetCell();
    }

    public void CalculateAdjacentCells()
    {
        adjacentCells = GrabAdjacentCells();
    }

    private List<Cell> GrabAdjacentCells()
    {
        List<Cell> adjacent = new List<Cell>();

        bool topCell = IndexInGrid / grid.Height != grid.Height - 1;
        bool rightCell = IndexInGrid % grid.Width != grid.Width - 1;
        bool bottomCell = IndexInGrid / grid.Height != 0;
        bool leftCell = IndexInGrid % grid.Width != 0;

        adjacent.Add(topCell ? grid.Cells[IndexInGrid + grid.Width] : null);
        adjacent.Add(rightCell ? grid.Cells[IndexInGrid + 1] : null);
        adjacent.Add(bottomCell ? grid.Cells[IndexInGrid - grid.Width] : null);
        adjacent.Add(leftCell ? grid.Cells[IndexInGrid - 1] : null);

        return adjacent;
    }

    public void AddPiece(GridPiece newPiece)
    {
        piece = newPiece;
    }

    // Piece passed in to ensure that no accidental removal
    // of a new/unknown piece occurs
    public void RemovePiece(GridPiece pieceToRemove)
    {
        if (piece == pieceToRemove)
            piece = null;
    }

    public void ResetCell()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}