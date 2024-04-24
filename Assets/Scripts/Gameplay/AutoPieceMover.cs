using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoPieceMover : MonoBehaviour
{
    private GridManager gridManager;
    private AdjacentGridGameManager adjacentManager;

    [SerializeField] private UnityEvent<bool> OnMovingChanged;
    [SerializeField] private float movementYieldTime;

    public enum DIR { UP, RIGHT, DOWN, LEFT };

    private float MOVE_DELAY = 0.2f;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        adjacentManager = GetComponent<AdjacentGridGameManager>();
    }

    public void MovePieceAllTheWay(GridPiece piece, DIR direction)
    {
        StartCoroutine(MoveToEnd(piece, direction));
    }

    private IEnumerator MoveToEnd(GridPiece piece, DIR direction)
    {
        OnMovingChanged.Invoke(true);

        yield return new WaitForSeconds(MOVE_DELAY);

        Cell nextCell = piece.CurrentCell.AdjacentCells[(int)direction];
        adjacentManager.PickupGroupedPieces(piece);


        yield return null;

        while(true)
        {
            OnMovingChanged.Invoke(true);
            yield return new WaitForSeconds(movementYieldTime);

            //if(!adjacentManager.GroupPickedUp)
            //    adjacentManager.PickupGroupedPieces(piece);

            yield return null;

            print(nextCell.gameObject.name);

            piece.PlaceOnCell(nextCell);
            adjacentManager.PlaceGroupFromActiveCell(nextCell);

            nextCell = nextCell.AdjacentCells[(int)direction];
            // yield return new WaitForSeconds(movementYieldTime);

            if (nextCell != null && adjacentManager.ValidMovement(nextCell))
                adjacentManager.PickupGroupedPieces(piece);
            else
                break;
            // adjacentManager.PickupGroupedPieces(piece);

            //adjacentManager.PickupGroupedPieces(piece);
        }

        piece.PlaceOnIndicator();
        adjacentManager.PlaceGroupFromActiveCell(piece.CurrentCell);

        //piece.PlaceOnCell(nextCell);
        //adjacentManager.PlaceGroupFromActiveCell(nextCell);

        OnMovingChanged.Invoke(false);
    }
}