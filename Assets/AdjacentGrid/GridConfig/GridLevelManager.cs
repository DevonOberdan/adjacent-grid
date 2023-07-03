using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridLevelManager : MonoBehaviour
{
    [SerializeField] GridManager gridManager;

    [SerializeField]
    GridPuzzleConfigSO[] puzzleConfigs;

    [SerializeField] UnityEvent OnNewLevel;
    [SerializeField] UnityEvent OnWonGame;

    [SerializeField] bool startingCustom;

    [Space]
    [Header("Config Generation")]

    [SerializeField] string newPuzzleName;

    int levelIndex;

    public GridManager GridManager => gridManager;
    public string NewPuzzleName => newPuzzleName;

    bool LastLevelComplete => levelIndex == puzzleConfigs.Length;


    int LevelIndex 
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
                OnNewLevel.Invoke();
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
}