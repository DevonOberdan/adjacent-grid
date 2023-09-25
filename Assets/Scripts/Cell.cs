using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler
{
    private GridPiece piece;
    private GridManager grid;
    private List<Cell> adjacentCells;

    public GridPiece CurrentPiece => piece;
    public List<Cell> AdjacentCells => adjacentCells;

    public bool Occupied => piece != null;
    public int IndexInGrid { get; private set; }


    public void Init(GridManager manager, int index)
    {
        grid = manager;
        IndexInGrid = index;
    }

    private void Awake()
    {
        adjacentCells = new List<Cell>();
    }

    private void Start()
    {
        IndexInGrid = grid.Cells.IndexOf(this);

        GrabAdjacentCells();
    }

    private void GrabAdjacentCells()
    {
        bool leftCell = IndexInGrid % grid.Width != 0;
        bool rightCell = IndexInGrid % grid.Width != grid.Width - 1;
        bool topCell = IndexInGrid / grid.Height != grid.Height - 1;
        bool bottomCell = IndexInGrid / grid.Height != 0;

        if (leftCell) adjacentCells.Add(grid.Cells[IndexInGrid - 1]);
        if (rightCell) adjacentCells.Add(grid.Cells[IndexInGrid + 1]);
        if (topCell) adjacentCells.Add(grid.Cells[IndexInGrid + grid.Width]);
        if (bottomCell) adjacentCells.Add(grid.Cells[IndexInGrid - grid.Width]);
    }

    public void AddPiece(GridPiece newPiece)
    {
        piece = newPiece;

        // set position.. DOTween later
        piece.transform.position = transform.position;
    }

    // Piece passed in to ensure that no accidental removal
    // of a new/unknown piece occurs
    public void RemovePiece(GridPiece pieceToRemove)
    {
        if (piece == pieceToRemove)
            piece = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        grid.HoveredCell = this;
    }
}