using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
    private PlayerStats[] players;
    private bool matchEnded = false;

    void Start()
    {
        players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (matchEnded) return;

        var alivePlayers = players.Where(p => p.currentHP > 0).ToArray();

        if (alivePlayers.Length <= 1)
        {
            matchEnded = true;
            EndMatch(alivePlayers.FirstOrDefault());
        }
    }

    void EndMatch(PlayerStats winner)
    {
        if (winner != null)
            Debug.Log("Winner: " + winner.name);
        else
            Debug.Log("Draw");

        Time.timeScale = 0f; // pause game
    }
}
