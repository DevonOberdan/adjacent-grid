using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler
{
    private GridPiece piece;
    private GridManager grid;
    private List<Cell> adjacentCells;
    private int indexInGrid;


    public GridPiece CurrentPiece => piece;
    public List<Cell> AdjacentCells => adjacentCells;

    public bool Occupied => piece != null;
    public int IndexInGrid => indexInGrid;


    public void Init(GridManager manager, int index)
    {
        grid = manager;
        indexInGrid = index;
    }

    private void Awake()
    {
        adjacentCells = new List<Cell>();
    }

    private void Start()
    {
        indexInGrid = grid.Cells.IndexOf(this);

        GrabAdjacentCells();
    }

    private void GrabAdjacentCells()
    {
        bool leftCell = indexInGrid % grid.Width != 0;
        bool rightCell = indexInGrid % grid.Width != grid.Width - 1;
        bool topCell = indexInGrid / grid.Height != grid.Height - 1;
        bool bottomCell = indexInGrid / grid.Height != 0;

        if (leftCell) adjacentCells.Add(grid.Cells[indexInGrid - 1]);
        if (rightCell) adjacentCells.Add(grid.Cells[indexInGrid + 1]);
        if (topCell) adjacentCells.Add(grid.Cells[indexInGrid + grid.Width]);
        if (bottomCell) adjacentCells.Add(grid.Cells[indexInGrid - grid.Width]);
    }

    public bool AddPiece(GridPiece newPiece)
    {
        // if cell is empty or contains piece of another color

        if (Occupied)
        {
            if (!newPiece.IsOfSameType(piece))
                DestroyPiece();
        }

        piece = newPiece;
        return true;
    }

    public void RemovePiece(GridPiece pieceToRemove)
    {
        if (piece == pieceToRemove)
            piece = null;
    }

    private void DestroyPiece()
    {
        grid.RemovePiece(piece);
        piece = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        grid.HoveredOverCell = this;
    }
}