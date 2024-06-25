using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using UnityEngine;

public class GameUISaveController : MonoBehaviour
{
    [SerializeField] private GameEvent EnterLevelSelectEvent, ExitLevelSelectEvent;
    [SerializeField] private GameObject tutorialTextRoot;

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

    private  void HandleLevelSelectSetup()
    {
        if (GameManager.StartInLevelSelect)
        {
            EnterLevelSelectEvent.Raise();
        }
        else
        {
            ExitLevelSelectEvent.Raise();
        }

        GameManager.StartInLevelSelect = false;
    }
}