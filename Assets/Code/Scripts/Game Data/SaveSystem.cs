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



    #region Access Variables (Get Only)
    public static string FilePath { get; private set; }
    public static GameData gameData { get; private set; }
    
    public static readonly int TotalLevel = 9;
    public static int CurrentLevel { get; private set; }

    #endregion Access Variables (Get Only)



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

        FilePath = Application.persistentDataPath + "gamedata.json";

        LoadGame();
    }


    #region Save And Load Function
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(FilePath, json);
    }
    public void LoadGame()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Loading data from: " + FilePath);
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
        for (int i = 1; i <= TotalLevel; i++)
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

        SaveGame();
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
    
    public void SetCurrentLevel(int level) => CurrentLevel = level;
    
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
