using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public enum MatchState
{
    Playing,
    RoundEnd,
    DraftPhase,
    Resetting,
    MatchFinished
}

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    public int roundsToWin = 3;
    public MatchState state;

    private Dictionary<string, int> playerWins = new();
    private PlayerStats[] players;

    void Awake() => instance = this;

    void Start()
    {
        players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p == null) continue; // Skip destroyed objects

            PlayerData data = System.Array.Find(DataManager.instance.players, d => d.playerID == p.gameObject.name);
            if (data != null)
                playerWins[p.gameObject.name] = data.roundWins;
        }
        state = MatchState.Playing;
    }

    void Update()
    {
        if (state != MatchState.Playing) return;

        // Filter out null AND check currentHP
        var alivePlayers = players.Where(p => p != null && p.currentHP > 0).ToArray();

        Debug.Log("playing: alive " + alivePlayers.Length);

        if (alivePlayers.Length <= 1)
        {
            state = MatchState.RoundEnd;
            EndRound(alivePlayers.FirstOrDefault());
        }
    }

    void EndRound(PlayerStats winner)
    {
        if (winner != null)
        {
            string winnerName = winner.gameObject.name;

            playerWins[winnerName]++;

            PlayerData data = System.Array.Find(DataManager.instance.players, d => d.playerID == winnerName);
            if (data != null) data.roundWins = playerWins[winnerName];
        }

        if (UIManager.instance != null)
            UIManager.instance.ShowEndRound(winner, playerWins);

        if (playerWins.Values.Any(w => w >= roundsToWin) == false)
        {
            state = MatchState.DraftPhase;
            FindFirstObjectByType<CardUIManager>().ShowCardSelection();
        }
        else
        {
            state = MatchState.MatchFinished;
            Debug.Log("Game Over! Match Winner determined.");
        }
    }

    public void StartNextRound()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}