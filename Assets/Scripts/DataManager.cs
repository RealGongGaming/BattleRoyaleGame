using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public string playerID;
    public bool isReady;
    public WeaponType weaponType;
    public int roundWins;

    public float hpMultiplier = 1f;
    public float moveSpeedMultiplier = 1f;
    public float attackMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float attackRangeMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public float knockbackResistBonus = 0f;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public PlayerData[] players = new PlayerData[4];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < players.Length; i++)
                players[i] = new PlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerData(string id, bool ready, WeaponType weapon)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerID == id || string.IsNullOrEmpty(players[i].playerID))
            {
                players[i].playerID = id;
                players[i].isReady = ready;
                players[i].weaponType = weapon;

                PrintAllPlayers(); 
                return;
            }
        }

        PrintAllPlayers(); 
    }
    //for debug
    private void PrintAllPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            Debug.Log(
                $"Slot {i} | ID: {players[i].playerID} | Ready: {players[i].isReady} | Weapon: {players[i].weaponType}"
            );
        }
    }


    public void RemovePlayer(string id)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerID == id)
            {
                players[i] = new PlayerData();
                return;
            }
        }
    }

    public int ReadyCount()
    {
        int count = 0;
        foreach (var p in players)
            if (p != null && p.isReady)
                count++;

        return count;
    }
}