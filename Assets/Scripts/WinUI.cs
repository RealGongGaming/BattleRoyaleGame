using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject roundEndPanel;
    public TMPro.TextMeshProUGUI winnerText;

    void Awake()
    {
        instance = this;
    }

    public void ShowEndRound(PlayerStats winner,
                             Dictionary<PlayerStats, int> scores)
    {
        roundEndPanel.SetActive(true);
        winnerText.text = winner ?
            winner.name + " wins the round!" :
            "Draw!";
    }
}