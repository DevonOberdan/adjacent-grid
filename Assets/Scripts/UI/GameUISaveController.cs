using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using UnityEngine;

public class GameUISaveController : MonoBehaviour
{
    [SerializeField] private GameEvent EnterLevelSelectEvent, ExitLevelSelectEvent;
    [SerializeField] private GameObject tutorialTextRoot;

    private bool levelSelectSetup;

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

    public void HandleLevelSelectSetup()
    {
       // if (levelSelectSetup)
       //     return;

        //levelSelectSetup = true;

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