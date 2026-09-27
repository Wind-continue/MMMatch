using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string nickname;
    public int coins;
    public int currentLevel;
    public int maxUnlockedLevel;

    public PlayerData()
    {
        nickname = "Player";
        coins = 0;
        currentLevel = 1;
        maxUnlockedLevel = 1;
    }
}