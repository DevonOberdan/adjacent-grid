using FinishOne.GeneralUtilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridHistoryManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> HasHistoryBroadcast;

    [Tooltip("Set to 0 for unlimited attempts")]
    [SerializeField] private int maxRewindCount;

    [SerializeField] private UnityEvent<int> RemainingRewindBroadcast;

    private int rewindsRemaining = 0;

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

    public void ResetHistory()
    {
        if (gridHistory == null)
            gridHistory = new();
        else
            gridHistory.Clear();

        HasHistoryBroadcast.Invoke(false);
        rewindsRemaining = maxRewindCount;

        RemainingRewindBroadcast.Invoke(rewindsRemaining);
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

        bool hasHistory = gridHistory.Count > 1;
        if (maxRewindCount > 0)
            hasHistory = hasHistory && rewindsRemaining > 0;

        HasHistoryBroadcast.Invoke(hasHistory);

        return needToRecord;
    }

    public void Rewind() => RewindGrid();

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

        rewindsRemaining = Mathf.Max(rewindsRemaining-1, 0);

        //if limitRewinds is true, check that there are still rewinds
        bool hasHistory = gridHistory.Count > 1;
        if (maxRewindCount > 0)
            hasHistory = hasHistory && rewindsRemaining > 0;

        HasHistoryBroadcast.Invoke(hasHistory);

        RemainingRewindBroadcast.Invoke(rewindsRemaining);

        rewindFlag = true;
        gridManager.OnGridChanged?.Invoke();

        return true;
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

    private void OnValidate()
    {
        maxRewindCount = Mathf.Max(maxRewindCount, 0);
    }
}