using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UniqueSolutionBot : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    public Action<float> OnSetDelay;
    public Action<int> OnSetSolutionCount;
    public UnityEvent<bool> OnSetSolving;

    private AdjacentGridGameManager gameManager;
    private GridHistoryManager historyManager;
    private bool solving;
    private int uniqueCount;

    private static Dictionary<string, int> gridStateSolutionCountDict = new(); 

    public float MoveDelayTime { get; set; } = 0f;

    private YieldInstruction WaitForMove => MoveDelayTime == 0f ? null : new WaitForSeconds(MoveDelayTime);

    private void Awake()
    {
        gameManager = gridManager.GetComponent<AdjacentGridGameManager>();
        historyManager = gridManager.GetComponent<GridHistoryManager>();

        gridManager.OnGridReset += () => SetCount(gridManager.PuzzleConfig.SolutionCount);
    }

    public void SolveCurrentGrid()
    {
        if (solving)
        {
            StopAllCoroutines();
            gridManager.ResetToConfig();
            historyManager.ResetHistory();
            SetSolving(false);
        }
        else
        {
            StartCoroutine(SolvePuzzle());
        }
    }

    public IEnumerator SolvePuzzle()
    {
        SetSolving(true);
        SetCount(0);

        yield return WaitForMove;
        yield return StartCoroutine(CheckGroups());

        if (gridManager.GridSameAsConfig())
        {
            gridManager.PuzzleConfig.SetSolutionCount(uniqueCount);
        }

        SetSolving(false);
    }

    private IEnumerator CheckGroups()
    {
        if (gameManager.AboutToWin)
        {
            SetCount(uniqueCount+1);
            historyManager.RewindGrid();
            yield return WaitForMove;
            yield break;
        }
        else if (gameManager.Doomed)
        {
            historyManager.RewindGrid();
            yield return WaitForMove;
            yield break;
        }

        List<GridPiece> gridAsPieces = gridManager.Cells.Select(cell => cell.CurrentPiece).ToList();
        string hash = ComputeHash(gridAsPieces);

        if (gridStateSolutionCountDict.ContainsKey(hash))
        {
            SetCount(uniqueCount + gridStateSolutionCountDict[hash]);
            historyManager.RewindGrid();
            yield return WaitForMove;
            yield break;
        }

        int startingCount = uniqueCount;

        foreach (PieceGroup group in gameManager.CurrentGroups)
        {
            GridPiece drivingPiece = group.Group[0];

            if (drivingPiece == null || !drivingPiece.Interactable)
                continue;

            foreach (Cell cell in drivingPiece.CurrentCell.AdjacentCells)
            {
                gridManager.OnPiecePickedUp?.Invoke(drivingPiece);

                drivingPiece.IndicatorCell = cell;
                gridManager.OnPieceIndicatorMoved?.Invoke(drivingPiece.IndicatorCell);

                yield return WaitForMove;
                if (drivingPiece.UserDropPiece())
                {
                    // let events run, make new CurrentGroups
                    yield return WaitForMove;
                    yield return CheckGroups();
                }
            }
        }

        int subCount = uniqueCount - startingCount;
        gridStateSolutionCountDict[hash] = subCount;

        historyManager.RewindGrid();
        yield return WaitForMove;
    }

    static string ComputeHash(List<GridPiece> group)
    {
        var sb = new StringBuilder();
        foreach (var piece in group)
        {
            sb.Append(piece == null ? "_": piece.PieceColor.ToString()).Append(",");
        }

        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private void SetCount(int count)
    {
        uniqueCount = count;
        OnSetSolutionCount?.Invoke(uniqueCount);
    }

    private void SetSolving(bool enabled)
    {
        solving = enabled;
        if (Camera.main.TryGetComponent(out BaseRaycaster raycaster))
        {
            raycaster.enabled = !solving;
        }

        OnSetSolving.Invoke(solving);
    }
}