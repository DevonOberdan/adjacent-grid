using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using UnityEngine;

public class GameUISaveController : MonoBehaviour
{
    [SerializeField] private GameEvent EnterLevelSelectEvent, ExitLevelSelectEvent;
    [SerializeField] private GameObject tutorialTextRoot;

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (SaveSystem.Instance.gameData == null)
        {
            SaveSystem.Instance.NewGame();
        }

        if (GameManager.StartInLevelSelect)
        {
            EnterLevelSelectEvent.Raise();
            GameManager.StartInLevelSelect = false;
        }
        else
        {
            ExitLevelSelectEvent.Raise();
        }

        if (!SaveSystem.Instance.gameData.NewGame)
        {
            tutorialTextRoot.SetActive(false);
        }
    }
}