using FinishOne.GeneralUtilities;
using FinishOne.SaveSystem;
using System.Linq;
using UnityEngine;

public interface ISaveable
{
    string Id { get; set; }
}

public interface IBind<TData> where TData: ISaveable
{
    string Id { get; set; }
    void Bind(TData data);
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private IntGameEvent LoadedIndexBroadcast;
    [SerializeField] public GameData gameData;
    
    private IDataService dataService;

    private void Awake()
    {
        Instance = this;

        dataService = new FileDataService(new JsonSerializer());
    }

    private void OnDestroy()
    {
        Instance = null;
        SaveGame();
    }

    public void BindData()
    {
        gameData.LevelData = Bind<GridLevelManager, LevelData>(gameData.LevelData);
    }

    private TData Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
        if (entity != null)
        {
            if (data == null)
            {
                data = new TData { Id = entity.Id };
            }

            entity.Bind(data);

            return data;
        }

        return default;
    }

    public void NewGame()
    {
        gameData = new GameData("Game");
    }

    public void SaveGame()
    {
        gameData.NewGame = false;
        dataService.Save(gameData);
    }

    public void LoadGame(string name = "Game")
    {
        gameData = dataService.Load(name);
    }

    public void ReloadGame() => LoadGame(gameData.Name);
    public void DeleteGame(string name) => dataService.Delete(name);
}