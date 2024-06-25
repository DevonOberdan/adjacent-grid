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

    public abstract class SaveDataBinder : MonoBehaviour
    {
        public abstract GameData Data { get; set; }
        
        public abstract void BindData();

        public TData Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
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
    }

    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        public GameData Data => dataBinder.Data;

        private IDataService dataService;
        private SaveDataBinder dataBinder;
        private bool SavingAllowed;

        private const string ITCH_PATH = "idbfs/adjacent-grid-game_sgwhf94hgfw/";

        private void Awake()
        {
            Instance = this;
            dataBinder = GetComponent<SaveDataBinder>();
#if UNITY_WEBGL && !UNITY_EDITOR
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

        public void NewGame(bool allowSaving = true)
        {
            dataBinder.Data = new GameData("Game");
            SavingAllowed = allowSaving;

            SaveGame();
        }

        public void SaveGame()
        {
            if (SavingAllowed && dataBinder.Data != null)
            {
                dataService.Save(dataBinder.Data);
            }
        }

        public void LoadGame(string name = "Game")
        {
            dataBinder.Data = dataService.Load(name);

            if (dataBinder.Data == null)
            {
                NewGame();
            }

            SavingAllowed = true;
        }

        public void ReloadGame() => LoadGame(dataBinder.Data.Name);
        public void DeleteGame(string name) => dataService.Delete(name);
    }
}