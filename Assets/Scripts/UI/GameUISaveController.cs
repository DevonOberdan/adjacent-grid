using FinishOne.GeneralUtilities;
using System.Collections;
using System.Collections.Generic;
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

        if (!SaveSystem.Instance.gameData.NewGame)
        {
            tutorialTextRoot.SetActive(false);
            ExitLevelSelectEvent.Raise();
        }
        else
        {
            EnterLevelSelectEvent.Raise();
        }
    }
}