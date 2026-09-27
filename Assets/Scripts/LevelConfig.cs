using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    public int step;
    public List<TargetConfig> target;

    [Serializable]
    public class TargetConfig
    {
        public int type;
        public int amount;
    }

    [Serializable]
    private class LevelsData
    {
        public List<LevelConfig> levels;
    }

    private static readonly string[] ConfigRelativePaths = new string[]
    {
        "/configs/levels.json",
        "/Scripts/configs/levels.json"
    };

    private static LevelsData cachedLevelsData;

    public static LevelConfig Load(int level = 1)
    {
        LevelsData data = LoadLevelsData();
        if (data != null && data.levels != null && data.levels.Count > 0)
        {
            int index = Mathf.Clamp(level - 1, 0, data.levels.Count - 1);
            LevelConfig config = data.levels[index];
            if (config != null && config.target != null && config.target.Count > 0)
            {
                return config;
            }
        }

        return GetDefault(level);
    }

    private static LevelsData LoadLevelsData()
    {
        if (cachedLevelsData != null)
            return cachedLevelsData;

        string json = null;
        foreach (string relativePath in ConfigRelativePaths)
        {
            string path = Application.dataPath + relativePath;
            if (File.Exists(path))
            {
                try
                {
                    json = File.ReadAllText(path);
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to read levels.json at " + path + ": " + e.Message);
                }
            }
        }

        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                LevelsData data = JsonUtility.FromJson<LevelsData>(json);
                if (data != null && data.levels != null && data.levels.Count > 0)
                {
                    cachedLevelsData = data;
                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse levels.json: " + e.Message);
            }
        }

        Debug.LogWarning("Levels config not found or invalid, using defaults.");
        return null;
    }

    public static void ClearCache()
    {
        cachedLevelsData = null;
    }

    private static LevelConfig GetDefault(int level)
    {
        int types = level < 9 ? 2 : (level < 23 ? 3 : 4);
        int baseAmount = Mathf.Min(5 + level / 3, 10);
        int step = Mathf.Max(25 - level / 2, 15);

        var targets = new List<TargetConfig>();
        for (int i = 0; i < types; i++)
        {
            targets.Add(new TargetConfig { type = i, amount = baseAmount });
        }

        return new LevelConfig
        {
            step = step,
            target = targets
        };
    }
}