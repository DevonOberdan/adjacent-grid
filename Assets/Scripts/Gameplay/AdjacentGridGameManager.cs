using DG.Tweening;
using FinishOne.GeneralUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public struct PieceGroup
{
    public List<GridPiece> Group;

    public PieceGroup(List<GridPiece> group)
    {
        this.Group = group;
    }
}

public class AdjacentGridGameManager : MonoBehaviour
{
    [SerializeField] private bool highlightGroup;

    [SerializeField] private UnityEvent OnLevelComplete;
    [SerializeField] private UnityEvent<bool> OnDoomed;

    private GridManager gridManager;
    private List<GridPiece> activeGrouping;
    private GridPiece activelyHeldPiece;
    private Dictionary<GridPiece, int> nonActivelyHeldPieceOffsets;
    private bool wasDoomed;

    private bool WinCondition => gridManager.PieceCount == 1;

    public bool AboutToWin => gridManager.PieceCount == 2 && AnyValidMovements();
    public bool Doomed => wasDoomed;
    public List<PieceGroup> CurrentGroups { get; private set; }

    private void Awake()
    {
        nonActivelyHeldPieceOffsets = new Dictionary<GridPiece, int>();
        activeGrouping = new List<GridPiece>();

        gridManager = GetComponent<GridManager>();

        SetupGridListeners();
    }

    private void SetupGridListeners()
    {
        gridManager.OnPiecePickedUp += PickupGroupedPieces;
        gridManager.OnPieceDropped += HandlePieceDropped;
        gridManager.OnGridChanged += HandleGridChanged;

        gridManager.OnPieceIndicatorMoved += MoveGroupedPieces;

        if (highlightGroup)
            gridManager.OnPieceHovered += HandlePieceHovered;

        gridManager.OnGridChanged += FindAllGroups;
    }

    #region Grouping Functions

    private void PickupGroupedPieces(GridPiece piece)
    {
        List<GridPiece> groupedPieces = GetAdjacentPieces(piece);

        activeGrouping = groupedPieces;
        activelyHeldPiece = piece;

        ProcessGroupedOffsets();

        foreach (GridPiece groupedPiece in groupedPieces)
        {
            groupedPiece.HandlePickup();
        }
    }

    private void FindAllGroups()
    {
        CurrentGroups = new();

        List<GridPiece> checkedPieces = new List<GridPiece>();
        foreach (GridPiece piece in gridManager.Pieces)
        {
            List<GridPiece> newPieces = GetAdjacentPieces(piece);
            if(!newPieces.Any(piece => checkedPieces.Contains(piece)))
            {
                checkedPieces.AddRange(newPieces);
                CurrentGroups.Add(new PieceGroup(newPieces));
            }
        }
    }

    /// <summary>
    /// Returns list of all "adjacent" connected pieces from the given piece (i.e. the chain of 
    /// connected pieces of the same color).
    /// </summary>
    private List<GridPiece> GetAdjacentPieces(GridPiece piece, List<GridPiece> groupedPieces = null)
    {
        if (groupedPieces == null)
            groupedPieces = new();

        groupedPieces.Add(piece);
        Cell currentPieceCell = piece.CurrentCell;

        foreach (Cell adjacentCell in currentPieceCell.AdjacentCells)
        {
            if (groupedPieces.Contains(adjacentCell.CurrentPiece))
            {
                continue;
            }

            if (adjacentCell.Occupied && adjacentCell.CurrentPiece.IsOfSameType(piece))
            {
                groupedPieces = GetAdjacentPieces(adjacentCell.CurrentPiece, groupedPieces);
            }
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
    #endregion

    #region Handle Dropped
    private void HandlePieceDropped(GridPiece droppedPiece, bool actuallyDropped)
    {
        PlaceGroupedPieces();

        if (actuallyDropped && WinCondition)
            OnLevelComplete.Invoke();
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

        gridManager.OnGridChanged?.Invoke();
    }
    #endregion

    #region Doomed
    private void HandleGridChanged()
    {
        CheckDoomed();
    }

    private void CheckDoomed()
    {
        bool isDoomed = !WinCondition && !AnyValidMovements();
        if (isDoomed != wasDoomed)
            OnDoomed.Invoke(isDoomed);

        wasDoomed = isDoomed;
    }

    private bool AnyValidMovements()
    {
        foreach (GridPiece piece in gridManager.Pieces.Where(piece => piece.Interactable))
        {
            foreach (Cell adjacentCell in piece.CurrentCell.AdjacentCells)
            {
                GridPiece adjacentPiece = adjacentCell.CurrentPiece;

                if (adjacentPiece && !adjacentPiece.IsOfSameType(piece) && adjacentPiece.Interactable)
                    return true;
            }
        }

        return false;
    }
    #endregion

    #region Group Movement

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

            bool consumesOtherPiece = AnyHitOpposingPiece();

            foreach (GridPiece piece in activeGrouping)
            {
                piece.CanPlaceOnIndicator = consumesOtherPiece;
            }
        }
        else
        {
            DisplayInvalidGrouping();



            // override the normal GridPiece Indicator handling 
            activelyHeldPiece.ResetIndicator();//IndicatorCell = activelyHeldPiece.CurrentCell;// activeIndicatorCell;
            //activelyHeldPiece.CanPlaceOnIndicator = false;
            //activelyHeldPiece.ShowIndicator(false);
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

            // overlaps with piece that cannot be consumed, i.e. "wall"
            if(piece.IndicatorCell.Occupied && !piece.IndicatorCell.CurrentPiece.Interactable)
            {
                return false;
            }
        }
        return hit;
    }

    private bool HitsOpposingPiece(GridPiece piece)
    {
        Cell cell = piece.IndicatorCell;
        // hovering over piece          && is opposing type            && it's a consumable piece
        return cell.Occupied && !cell.CurrentPiece.IsOfSameType(piece) && cell.CurrentPiece.Interactable;
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
    #endregion

    private void HandlePieceHovered(GridPiece hoveredPiece, bool hovered)
    {        
        List<GridPiece> pieces = GetAdjacentPieces(hoveredPiece);

        foreach (GridPiece piece in pieces)
        {
            piece.HandleHover(hovered);
        }
    }
}