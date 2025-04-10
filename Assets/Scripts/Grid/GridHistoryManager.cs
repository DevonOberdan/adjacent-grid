using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridHistoryManager : MonoBehaviour
{
    [SerializeField] public UnityEvent<bool> HasHistoryBroadcast;
    [SerializeField] private UnityEvent<int> RemainingRewindBroadcast;

    [Tooltip("Set to 0 for unlimited attempts")]
    [SerializeField] private int maxRewindCount;

    [SerializeField] private bool rewindable = true;

    private List<GridPiece[]> gridHistory = new();
    private GridManager gridManager;

    private int rewindsRemaining;
    private bool rewindFlag;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();

        gridManager.OnGridChanged += () => RecordPiecePlacement();
        gridManager.OnGridReset += ResetHistory;
    }

    public int HistoryCount => gridHistory.Count;
    public void DisableRewindable(bool disable) => SetRewindable(!disable);
    public void SetRewindable(bool allow) => rewindable = allow;
    public void SetFlag(bool set) => rewindFlag = set;

    public void ResetHistory()
    {
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

        CheckForHistory();

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

        CheckForHistory();

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

    private void CheckForHistory()
    {
        bool hasHistory = gridHistory.Count > 1;
        if (maxRewindCount > 0)
            hasHistory = hasHistory && rewindsRemaining > 0;

        HasHistoryBroadcast.Invoke(hasHistory && rewindable);
    }

    private void OnValidate()
    {
        maxRewindCount = Mathf.Max(maxRewindCount, 0);
    }
}