using System;

[Serializable]
public class GameData
{
    public string Name;
    public LevelData LevelData;
    public GameData(string name)
    {
        this.Name = name;
    }
}