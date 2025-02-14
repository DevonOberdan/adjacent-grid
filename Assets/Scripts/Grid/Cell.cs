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

    private List<Cell> GrabAdjacentCells()
    {
        return GetAdjacentCellsFromPosition();
    }

    private List<Cell> GetAdjacentCellsFromGridList()
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

    private List<Cell> GetAdjacentCellsFromPosition()
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

    public void ResetCell()
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.red;
        if (AdjacentCells[0] != null) Gizmos.DrawSphere(AdjacentCells[0].transform.position, 0.25f);
        if (AdjacentCells[1] != null) Gizmos.DrawSphere(AdjacentCells[1].transform.position, 0.25f);
        if (AdjacentCells[2] != null) Gizmos.DrawSphere(AdjacentCells[2].transform.position, 0.25f);
        if (AdjacentCells[3] != null) Gizmos.DrawSphere(AdjacentCells[3].transform.position, 0.25f);
    }
}