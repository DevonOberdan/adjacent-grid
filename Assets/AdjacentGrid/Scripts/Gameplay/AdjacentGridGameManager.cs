using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AdjacentGridGameManager : MonoBehaviour
{
    [SerializeField] bool debug;

    [SerializeField] UnityEvent OnLevelComplete;

    GridManager gridManager;

    List<GridPiece> activeGrouping;
    GridPiece activelyHeldPiece;

    Dictionary<GridPiece, int> nonActivelyHeldPieceOffsets;
    bool levelComplete;

    bool WinCondition => gridManager.ActivePieces == 1;


    private void Awake()
    {
        nonActivelyHeldPieceOffsets = new Dictionary<GridPiece, int>();
        activeGrouping = new List<GridPiece>();

        gridManager = GetComponent<GridManager>();
    }

    void Start()
    {
        gridManager.OnPiecePickedUp += FindGroupedPieces;

        gridManager.OnPieceDropped += (piece) => PlaceGroupedPieces();
        gridManager.OnPieceIndicatorMoved += MoveGroupedPieces;
        //gridManager.OnPointerLeftGrid += HandleGridExit;
    }

    private void Update()
    {
        if(WinCondition)
        {
            //levelComplete = true;
            OnLevelComplete.Invoke();
        }
    }

    public void HandleGridExit()
    {
        print("exit grid");
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
            activelyHeldPiece.PlaceOnIndicator();
            gridManager.OnPieceDropped?.Invoke(activelyHeldPiece);
        }
    }


    void MoveGroupedPieces(Cell activeIndicatorCell)
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
            activelyHeldPiece.IndicatorCell = activelyHeldPiece.CurrentCell;
        }
    }
    
    bool ValidateMovement(Cell activeIndicatorCell)
    {
        bool allValid = true;

        foreach (GridPiece piece in nonActivelyHeldPieceOffsets.Keys)
        {
            int newIndicatorIndex = activeIndicatorCell.IndexInGrid + nonActivelyHeldPieceOffsets[piece];

                // Grouped piece out of bounds
            if (!gridManager.Cells.IsValidIndex(newIndicatorIndex))
            {
                print("OUT OF GRID INDEX!");
                ShowAsInvalid();
                allValid = false;
            }   // Grouped piece trying to move "out" of the grid left or right
            else if (!(gridManager.Cells[newIndicatorIndex] == piece.CurrentCell || piece.CurrentCell.AdjacentCells.Contains(gridManager.Cells[newIndicatorIndex])))
            {
                print(newIndicatorIndex);
                print(piece.name+" CANT MOVE LEFT OR RIGHT!");

                ShowAsInvalid();
                allValid = false;
            }
            else if (!gridManager.PointerInGrid)
            {

            }

            void ShowAsInvalid()
            {
                piece.CanPlaceOnIndicator = false;
                piece.IndicatorCell = piece.CurrentCell;
                piece.ShowIndicator(true);
            }
        }
        //print("even getting to this?");
        //if (!gridManager.PointerInGrid && allValid)//&& !gridManager.HoveredOverCell)
        //{
        //    print("reset indicators");
        //    foreach (GridPiece piece in activeGrouping)
        //    {
        //        piece.IndicatorCell = piece.CurrentCell;
        //    }
        //    //activelyHeldPiece.PlaceOnIndicator();
        //    //gridManager.OnPieceDropped?.Invoke(activelyHeldPiece);
        //    // print("not in grid");
        //    allValid = false;
        //}



        return allValid;
    }

    void ValidatePlacement()
    {
        bool validPlacement = HitsOpposingPiece() || debug;


        foreach (GridPiece piece in activeGrouping)
        {
            piece.CanPlaceOnIndicator = validPlacement;
        }

    }

    bool HitsOpposingPiece()
    {
        bool hit = false;

        foreach (GridPiece piece in activeGrouping)
        {
            // hovering over piece          && is opposing type
            if (piece.IndicatorCell.Occupied && !piece.IndicatorCell.CurrentPiece.IsOfSameType(piece))
            {
                hit = true;
            }
        }
        return hit;
    }

    void PlaceGroupedPieces()
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


    List<GridPiece> GetAdjacentPieces(List<GridPiece> groupedPieces, GridPiece piece)
    {
        groupedPieces.Add(piece);
        Cell currentPieceCell = piece.CurrentCell;

        foreach(Cell adjacentCell in currentPieceCell.AdjacentCells)
        {
            if (groupedPieces.Contains(adjacentCell.CurrentPiece))
                continue;

            if (adjacentCell.Occupied && adjacentCell.CurrentPiece.IsOfSameType(piece))
                groupedPieces = GetAdjacentPieces(groupedPieces, adjacentCell.CurrentPiece);
        }

        return groupedPieces;
    }

    void ProcessGroupedOffsets()
    {
        foreach(GridPiece piece in activeGrouping)
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