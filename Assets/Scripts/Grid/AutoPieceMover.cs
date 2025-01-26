using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AutoPieceMover : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> OnMovingChanged;
    [SerializeField] private float movementYieldTime;
    [SerializeField] private float endDelayTime = 0.5f;

    [SerializeField] private bool showIndicators;

    public enum DIR { UP, RIGHT, DOWN, LEFT };

    private GridManager gridManager;
    private GridHistoryManager historyManager;
    private AdjacentGridGameManager adjacentManager;

    private const float MOVE_DELAY = 0.2f;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        historyManager = GetComponent<GridHistoryManager>();
        adjacentManager = GetComponent<AdjacentGridGameManager>();
    }

    private void OnEnable()
    {
        OnMovingChanged.AddListener(adjacentManager.SetIgnoreGridChange);
        OnMovingChanged.AddListener(historyManager.SetFlag);
    }

    private void OnDisable()
    {
        OnMovingChanged.RemoveListener(adjacentManager.SetIgnoreGridChange);
        OnMovingChanged.RemoveListener(historyManager.SetFlag);
    }

    public void MovePieceAllTheWay(GridPiece piece, DIR direction)
    {
        StartCoroutine(MoveToEnd(piece, direction));
    }

    private IEnumerator MoveToEnd(GridPiece piece, DIR direction)
    {
        OnMovingChanged.Invoke(true);

        gridManager.PickedUpPiece(piece);

        yield return new WaitForSeconds(MOVE_DELAY);

        Cell nextCell = piece.CurrentCell.AdjacentCells[(int)direction];
        adjacentManager.PickupGroupedPieces(piece);

        yield return null;

        while (nextCell != null && adjacentManager.GroupStaysInGrid(nextCell))
        {
            OnMovingChanged.Invoke(true);
            yield return new WaitForSeconds(movementYieldTime);

            piece.IndicatorCell = nextCell;
            piece.ShowIndicator(false);

            adjacentManager.MoveGroupIndicators(nextCell, showIndicators);
            nextCell = nextCell.AdjacentCells[(int)direction];

            yield return new WaitForSeconds(movementYieldTime);
        }

        Cell finalCell = piece.IndicatorCell;
        DIR oppositeDir = GetOppositeDir(direction);

        // see if we end up trying to land on pieces we cant land on.. go back until we can
        while (finalCell != null && finalCell != piece.CurrentCell && (!adjacentManager.PieceCanLand(piece, finalCell) || !adjacentManager.GroupCanLand(finalCell)))
        {
            OnMovingChanged.Invoke(true);
            piece.IndicatorCell = finalCell;
            piece.ShowIndicator(false);

            adjacentManager.MoveGroupIndicators(finalCell, showIndicators);

            finalCell = finalCell.AdjacentCells[(int)oppositeDir];
        }

        piece.IndicatorCell = finalCell;
        piece.ShowIndicator(false);

        adjacentManager.MoveGroupIndicators(finalCell, true);
        piece.ShowIndicator(true);

        yield return new WaitForSeconds(endDelayTime);

        OnMovingChanged.Invoke(false);
        piece.UserDropPiece();
    }

    private DIR GetOppositeDir(DIR dir)
    {
        if (dir == DIR.UP)
            return DIR.DOWN;
        else if (dir == DIR.DOWN)
            return DIR.UP;
        else if (dir == DIR.LEFT)
            return DIR.RIGHT;

        return DIR.LEFT;
    }
}