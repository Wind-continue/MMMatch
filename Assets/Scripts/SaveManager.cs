using System;
using UnityEngine;

public class SaveManager
{
    private const string KEY_PLAYER_DATA = "PlayerData";
    private const string KEY_LEVEL_PREFIX = "Level_";

    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveManager();
            }
            return _instance;
        }
    }

    private PlayerData _playerData;
    public PlayerData PlayerData
    {
        get
        {
            if (_playerData == null)
            {
                LoadPlayerData();
            }
            return _playerData;
        }
    }

    private SaveManager()
    {
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        string json = PlayerPrefs.GetString(KEY_PLAYER_DATA, "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                _playerData = JsonUtility.FromJson<PlayerData>(json);
                if (_playerData != null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SaveManager: Failed to parse PlayerData, resetting. Error: " + e.Message);
            }
        }

        _playerData = new PlayerData();
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        if (_playerData == null)
        {
            _playerData = new PlayerData();
        }
        string json = JsonUtility.ToJson(_playerData);
        PlayerPrefs.SetString(KEY_PLAYER_DATA, json);
        PlayerPrefs.Save();
    }

    public void SetNickname(string name)
    {
        PlayerData.nickname = name;
        SavePlayerData();
    }

    public void AddCoins(int amount)
    {
        PlayerData.coins += amount;
        if (PlayerData.coins < 0)
            PlayerData.coins = 0;
        SavePlayerData();
    }

    public bool SpendCoins(int amount)
    {
        if (PlayerData.coins < amount)
            return false;
        PlayerData.coins -= amount;
        SavePlayerData();
        return true;
    }

    public void SetCurrentLevel(int level)
    {
        PlayerData.currentLevel = level;
        SavePlayerData();
    }

    public void UnlockNextLevel()
    {
        int nextLevel = PlayerData.currentLevel + 1;
        if (nextLevel > PlayerData.maxUnlockedLevel)
        {
            PlayerData.maxUnlockedLevel = nextLevel;
            SavePlayerData();
        }
    }

    public bool IsLevelUnlocked(int level)
    {
        return level <= PlayerData.maxUnlockedLevel;
    }

    public void SaveLevelProgress(int level, int score, bool completed)
    {
        string key = KEY_LEVEL_PREFIX + level;
        int existingScore = GetLevelScore(level);
        if (score > existingScore)
        {
            string json = JsonUtility.ToJson(new LevelProgressData(score, completed));
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        else if (completed && !IsLevelCompleted(level))
        {
            string json = JsonUtility.ToJson(new LevelProgressData(existingScore, completed));
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
    }

    public int GetLevelScore(int level)
    {
        string json = PlayerPrefs.GetString(KEY_LEVEL_PREFIX + level, "");
        if (!string.IsNullOrEmpty(json))
        {
            LevelProgressData data = JsonUtility.FromJson<LevelProgressData>(json);
            if (data != null)
                return data.score;
        }
        return 0;
    }

    public bool IsLevelCompleted(int level)
    {
        string json = PlayerPrefs.GetString(KEY_LEVEL_PREFIX + level, "");
        if (!string.IsNullOrEmpty(json))
        {
            LevelProgressData data = JsonUtility.FromJson<LevelProgressData>(json);
            if (data != null)
                return data.completed;
        }
        return false;
    }

    public void ClearAllData()
    {
        PlayerPrefs.DeleteKey(KEY_PLAYER_DATA);
        int level = 1;
        while (PlayerPrefs.HasKey(KEY_LEVEL_PREFIX + level))
        {
            PlayerPrefs.DeleteKey(KEY_LEVEL_PREFIX + level);
            level++;
        }
        PlayerPrefs.Save();
        _playerData = new PlayerData();
    }

    [Serializable]
    private class LevelProgressData
    {
        public int score;
        public bool completed;

        public LevelProgressData(int score, bool completed)
        {
            this.score = score;
            this.completed = completed;
        }
    }
}