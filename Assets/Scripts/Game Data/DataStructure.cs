using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int score;
    public bool isCompleted;
    public bool isUnlocked;
}


[System.Serializable]
public class GameData
{
    public List<LevelData> levels = new List<LevelData>();
}