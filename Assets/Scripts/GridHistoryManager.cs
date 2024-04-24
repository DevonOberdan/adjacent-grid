using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHistoryManager : MonoBehaviour
{
    [SerializeField] private Button historyButton;
    [SerializeField] private bool rewindable = true;

    private List<GridPiece[]> gridHistory;
    private GridManager gridManager;

    private bool rewindFlag;

    private void Awake()
    {
        ResetHistory();

        gridManager = GetComponent<GridManager>();

        gridManager.OnGridChanged += () => RecordPiecePlacement();
        gridManager.OnGridReset += ResetHistory;
    }

    private void Start()
    {
        historyButton.onClick.AddListener(() => RewindGrid());
    }

    public void DisableRewindable(bool disable) => SetRewindable(!disable);
    public void SetRewindable(bool allow)
    {
        rewindable = allow;

        if (historyButton != null)
            historyButton.interactable = gridHistory.Count > 1 && rewindable;
    }

    public void SetFlag(bool set)
    {
        rewindFlag = set;
    }

    public void ResetHistory()
    {
        if (gridHistory == null)
            gridHistory = new();
        else
            gridHistory.Clear();

        if (historyButton != null)
            historyButton.interactable = false;
    }

    public bool RecordPiecePlacement()
    {
        if (rewindFlag)
        {
            rewindFlag = false;
            return false;
        }

        // add current configuration to list of lists
        GridPiece[] currentCells = new GridPiece[gridManager.Cells.Count];

        for (int i = 0; i < gridManager.Cells.Count; i++)
        {
            currentCells[i] = gridManager.Cells[i].CurrentPiece;
        }

        bool needToRecord = !SameAsLastState(currentCells);

        if (needToRecord)
        {
            gridHistory.Add(currentCells);
        }

        if (historyButton != null)
            historyButton.interactable = gridHistory.Count > 1 && rewindable;

        return needToRecord;
    }

    private bool SameAsLastState(GridPiece[] newPieceConfig)
    {
        if (gridHistory.Count == 0)
            return false;

        for (int i = 0; i < gridHistory[^1].Length; i++)
        {
            if (newPieceConfig[i] != gridHistory[^1][i])
                return false;
        }

        return true;
    }

    public bool RewindGrid()
    {
        if (gridHistory.Count <= 1)
            return false;

        gridHistory.RemoveAt(gridHistory.Count - 1);

        GridPiece[] previousGridConfig = gridHistory[^1];

        for (int i = 0; i < gridManager.Cells.Count; i++)
        {
            if (previousGridConfig[i] != null)
            {
                gridManager.AddPiece(previousGridConfig[i]);
                previousGridConfig[i].CurrentCell = gridManager.Cells[i];
                previousGridConfig[i].IndicatorCell = previousGridConfig[i].CurrentCell;
                previousGridConfig[i].ShowIndicator(false);
            }
        }

        if (historyButton != null)
            historyButton.interactable = gridHistory.Count > 1;

        rewindFlag = true;

        gridManager.OnGridChanged?.Invoke();

        return true;
    }
}