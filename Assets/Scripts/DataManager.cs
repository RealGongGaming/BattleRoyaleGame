using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private Dictionary<string, bool> readyPlayer = new Dictionary<string, bool>();
    public Dictionary<string, bool> ReadyPlayers
    {
        get => readyPlayer;
        set => readyPlayer = value;
    }

    public static DataManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerReady(string playerID, bool isReady)
    {
        readyPlayer[playerID] = isReady;
    }

    public void RemovePlayer(string playerID)
    {
        if (readyPlayer.ContainsKey(playerID))
            readyPlayer.Remove(playerID);
    }

    public bool IsPlayerReady(string playerID)
    {
        return readyPlayer.ContainsKey(playerID) && readyPlayer[playerID];
    }
}
