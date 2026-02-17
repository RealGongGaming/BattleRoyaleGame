using System.Collections;
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
    public GameObject VictoryPlayer;
    public WeaponSelector VictoryWeaponSelector;

    void Awake()
    {
        instance = this;
    }

    public IEnumerator ShowEndRound(PlayerStats winner,
                             Dictionary<string, int> scores)
    {
        var tags = FindObjectsByType<PlayerNameTag>(FindObjectsSortMode.None);
        foreach (var tag in tags)
        {
            tag.visibleAlpha = 0;
            tag.occludedAlpha = 0;
        }
        var matchWinner = scores.FirstOrDefault(kvp => kvp.Value >= 3);
        if (matchWinner.Value >= 3)
        {
            winnerText.text = matchWinner.Key + " Wins The Match!";
            var allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
            var winnerPlayer = allPlayers.FirstOrDefault(p => p.name == matchWinner.Key);
            if (winnerPlayer != null)
            {
                var winnerWeapon = winnerPlayer.GetComponent<WeaponSelector>();
                if (winnerWeapon != null)
                {
                    VictoryWeaponSelector.currentWeapon = winnerWeapon.currentWeapon;
                }
            }
            VictoryPlayer.SetActive(true);
            yield return new WaitForSeconds(1);
            winnerText.text = matchWinner.Key + " Wins The Match!";
            roundEndPanel.SetActive(true);
            backToTitleButton.SetActive(true);
            VictoryPlayer.transform.localPosition = new Vector3(0, 40, -460);
        }
        else
        {
            roundEndPanel.SetActive(true);
            winnerText.text = winner.name + "'s Round Victory!";
            yield return null;
        }
        scoreBoard.text = string.Join("\n", scores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }

    public void ReturnToTitle()
    {
        Destroy(DataManager.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        SceneManager.LoadScene(0);
    }
}