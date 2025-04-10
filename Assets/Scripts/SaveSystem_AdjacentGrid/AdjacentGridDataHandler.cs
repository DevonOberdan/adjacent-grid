using FinishOne.SaveSystem;
using UnityEngine;

public class AdjacentGridDataHandler : SaveDataHandler
{
    [SerializeField] public AdjacentGridGameData gameData;

    public override GameData Data  => gameData;

    public override string ITCH_PATH => "idbfs/adjacent-grid_sgwhf94hgfw/";

    public override void NewData(string name)
    {
        gameData = new AdjacentGridGameData(name);
    }

    public override void LoadData(string name, IDataService dataService)
    {
        gameData = dataService.Load<AdjacentGridGameData>(name);
    }

    public override void BindData()
    {
        gameData.LevelData = Bind<GridLevelManager, LevelData>(gameData.LevelData);
    }
}