using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UniqueSolutionBot : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private AdjacentGridGameManager gameManager;
    private GridHistoryManager historyManager;

    [Header("UI References")]
    [SerializeField] private Slider delaySlider;
    [SerializeField] private Button solveButton;
    [SerializeField] private TMP_Text uniqueSolutionText, delayTimeText;

    private float MOVE_DELAY = 0.005f;
    private bool solving;
    private int uniqueCount;

    private void Awake()
    {
        gameManager = gridManager.GetComponent<AdjacentGridGameManager>();
        historyManager = gridManager.GetComponent<GridHistoryManager>();

        gridManager.OnGridReset += HandleGridReset;

        solveButton.onClick.AddListener(HandleButton);
        delaySlider.onValueChanged.AddListener(SetTimeDelay);
    }

    public void SolveSlowly()
    {
        SetTimeDelay(1.75f);
        SolveNewPuzzle();
    }

    private void HandleButton()
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
            SolveNewPuzzle();
        }
    }

    private void HandleGridReset()
    {
        SetCount(gridManager.PuzzleConfig.SolutionCount);
    }

    private void SetCount(int count)
    {
        uniqueCount = count;
        uniqueSolutionText.text = "" + uniqueCount;
    }

    private void SetSolving(bool enabled)
    {
        solving = enabled;
        solveButton.GetComponentInChildren<TMP_Text>().text = enabled ? "Reset" : "Find Solutions";
    }

    private void SetTimeDelay(float value)
    {
        delayTimeText.text = "" + value;

        if (!float.TryParse(delayTimeText.text, out MOVE_DELAY))
        {
            MOVE_DELAY = 0.05f;
        }
    }

    private void SolveNewPuzzle()
    {
        SetCount(0);
        SetSolving(true);
        StartCoroutine(SolvePuzzle());
    }

    private IEnumerator SolvePuzzle()
    {
        yield return new WaitForSeconds(MOVE_DELAY);
        yield return StartCoroutine(CheckGroups());
        uniqueSolutionText.text = "" + uniqueCount;

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
            yield return new WaitForSeconds(MOVE_DELAY);
            yield break;
        }
        else if (gameManager.Doomed)
        {
            historyManager.RewindGrid();
            yield return new WaitForSeconds(MOVE_DELAY);
            yield break;
        }

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

                yield return new WaitForSeconds(MOVE_DELAY);
                if (drivingPiece.UserDropPiece())
                {
                    // let events run, make new CurrentGroups
                    yield return new WaitForSeconds(MOVE_DELAY);
                    yield return StartCoroutine(CheckGroups());
                }
            }
        }

        historyManager.RewindGrid();
        yield return new WaitForSeconds(MOVE_DELAY);
    }
}