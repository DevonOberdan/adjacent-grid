using FinishOne.SaveSystem;
using System;

[Serializable]
public class AdjacentGridGameData : GameData
{
    public LevelData LevelData;

    public AdjacentGridGameData(string name) : base(name) { }
}