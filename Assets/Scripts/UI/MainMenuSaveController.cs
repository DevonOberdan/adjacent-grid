using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSaveController : MonoBehaviour
{
    [SerializeField] private Button newGameButton, continueButton, levelButton;
   // private TMP_Text newText, continueText
    private void Awake()
    {
        
    }

    private void Start()
    {
        SaveSystem.Instance.LoadGame();

        bool noData = SaveSystem.Instance.gameData == null;

        if (noData)
        {
            continueButton.interactable = false;
        }
        else
        {
            bool allLevelsBeat = SaveSystem.Instance.gameData.LevelData.AllLevelsComplete;
            continueButton.interactable = !allLevelsBeat;
        }
    }

    public void StartNewGame()
    {
        GameManager.StartInLevelSelect = false;

        SaveSystem.Instance.NewGame();
    }

    public void ContinueGame()
    {
        GameManager.StartInLevelSelect = false;
    }

    public void HandleLevelSelect()
    {
        GameManager.StartInLevelSelect = true;
    }

    private void OnApplicationQuit()
    {
        if(SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
    }
}