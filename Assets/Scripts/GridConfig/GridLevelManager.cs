using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class LevelData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }

    public int Index;
    public bool AllLevelsComplete;
}

public class GridLevelManager : MonoBehaviour, IBind<LevelData>
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private bool startingCustom;
    [SerializeField] private GridPuzzleConfigSO[] puzzleConfigs;

    public UnityEvent<int> OnNewLevel;
    [SerializeField] private UnityEvent OnWonGame;
    [SerializeField] private GameEvent OnRequestNext, OnRequestRestart, OnRequestDecrement;

    private GameEventClassListener nextListener, restartListener, decremenetListener;

    public int CompletedLevelCount { get; private set; } = 0;

    [SerializeField] private TMP_Text levelText;

    [Space]
    [Header("Config Generation")]

    [SerializeField] private string newPuzzleName;

    public GridManager GridManager => gridManager;
    public string NewPuzzleName => newPuzzleName;
    public int PuzzleCount => puzzleConfigs.Length;
    private bool LastLevelComplete => levelIndex == puzzleConfigs.Length;

    private const string LEVEL_TEXT = "Level ";

    private int levelIndex;

    public int LevelIndex
    {
        get => levelIndex;
        private set
        {
            levelIndex = Mathf.Clamp(value, 0, puzzleConfigs.Length);

            if (LastLevelComplete)
            {
                OnWonGame.Invoke();
                data.AllLevelsComplete = true;
            }
            else
            {
                gridManager.PuzzleConfig = puzzleConfigs[levelIndex];
                OnNewLevel.Invoke(levelIndex);
            }
        }
    }

    #region Save Data

    [Header("Save Data")]
    [SerializeField] private LevelData data;
    [field: SerializeField] public string Id { get; set; } = "LevelManager";

    public void Bind(LevelData data)
    {
        this.data = data;
        this.data.Id = Id;

        if (!startingCustom)
        {
            CompletedLevelCount = data.Index;
            LevelIndex = Mathf.Clamp(CompletedLevelCount, 0, puzzleConfigs.Length-1);
        }
    }
    #endregion

    private void Awake()
    {
        nextListener = new GameEventClassListener(Increment);
        decremenetListener = new GameEventClassListener(Decrement);
        restartListener = new GameEventClassListener(ResetCurrentLevel);

        OnRequestNext.RegisterListener(nextListener);
        OnRequestDecrement.RegisterListener(decremenetListener);
        OnRequestRestart.RegisterListener(restartListener);
    }

    private void Start()
    {
        if (startingCustom)
            levelIndex = -1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Decrement();
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            CheckNewLevelCompletion();
            Increment();
        }
    }

    private void OnDestroy()
    {
        OnRequestNext.UnregisterListener(nextListener);
        OnRequestDecrement.UnregisterListener(decremenetListener);
        OnRequestRestart.UnregisterListener(restartListener);

        nextListener = null;
        decremenetListener = null;
        restartListener = null;

        if(SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
    }

    public void Decrement() => LevelIndex--;
    public void Increment() => LevelIndex++;

    public void SetLevelIndex(int index) => LevelIndex = index;

    public void ResetCurrentLevel() => LevelIndex = levelIndex;

    public void CheckNewLevelCompletion()
    {
        if (levelIndex == CompletedLevelCount)
        {
            CompletedLevelCount++;
            data.Index = CompletedLevelCount;

            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.SaveGame();
            }
        }
    }

    public void SetLevelText(int level) => levelText.text = LEVEL_TEXT + $"{level + 1}/{puzzleConfigs.Length}";
}