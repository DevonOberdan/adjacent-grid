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

/// <summary>
/// Hooks into GridManager events in order to provide this game with its unique grouping mechanic
/// 
/// When a piece is picked up, this determines all cardinally-adjacent pieces, extended out from the
/// grabbed piece, and makes them mimic the grabbed piece behaviour.
/// </summary>
public class AdjacentGridGameManager : MonoBehaviour
{
    [SerializeField] private bool highlightGroup;
    [SerializeField] private UnityEvent OnLevelComplete;
    [SerializeField] private UnityEvent<bool> OnDoomed;

    private GridManager gridManager;
    private List<GridPiece> activeGrouping;
    private GridPiece activelyHeldPiece;
    private bool wasDoomed;

    #region Public Properties
    private bool WinCondition => gridManager.PieceCount == 1;

    public bool AboutToWin => gridManager.PieceCount == 2 && AnyValidMovements();
    public bool Doomed => wasDoomed;
    public List<PieceGroup> CurrentGroups { get; private set; }

    public bool GroupPickedUp => activeGrouping != null && activeGrouping.Count > 0;
    public bool IgnoreGridChange { get; private set; }

    public IEnumerable<GridPiece> NonGrabbedGroupMembers => activeGrouping.Where(p => p != activelyHeldPiece);
    #endregion

    private void Awake()
    {
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

    private void HandlePieceHovered(GridPiece hoveredPiece, bool hovered)
    {
        List<GridPiece> pieces = GetAdjacentPieces(hoveredPiece);

        foreach (GridPiece piece in pieces)
        {
            piece.HandleHover(hovered);
        }
    }

    #region Grouping Functions

    public void PickupGroupedPieces(GridPiece piece)
    {
        List<GridPiece> groupedPieces = GetAdjacentPieces(piece);

        activeGrouping = groupedPieces;
        activelyHeldPiece = piece;

        foreach (GridPiece groupedPiece in groupedPieces)
        {
            groupedPiece.HandlePickup();
        }
    }

    private void FindAllGroups()
    {
        CurrentGroups = new();

        List<GridPiece> checkedPieces = new();
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
        groupedPieces ??= new();

        groupedPieces.Add(piece);
        Cell currentPieceCell = piece.CurrentCell;

        foreach (Cell adjacentCell in currentPieceCell.AdjacentCells)
        {
            if (adjacentCell == null || groupedPieces.Contains(adjacentCell.CurrentPiece))
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
        //foreach (GridPiece piece in activeGrouping)
        //{
        //    if (piece != activelyHeldPiece && !nonActivelyHeldPieceOffsets.ContainsKey(piece))
        //    {
        //        nonActivelyHeldPieceOffsets.Add(piece, piece.CurrentCell.IndexInGrid - activelyHeldPiece.CurrentCell.IndexInGrid);
        //    }
        //}
    }
    #endregion

    #region Handle Dropped
    public void HandlePieceDropped(GridPiece droppedPiece, bool actuallyDropped)
    {
        PlaceGroupedPieces(actuallyDropped);

        if (actuallyDropped && WinCondition)
            OnLevelComplete.Invoke();
    }

    public void PlaceGroupedPieces(bool drop)
    {
        foreach (GridPiece piece in NonGrabbedGroupMembers)
        {
            piece.CanPlaceOnIndicator = drop;
            piece.PlaceOnIndicator();
        }

       // nonActivelyHeldPieceOffsets.Clear();
        activeGrouping.Clear();
        activelyHeldPiece = null;

        gridManager.OnGridChanged?.Invoke();
    }

    #endregion

    #region Doomed
    public void SetIgnoreGridChange(bool ignore)
    {
        IgnoreGridChange = ignore;
    }

    private void HandleGridChanged()
    {
        if(!IgnoreGridChange)
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
                if(adjacentCell == null)
                    continue;

                GridPiece adjacentPiece = adjacentCell.CurrentPiece;

                if (adjacentPiece && !adjacentPiece.IsOfSameType(piece) && adjacentPiece.Consumable)
                    return true;
            }
        }

        return false;
    }
    #endregion

    #region Group Movement

