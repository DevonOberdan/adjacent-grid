using UnityEngine;

namespace FinishOne.SaveSystem
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        public GameData Data => gameDataHandler.Data;

        private SaveDataHandler gameDataHandler;
        private IDataService dataService;
        private bool SavingAllowed;

        private void Awake()
        {
            Instance = this;
            gameDataHandler = GetComponent<SaveDataHandler>();
            dataService = gameDataHandler.GetDataService();
        }

        private void OnDestroy()
        {
            Instance = null;
            SaveGame();
        }

        public void NewGame(bool allowSaving = true)
        {
            gameDataHandler.NewData("Game");
            SavingAllowed = allowSaving;

            SaveGame();
        }

        public void SaveGame()
        {
            if (SavingAllowed && gameDataHandler.Data != null)
            {
                dataService.Save(gameDataHandler.Data);
            }
        }

        public void LoadGame(string name = "Game")
        {
            gameDataHandler.LoadData(name, dataService);

            if (gameDataHandler.Data == null)
            {
                NewGame();
            }

            SavingAllowed = true;
        }

        public void ReloadGame() => LoadGame(gameDataHandler.Data.Name);
        public void DeleteGame(string name) => dataService.Delete(name);
    }
}