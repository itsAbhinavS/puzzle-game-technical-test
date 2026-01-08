using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    #region Singleton
    private static SaveSystem _instance;
    public static SaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SaveSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveSystem");
                    _instance = go.AddComponent<SaveSystem>();
                }
            }
            return _instance;
        }
    }
    #endregion Singleton


    private string filePath;
    private GameData gameData;
    private const int TOTAL_LEVEL = 9;
    

    private void Awake()
    {
        #region Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion Singleton

        filePath = Application.persistentDataPath + "/gamedata.json";
        LoadGame();
    }


    #region Save And Load Function
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
    }
    public void LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Loading data from: " + filePath);
        }
        else
        {
            // Initialize new game data if no save file exists
            gameData = new GameData();
            InitializeDataSetup();
            Debug.Log("No save file found. Creating new game data.");
        }
    }
    #endregion Save And Load Function


    #region Set Function
    private void InitializeDataSetup()
    {
        for (int i = 1; i <= TOTAL_LEVEL; i++)
        {
            LevelData level = new LevelData
            {
                levelNumber = i,
                score = 0,
                isCompleted = false,
                isUnlocked = (i == 1) // Only first level is unlocked
            };
            gameData.levels.Add(level);
        }
    }

    public void CompletedLevel(int levelNumber, int newScore)
    {
        LevelData level = gameData.levels.Find(l => l.levelNumber == levelNumber);
        if (level != null && level.isUnlocked)
        {
            // Update current level
            level.score = Mathf.Max(level.score, newScore); // Keep highest score
            level.isCompleted = true;

            // Unlock next level
            LevelData nextLevel = gameData.levels.Find(l => l.levelNumber == levelNumber + 1);
            if (nextLevel != null)
            {
                nextLevel.isUnlocked = true;
            }

            SaveGame();
            Debug.Log($"Level {levelNumber} completed! Score: {newScore}");
        }
    }
    #endregion Set Function


    #region Get Functions
    public bool IsLevelUnlocked(int levelNumber)
    {
        LevelData level = gameData.levels.Find(l => l.levelNumber == levelNumber);
        return level != null && level.isUnlocked;
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        LevelData level = gameData.levels.Find(l => l.levelNumber == levelNumber);
        return level != null && level.isCompleted;
    }

    public int GetLevelScore(int levelNumber)
    {
        LevelData level = gameData.levels.Find(l => l.levelNumber == levelNumber);
        return level != null ? level.score : 0;
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return gameData.levels.Find(l => l.levelNumber == levelNumber);
    }

    public List<LevelData> GetAllLevels()
    {
        return gameData.levels;
    }

    public int GetTotalScore()
    {
        int total = 0;
        foreach (LevelData level in gameData.levels)
        {
            total += level.score;
        }
        return total;
    }
    public int GetTotalLevel() 
    {
        return TOTAL_LEVEL;
    }
    #endregion Get Functions


    // Use this with caution
    public void ResetGameData()
    {
        if(gameData != null) gameData.levels.Clear();
        InitializeDataSetup();
        SaveGame();
        Debug.Log("Game data reset");
    }
}
