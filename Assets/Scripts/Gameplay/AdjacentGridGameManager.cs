using FinishOne.GeneralUtilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdjacentGridGameManager : MonoBehaviour
{
    [SerializeField] private bool debug;

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

        //gridManager.OnPointerLeftGrid += HandleGridExit;
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

        bool valid = true;

        foreach (GridPiece piece in activeGrouping)
        {
            if (!piece.CanPlaceOnIndicator)
                valid = false;
        }

        if (valid)
        {
            foreach (GridPiece piece in activeGrouping)
            {
                piece.IndicatorCell = piece.CurrentCell;
            }
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

            ValidatePlacement();
        }
        else
        {
            // override the normal GridPiece Indicator handling 
            activelyHeldPiece.IndicatorCell = activeIndicatorCell;
            activelyHeldPiece.CanPlaceOnIndicator = false;
            activelyHeldPiece.ShowIndicator(false);
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
                ShowAsInvalid(piece);
                allValid = false;
            }   // Grouped piece trying to move "out" of the grid left or right
            else if (!(gridManager.Cells[newIndicatorIndex] == piece.CurrentCell || piece.CurrentCell.AdjacentCells.Contains(gridManager.Cells[newIndicatorIndex])))
            {
                ShowAsInvalid(piece);
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

    private void ShowAsInvalid(GridPiece piece)
    {
        piece.CanPlaceOnIndicator = false;
        piece.IndicatorCell = piece.CurrentCell;
        piece.ShowIndicator(true);
    }

    private void ValidatePlacement()
    {
        bool validPlacement = HitsOpposingPiece() || debug;

        foreach (GridPiece piece in activeGrouping)
        {
            piece.CanPlaceOnIndicator = validPlacement;
        }
    }

    private bool HitsOpposingPiece()
    {
        bool hit = false;

        foreach (GridPiece piece in activeGrouping)
        {
            // hovering over piece          && is opposing type
            if (piece.IndicatorCell.Occupied && !piece.IndicatorCell.CurrentPiece.IsOfSameType(piece))
                hit = true;
        }
        return hit;
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

    public void FindGroupedPieces(GridPiece piece)
    {
        List<GridPiece> groupedPieces = new();

        groupedPieces = GetAdjacentPieces(groupedPieces, piece);

        activeGrouping = groupedPieces;
        activelyHeldPiece = piece;

        ProcessGroupedOffsets();
    }

    private List<GridPiece> GetAdjacentPieces(List<GridPiece> groupedPieces, GridPiece piece)
    {
        groupedPieces.Add(piece);
        Cell currentPieceCell = piece.CurrentCell;

        foreach (Cell adjacentCell in currentPieceCell.AdjacentCells)
        {
            if (groupedPieces.Contains(adjacentCell.CurrentPiece))
                continue;

            if (adjacentCell.Occupied && adjacentCell.CurrentPiece.IsOfSameType(piece))
                groupedPieces = GetAdjacentPieces(groupedPieces, adjacentCell.CurrentPiece);
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