using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler
{

    SpriteRenderer visual;
    new BoxCollider2D collider;
    
    GridPiece piece;

    GridManager grid;

    List<Cell> adjacentCells;

    int indexInGrid;

    public bool Occupied => piece != null;

    public GridPiece CurrentPiece => piece;
    public List<Cell> AdjacentCells => adjacentCells;

    public int IndexInGrid => indexInGrid;


    private void Awake()
    {
        visual = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        adjacentCells = new List<Cell>();
    }

    private void GrabAdjacentCells()
    {
        bool leftCell  = indexInGrid % grid.Width != 0;
        bool rightCell = indexInGrid % grid.Width != grid.Width - 1;
        bool topCell   = indexInGrid / grid.Height != grid.Height - 1;
        bool bottomCell = indexInGrid / grid.Height != 0;

        if (leftCell)   adjacentCells.Add(grid.Cells[indexInGrid - 1]);
        if (rightCell)  adjacentCells.Add(grid.Cells[indexInGrid + 1]);
        if (topCell)    adjacentCells.Add(grid.Cells[indexInGrid + grid.Width]);
        if (bottomCell) adjacentCells.Add(grid.Cells[indexInGrid - grid.Width]);
    }

    public void Init(GridManager manager, int index)
    {
        grid = manager;
        indexInGrid = index;
    }

    void Start()
    {
        indexInGrid = grid.Cells.IndexOf(this);

        GrabAdjacentCells();
    }

    void Update()
    {
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("collision");
    }

    public bool AddPiece(GridPiece newPiece)
    {
        // if cell is empty or contains piece of another color

        if (Occupied)
        {
            if (!newPiece.IsOfSameType(piece))
                DestroyPiece();
            //else
            //    return false;
        }

        piece = newPiece;
        return true;
    }

    public void RemovePiece(GridPiece pieceToRemove)
    {
        if (piece == pieceToRemove)
            piece = null;
    }

    void DestroyPiece()
    {
        grid.RemovePiece(piece);
        //Destroy(piece.gameObject);
        piece = null;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //foreach (Cell cell in adjacentCells)
        //    Gizmos.DrawSphere(cell.transform.position, .25f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        grid.HoveredOverCell = this;
    }
}