    public void MoveGroupIndicators(Cell activeIndicatorCell, bool show=true)
    {
        foreach (GridPiece piece in NonGrabbedGroupMembers)
        {
            int newIndex = activelyHeldPiece.CurrentCell.AdjacentCells.IndexOf(activeIndicatorCell);

            if (newIndex >= 0)
            {
                piece.IndicatorCell = piece.CurrentCell.AdjacentCells[newIndex];
            }
        }

        foreach (GridPiece piece in activeGrouping)
        {
            piece.CanPlaceOnIndicator = true;
            piece.ShowIndicator(show);
        }
    }

    public void ShowGroupIndicators(bool show)
    {
        foreach (GridPiece piece in activeGrouping)
        {
            piece.ShowIndicator(show);
        }
    }

    private void MoveGroupedPieces(Cell activeIndicatorCell)
    {
        bool validMovement = ValidateMovement(activeIndicatorCell);

        if (validMovement)
        {
            foreach (GridPiece piece in NonGrabbedGroupMembers)
            {
                int newIndex = activelyHeldPiece.CurrentCell.AdjacentCells.IndexOf(activeIndicatorCell);
                piece.IndicatorCell = newIndex >= 0 ? piece.CurrentCell.AdjacentCells[newIndex] : piece.CurrentCell;
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

            // override the normal GridPiece Indicator handling, and mark edge pieces as invalid
            activelyHeldPiece.ResetIndicator();

            foreach (GridPiece piece in NonGrabbedGroupMembers)
            {
                if (!piece.CanPlaceOnIndicator)
                    piece.MarkIndicatorCellInvalid();
            }
        }
    }

    private bool ValidateMovement(Cell activeIndicatorCell)
    {
        if(activeIndicatorCell == null)
            return false;

        bool allValid = true;

        foreach (GridPiece piece in NonGrabbedGroupMembers)
        {
            int newIndex = activelyHeldPiece.CurrentCell.AdjacentCells.IndexOf(activeIndicatorCell);

            if (newIndex >=0 && piece.CurrentCell.AdjacentCells[newIndex] != null)
            {
                piece.IndicatorCell = piece.CurrentCell;
            }
            // Grouped piece out of bounds
            else
            {
                piece.MarkIndicatorCellInvalid();
                allValid = false;
            }
        }

        return allValid;
    }

    public bool GroupStaysInGrid(Cell activePieceCell)
    {
        foreach (GridPiece piece in NonGrabbedGroupMembers)
        {
            int newIndex = activelyHeldPiece.CurrentCell.AdjacentCells.IndexOf(activePieceCell);

            if (newIndex >= 0 && piece.CurrentCell.AdjacentCells[newIndex] != null)
            {
                return false;
            }
        }

        return true;
    }

    public bool GroupCanLand(Cell activePieceCell)
    {
        foreach (GridPiece piece in NonGrabbedGroupMembers)
        {
            int newIndex = activelyHeldPiece.CurrentCell.AdjacentCells.IndexOf(activePieceCell);

            if (newIndex >= 0 && piece.CurrentCell.AdjacentCells[newIndex] != null)
            {
                return false;
            }

            if (!PieceCanLand(piece, piece.CurrentCell.AdjacentCells[newIndex]))
            {
                return false;
            }
        }

        return true;
    }

    public bool PieceCanLand(GridPiece piece, Cell cell)
    {
        GridPiece cellPiece = cell.CurrentPiece;

        // new cell has blocking piece OR (a piece of the same type that is not in the current group)
        if (cellPiece != null && (cellPiece.Consumable == false || (cellPiece.IsOfSameType(piece) && !activeGrouping.Contains(cellPiece))))
        {
            return false;
        }

        return true;
    }

    public void PlaceGroupFromActiveCell(Cell activePieceDestination)
    {
        foreach (GridPiece piece in NonGrabbedGroupMembers)
        {
            int newIndex = activelyHeldPiece.CurrentCell.AdjacentCells.IndexOf(activePieceDestination);

            if (newIndex == -1 || piece.CurrentCell.AdjacentCells[newIndex] == null)
            {
                piece.PlaceOnCell(piece.CurrentCell);
            }
            else
            {
                piece.PlaceOnCell(piece.CurrentCell.AdjacentCells[newIndex]);
            }
        }

        activeGrouping.Clear();
        activelyHeldPiece = null;

        gridManager.OnGridChanged?.Invoke();
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
            if(piece.IndicatorCell.Occupied && !piece.IndicatorCell.CurrentPiece.Consumable)
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
        return cell && cell.Occupied && !cell.CurrentPiece.IsOfSameType(piece) && cell.CurrentPiece.Consumable;
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
}