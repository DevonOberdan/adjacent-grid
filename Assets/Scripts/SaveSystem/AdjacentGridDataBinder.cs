using FinishOne.SaveSystem;
using UnityEngine;

public class AdjacentGridDataBinder : SaveDataBinder
{
    [SerializeField] public AdjacentGridGameData gameData;

    public override GameData Data { get => gameData; set => gameData = new AdjacentGridGameData(value?.Name); }


    public override void BindData()
    {
        gameData.LevelData = Bind<GridLevelManager, LevelData>(gameData.LevelData);
    }
}