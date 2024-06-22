using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FinishOne.SaveSystem
{
    public class FileDataService : IDataService
    {
        private ISerializer serializer;
        private string dataPath;
        private string fileExtension;

        public FileDataService(ISerializer serializer, string path = "")
        {
            this.dataPath = path.Equals(string.Empty) ? Application.persistentDataPath : path;
            this.fileExtension = "json";
            this.serializer = serializer;

            if (!Directory.Exists(this.dataPath))
            {
                Directory.CreateDirectory(this.dataPath);
                Debug.Log("Created new directory: "+this.dataPath);
            }
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void Save(GameData data, bool overwrite = true)
        {
            string filePath = GetFilePath(data.Name);

            if(!overwrite && File.Exists(filePath))
            {
                throw new IOException($"The file '{data.Name}.{fileExtension}' already exists and cannot be overridden.");
            }

            File.WriteAllText(filePath, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string filePath = GetFilePath(name);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(filePath));
        }

        public void Delete(string name)
        {
            string filePath = GetFilePath(name);

            if (!File.Exists(filePath))
            {
                throw new IOException($"The file '{name}.{fileExtension}' does not exist.");
            }

            File.Delete(filePath);
        }

        public void DeleteAll()
        {
            string[] files = Directory.GetFiles(dataPath);

            foreach(string file in files)
            {
                File.Delete(file);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach(string path in Directory.EnumerateFiles(dataPath))
            {
                if(Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}