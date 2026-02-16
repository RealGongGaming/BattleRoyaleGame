using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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

    public int roundsToWin = 3; // best of five
    public MatchState state;

    private Dictionary<PlayerStats, int> wins = new();
    private PlayerStats[] players;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);

        foreach (var p in players)
            wins[p] = 0;

        state = MatchState.Playing;
        Debug.Log("MatchStart");
    }

    void Update()
    {
        if (state == MatchState.MatchFinished) return;

        var alivePlayers = players.Where(p => p.currentHP > 0).ToArray();

        if (alivePlayers.Length <= 1)
        {
            state  = MatchState.RoundEnd;
            EndRound(alivePlayers.FirstOrDefault());
        }
    }


    void EndRound(PlayerStats winner)
    {
        if (winner != null)
            wins[winner]++;

        UIManager.instance.ShowEndRound(winner, wins);

        if (wins.Values.Any(w => w >= roundsToWin ) == false)
        {
            state = MatchState.DraftPhase;

        }

    }




}
