using FinishOne.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSaveController : MonoBehaviour
{
    [SerializeField] private FinishOne.UI.Button newGameButton, continueButton, levelButton;

    AdjacentGridGameData gameData;

    private void Start()
    {
        SaveSystem.Instance.LoadGame();

        gameData = SaveSystem.Instance.Data as AdjacentGridGameData;

        bool noData = gameData.LevelData == null ||
                      gameData.LevelData.Index == 0;

        if (noData)
        {
            continueButton.Interactable = false;
        }
        else
        {
            bool allLevelsBeat = gameData.LevelData.AllLevelsComplete;
            continueButton.Interactable = !allLevelsBeat;
        }
    }

    public Button NewGameButton => newGameButton.GetComponent<Button>();

    public void StartNewGame()
    {
        SaveSystem.Instance.NewGame();
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