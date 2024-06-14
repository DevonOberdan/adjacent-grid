using FinishOne.GeneralUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GridLevelManager : MonoBehaviour
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

    private int levelIndex;

    public GridManager GridManager => gridManager;
    public string NewPuzzleName => newPuzzleName;
    public int PuzzleCount => puzzleConfigs.Length;

    private bool LastLevelComplete => levelIndex == puzzleConfigs.Length;

    private const string LEVEL_TEXT = "Level ";

    public int LevelIndex
    {
        get => levelIndex;
        private set
        {
            levelIndex = Mathf.Clamp(value, 0, puzzleConfigs.Length);

            if (LastLevelComplete)
            {
                OnWonGame.Invoke();
            }
            else
            {
                gridManager.PuzzleConfig = puzzleConfigs[levelIndex];
                OnNewLevel.Invoke(levelIndex);
            }
        }
    }

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
        else
            Invoke(nameof(StartGame), 0.1f);
    }

    private void StartGame()
    {
        LevelIndex = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Decrement();
        else if (Input.GetKeyUp(KeyCode.M))
            Increment();
    }

    private void OnDestroy()
    {
        OnRequestNext.UnregisterListener(nextListener);
        OnRequestDecrement.UnregisterListener(decremenetListener);
        OnRequestRestart.UnregisterListener(restartListener);

        nextListener = null;
        decremenetListener = null;
        restartListener = null;
    }

    public void Decrement() => LevelIndex--;
    public void Increment() => LevelIndex++;

    public void SetLevelIndex(int index) => LevelIndex = index;

    public void ResetCurrentLevel() => LevelIndex = levelIndex;

    public void NewLevelBeaten() => CompletedLevelCount++;

    public void SetLevelText(int level) => levelText.text = LEVEL_TEXT + $"{level + 1}/{puzzleConfigs.Length}";
}