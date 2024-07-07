using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class GameUISaveController : MonoBehaviour
{
    [SerializeField] private GameEvent EnterLevelSelectEvent, ExitLevelSelectEvent;
    [SerializeField] private GameObject tutorialTextRoot;
    [SerializeField] private FinishOne.UI.Button levelSelectCloseButton;

    private void Start()
    {
        if (SaveSystem.Instance.Data == null)
        {
            SaveSystem.Instance.NewGame();
        }

        AdjacentGridGameData gameData = SaveSystem.Instance.Data as AdjacentGridGameData;

        bool hasPlayed = gameData.LevelData != null && gameData.LevelData.Index > 0;

        if (hasPlayed)
        {
            tutorialTextRoot.SetActive(false);
        }

        HandleLevelSelectSetup();
    }

    private void HandleLevelSelectSetup()
    {
        if (GameManager.StartInLevelSelect)
        {
            EnterLevelSelectEvent.Raise();
            levelSelectCloseButton.gameObject.SetActive(false);
        }
        else
        {
            ExitLevelSelectEvent.Raise();
        }

        GameManager.StartInLevelSelect = false;
    }
}