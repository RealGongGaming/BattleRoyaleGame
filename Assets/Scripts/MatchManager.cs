using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private List<string> pickOrder = new();

    void Awake() => instance = this;

    void Start()
    {
        players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p == null) continue; 

            PlayerData data = System.Array.Find(DataManager.instance.players, d => d.playerID == p.gameObject.name);
            if (data != null)
                playerWins[p.gameObject.name] = data.roundWins;
        }
        state = MatchState.Playing;
    }

    void Update()
    {
        if (state != MatchState.Playing) return;

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

        pickOrder.Clear();

        if (winner != null) pickOrder.Add(winner.gameObject.name);

        players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p != winner) pickOrder.Add(p.gameObject.name);
        }

        // Start the timed sequence
        StartCoroutine(RoundEndSequence(winner));
    }

    private IEnumerator RoundEndSequence(PlayerStats winner)
    {
        // 1. Show the "Winner" UI (Round results)
        if (UIManager.instance != null)
            UIManager.instance.ShowEndRound(winner, playerWins);

        // 2. WAIT for 2 seconds so players can see who won
        yield return new WaitForSeconds(2f);

        // 3. Check if anyone has won the whole match yet
        if (playerWins.Values.Any(w => w >= roundsToWin) == false)
        {
            state = MatchState.DraftPhase;

            // 4. Create the Pick Order List
            List<string> pickOrder = new List<string>();

            // Winner always picks first
            if (winner != null)
            {
                pickOrder.Add(winner.gameObject.name);
            }

            // Add all other players (the losers) to the list
            // Note: You can sort 'players' by death time if you track that, 
            // but this adds all remaining players to the queue.
            foreach (var p in players)
            {
                if (p != winner)
                {
                    pickOrder.Add(p.gameObject.name);
                }
            }

            // 5. Tell CardManager to start the sequence
            CardManager cardManager = FindFirstObjectByType<CardManager>();
            if (cardManager != null)
            {
                // We pass the list of names to the CardManager 
                // so it knows who picks in what order.
                cardManager.StartDrafting(pickOrder);
            }
        }
        else
        {
            state = MatchState.MatchFinished;
            Debug.Log("Match Over! Final winner determined.");
            // Logic for returning to Main Menu or showing Final Stats goes here
        }
    }

    public void StartNextRound()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}