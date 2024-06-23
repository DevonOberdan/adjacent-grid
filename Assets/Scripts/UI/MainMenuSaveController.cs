using FinishOne.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSaveController : MonoBehaviour
{
    [SerializeField] private Button newGameButton, continueButton, levelButton;

    private void Start()
    {
        SaveSystem.Instance.LoadGame();

        bool noData = SaveSystem.Instance.gameData.LevelData == null ||
                      SaveSystem.Instance.gameData.LevelData.Index == 0;

        if (noData)
        {
            continueButton.interactable = false;
            levelButton.interactable = false;
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