using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuSaveController : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;

    private void Start()
    {
        SaveSystem.Instance.LoadGame();

        buttonText.text = SaveSystem.Instance.gameData == null ? "Play" : "Continue";
    }

    public void StartNewGame()
    {
        if(SaveSystem.Instance.gameData == null || SaveSystem.Instance.gameData.Name.Equals(string.Empty))
        {
            SaveSystem.Instance.NewGame();
        }
    }

    private void OnApplicationQuit()
    {
        if(SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
    }
}