using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private GridPiece piece;
    private GridManager grid;
    private List<Cell> adjacentCells;

    public GridPiece CurrentPiece => piece;
    public List<Cell> AdjacentCells => adjacentCells;
    public bool Occupied => piece != null;
    public int IndexInGrid { get; private set; }
    public bool CanSetIndicatorColor { get; set; } = true;
    public bool Hovered { get; set; }

    public void Init(GridManager manager, int index)
    {
        grid = manager;
        IndexInGrid = index;
    }

    private void Start()
    {
        grid.SetCellInitialized();
    }

    public void CalculateAdjacentCells()
    {
        adjacentCells = GrabAdjacentCells();
    }

    private List<Cell> GrabAdjacentCells()
    {
        return new()
        {
            GetCellInDirection(transform.forward),
            GetCellInDirection(transform.right),
            GetCellInDirection(-transform.forward),
            GetCellInDirection(-transform.right)
        };
    }

    private Cell GetCellInDirection(Vector3 dir)
    {
        Cell cell = null;

        if(Physics.SphereCast(transform.position, 0.2f, dir, out RaycastHit hit))
        {
            cell = hit.transform.GetComponent<Cell>();
        }

        return cell;
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

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || AdjacentCells == null || AdjacentCells.Count == 0)
            return;

        Gizmos.color = Color.red;
        if (AdjacentCells[0] != null) Gizmos.DrawSphere(AdjacentCells[0].transform.position, 0.25f);
        if (AdjacentCells[1] != null) Gizmos.DrawSphere(AdjacentCells[1].transform.position, 0.25f);
        if (AdjacentCells[2] != null) Gizmos.DrawSphere(AdjacentCells[2].transform.position, 0.25f);
        if (AdjacentCells[3] != null) Gizmos.DrawSphere(AdjacentCells[3].transform.position, 0.25f);
    }
}