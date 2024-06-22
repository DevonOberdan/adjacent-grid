using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using UnityEngine;

public class GameUISaveController : MonoBehaviour
{
    [SerializeField] private GameEvent EnterLevelSelectEvent, ExitLevelSelectEvent;
    [SerializeField] private GameObject tutorialTextRoot;

    private void Start()
    {
        if (SaveSystem.Instance.gameData == null)
        {
            SaveSystem.Instance.NewGame();
        }

        if (!SaveSystem.Instance.gameData.NewGame)
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