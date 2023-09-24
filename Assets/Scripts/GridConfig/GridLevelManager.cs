using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GridLevelManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private bool startingCustom;

    [SerializeField] private GridPuzzleConfigSO[] puzzleConfigs;

    [SerializeField] private UnityEvent<int> OnNewLevel;
    [SerializeField] private UnityEvent OnWonGame;


    [SerializeField] private TMP_Text levelText;

    [Space]
    [Header("Config Generation")]

    [SerializeField] private string newPuzzleName;

    private int levelIndex;

    public GridManager GridManager => gridManager;
    public string NewPuzzleName => newPuzzleName;

    private bool LastLevelComplete => levelIndex == puzzleConfigs.Length;

    private const string LEVEL_TEXT = "Level ";

    private int LevelIndex
    {
        get => levelIndex;
        set
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
        if (startingCustom)
            levelIndex = -1;
        else
            LevelIndex = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Decrement();
        else if (Input.GetKeyUp(KeyCode.M))
            Increment();
    }

    public void Decrement() => LevelIndex--;
    public void Increment() => LevelIndex++;

    public void SetLevelText(int level) => levelText.text = LEVEL_TEXT + $"{level + 1}/{puzzleConfigs.Length}";
}