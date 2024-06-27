using System;
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

    [Serializable]
    public class GameData
    {
        public string Name;

        public GameData(string name)
        {
            this.Name = name;
        }
    }

    public abstract class SaveDataHandler : MonoBehaviour
    {
        public abstract GameData Data { get; }

        //idbfs/adjacent-grid-game_sgwhf94hgfw/
        public abstract string ITCH_PATH { get; }

        public abstract void NewData(string name);
        public abstract void LoadData(string name, IDataService dataService);
        public abstract void BindData();

        public virtual IDataService GetDataService()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new FileDataService(new JsonSerializer(), ITCH_PATH);
#else
            return new FileDataService(new JsonSerializer());
#endif
        }

        public TData Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            T entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();

            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
                return data;
            }

            return default;
        }
    }
}