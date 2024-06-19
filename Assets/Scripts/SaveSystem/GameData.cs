using System;

[Serializable]
public class GameData
{
    public string Name;
    public bool NewGame;
    public LevelData LevelData;
    public GameData(string name)
    {
        this.Name = name;
        NewGame = true;
    }
}