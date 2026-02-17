using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject roundEndPanel;
    public TMPro.TextMeshProUGUI winnerText;
    public TMPro.TextMeshProUGUI scoreBoard;
    public GameObject backToTitleButton;

    void Awake()
    {
        instance = this;
    }

    public void ShowEndRound(PlayerStats winner,
                             Dictionary<string, int> scores)
    {
        Debug.Log("ShowingRoundEnd");
        roundEndPanel.SetActive(true);


        var matchWinner = scores.FirstOrDefault(kvp => kvp.Value >= 3);

        if (matchWinner.Value >= 3)
        {
            winnerText.text = matchWinner.Key + " Wins The Match!";
            backToTitleButton.SetActive(true);
        }
        else
        {
            
            winnerText.text = winner.name + "'s Round Victory!";
        }



        scoreBoard.text = string.Join("\n", scores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }

    public void ReturnToTitle()
    {
        Destroy(DataManager.instance.gameObject);  
        SceneManager.LoadScene(0);
    }
}