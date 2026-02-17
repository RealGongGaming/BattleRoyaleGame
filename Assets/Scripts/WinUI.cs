using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject roundEndPanel;
    public TMPro.TextMeshProUGUI winnerText;
    public TMPro.TextMeshProUGUI scoreBoard;

    void Awake()
    {
        instance = this;
    }

    public void ShowEndRound(PlayerStats winner,
                             Dictionary<string, int> scores)
    {
        Debug.Log("ShowingRoundEnd");
        roundEndPanel.SetActive(true);
        winnerText.text = winner.name + "'s Victory!";
        scoreBoard.text = string.Join("\n", scores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }
}