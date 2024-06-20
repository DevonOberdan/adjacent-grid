using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FinishOne.SaveSystem
{
    public class ItchDataService : IDataService
    {
        private ISerializer serializer;
        private string dataPath;
        private string fileExtension;

        public ItchDataService(ISerializer serializer)
        {
            this.dataPath = "idbfs/adjacent-grid-game_sgwhf94hgfw";
            this.fileExtension = "json";
            this.serializer = serializer;

            if(!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void Save(GameData data, bool overwrite = true)
        {
            throw new System.NotImplementedException();
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

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}