using System.Linq;
using UnityEngine;

namespace FinishOne.SaveSystem
{
    public interface ISaveable
    {
        string Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        string Id { get; set; }
        void Bind(TData data);
    }

    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        [SerializeField] public GameData gameData;

        private IDataService dataService;
        private bool SavingAllowed;

        private const string ITCH_PATH = "idbfs/adjacent-grid-game_sgwhf94hgfw/";

        private void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            dataService = new FileDataService(new JsonSerializer());
#elif UNITY_WEBGL
            dataService = new FileDataService(new JsonSerializer(), ITCH_PATH);
#else
            dataService = new FileDataService(new JsonSerializer());
#endif
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
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
                return data;
            }

            return default;
        }

        public void NewGame(bool allowSaving = true)
        {
            gameData = new GameData("Game");
            SavingAllowed = allowSaving;

            SaveGame();
        }

        public void SaveGame()
        {
            if (SavingAllowed && gameData != null)
            {
                dataService.Save(gameData);
            }
        }

        public void LoadGame(string name = "Game")
        {
            gameData = dataService.Load(name);

            if (gameData == null)
            {
                NewGame();
            }

            SavingAllowed = true;
        }

        public void ReloadGame() => LoadGame(gameData.Name);
        public void DeleteGame(string name) => dataService.Delete(name);
    }
}