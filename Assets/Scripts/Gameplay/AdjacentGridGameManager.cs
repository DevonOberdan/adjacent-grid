using FinishOne.GeneralUtilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdjacentGridGameManager : MonoBehaviour
{
    [SerializeField] private bool debug;
    [SerializeField] private bool highlightGroup;

    [SerializeField] private UnityEvent OnLevelComplete;
    private GridManager gridManager;
    private List<GridPiece> activeGrouping;
    private GridPiece activelyHeldPiece;
    private Dictionary<GridPiece, int> nonActivelyHeldPieceOffsets;

    private bool WinCondition => gridManager.ActivePieces == 1;


    private void Awake()
    {
        nonActivelyHeldPieceOffsets = new Dictionary<GridPiece, int>();
        activeGrouping = new List<GridPiece>();

        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        gridManager.OnPiecePickedUp += FindGroupedPieces;

        gridManager.OnPieceDropped += (piece, canDrop) => PlaceGroupedPieces();
        gridManager.OnPieceIndicatorMoved += MoveGroupedPieces;

        if (highlightGroup)
            gridManager.OnPieceHovered += HandlePieceHovered;
        //gridManager.OnPointerLeftGrid += HandleGridExit;
    }

    private void HandlePieceHovered(GridPiece hoveredPiece, bool hovered)
    {
        List<GridPiece> pieces = GetAdjacentPieces(hoveredPiece);

        foreach (GridPiece piece in pieces)
        {
            piece.Highlight(hovered);
        }
    }

    private void Update()
    {
        if (WinCondition)
        {
            OnLevelComplete.Invoke();
        }
    }

    public void HandleGridExit()
    {
        if (activelyHeldPiece == null)
            return;

        foreach (GridPiece piece in activeGrouping)
        {
            if (!piece.CanPlaceOnIndicator)
                return;
        }

        foreach (GridPiece piece in activeGrouping)
        {
            piece.IndicatorCell = piece.CurrentCell;
        }
    }

    private void MoveGroupedPieces(Cell activeIndicatorCell)
    {
        bool validMovement = ValidateMovement(activeIndicatorCell);

        if (validMovement)
        {
            foreach (GridPiece piece in nonActivelyHeldPieceOffsets.Keys)
            {
                int newIndicatorIndex = activeIndicatorCell.IndexInGrid + nonActivelyHeldPieceOffsets[piece];
                piece.IndicatorCell = gridManager.Cells[newIndicatorIndex];
            }

            bool validPlacement = AnyHitOpposingPiece() || debug;

            foreach (GridPiece piece in activeGrouping)
            {
                piece.CanPlaceOnIndicator = validPlacement;
            }
        }
        else
        {
            DisplayInvalidGrouping();

            // override the normal GridPiece Indicator handling 
            activelyHeldPiece.IndicatorCell = activeIndicatorCell;
            activelyHeldPiece.CanPlaceOnIndicator = false;
            activelyHeldPiece.ShowIndicator(false);
        }
    }

    // Hide all connected pieces that were not flagged as invalid
    private void DisplayInvalidGrouping()
    {
        foreach (GridPiece piece in activeGrouping)
        {
            if (piece.CanPlaceOnIndicator)
                piece.ShowIndicator(false);
        }
    }


    private bool ValidateMovement(Cell activeIndicatorCell)
    {
        bool allValid = true;

        foreach (GridPiece piece in nonActivelyHeldPieceOffsets.Keys)
        {
            int newIndicatorIndex = activeIndicatorCell.IndexInGrid + nonActivelyHeldPieceOffsets[piece];

            // Grouped piece out of bounds
            if (!gridManager.Cells.IsValidIndex(newIndicatorIndex))
            {
                piece.MarkIndicatorCellInvalid();
                allValid = false;
            }   // Grouped piece trying to move "out" of the grid left or right
            else if (!(gridManager.Cells[newIndicatorIndex] == piece.CurrentCell || piece.CurrentCell.AdjacentCells.Contains(gridManager.Cells[newIndicatorIndex])))
            {
                piece.MarkIndicatorCellInvalid();
                allValid = false;
            }
            else
            {
                // valid piece, hide it
                piece.IndicatorCell = piece.CurrentCell;
            }
        }

        return allValid;
    }

    private bool AnyHitOpposingPiece()
    {
        bool hit = false;

        foreach (GridPiece piece in activeGrouping)
        {
            // hits opposing piece       OR held in starting place
            if (HitsOpposingPiece(piece) || piece.IndicatorCell == piece.CurrentCell)
                hit = true;
        }
        return hit;
    }

    private bool HitsOpposingPiece(GridPiece piece)
    {
        // hovering over piece          && is opposing type
        return piece.IndicatorCell.Occupied && !piece.IndicatorCell.CurrentPiece.IsOfSameType(piece);
    }

    private void PlaceGroupedPieces()
    {
        foreach (GridPiece piece in nonActivelyHeldPieceOffsets.Keys)
        {
            piece.PlaceOnIndicator();
        }

        nonActivelyHeldPieceOffsets.Clear();
        activeGrouping.Clear();
        activelyHeldPiece = null;

        gridManager.RecordPiecePlacement();
    }

    private void FindGroupedPieces(GridPiece piece)
    {
        List<GridPiece> groupedPieces = GetAdjacentPieces(piece);

        activeGrouping = groupedPieces;
        activelyHeldPiece = piece;

        ProcessGroupedOffsets();

        groupedPieces.ForEach(piece => piece.ShowIndicator(true));
    }

    /// <summary>
    /// Returns list of all "adjacent" connected pieces from the given piece (i.e. the chain of 
    /// connected pieces of the same color).
    /// 
    /// List required to be passed in due to recursive nature of function.
    /// </summary>
    /// <param name="groupedPieces"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    private List<GridPiece> GetAdjacentPieces(GridPiece piece, List<GridPiece> groupedPieces = null)
    {
        if (groupedPieces == null)
            groupedPieces = new();

        groupedPieces.Add(piece);
        Cell currentPieceCell = piece.CurrentCell;

        foreach (Cell adjacentCell in currentPieceCell.AdjacentCells)
        {
            if (groupedPieces.Contains(adjacentCell.CurrentPiece))
                continue;

            if (adjacentCell.Occupied && adjacentCell.CurrentPiece.IsOfSameType(piece))
                groupedPieces = GetAdjacentPieces(adjacentCell.CurrentPiece, groupedPieces);
        }

        return groupedPieces;
    }

    private void ProcessGroupedOffsets()
    {
        foreach (GridPiece piece in activeGrouping)
        {
            if (piece == activelyHeldPiece)
                continue;
            nonActivelyHeldPieceOffsets.Add(piece, piece.CurrentCell.IndexInGrid - activelyHeldPiece.CurrentCell.IndexInGrid);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (activeGrouping != null)
            activeGrouping.ForEach(piece => Gizmos.DrawSphere(piece.transform.position, 0.1f));
    }
}