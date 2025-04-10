using System.Collections.Generic;

namespace FinishOne.SaveSystem
{
    public interface IDataService
    {
        void Save<T>(T data, bool overwrite = true) where T : GameData;
        T Load<T>(string name) where T : GameData;
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}