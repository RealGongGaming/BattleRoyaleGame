using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartBattle : MonoBehaviour
{
    public TextMeshProUGUI StartText;

    private void Update()
    {

        if (DataManager.instance.ReadyCount() >= 2)
        {
            StartText.text = "Player Ready: " + DataManager.instance.ReadyCount() + "/4." + " Press Enter To Start";
            StartText.color = Color.green;
        }
        else {
            StartText.text = "Player Ready: " + DataManager.instance.ReadyCount() + "/4." + " Need At Least 2 Players";
            StartText.color = Color.orange;
        }
    }
    public void Play()
    {
        if(DataManager.instance.ReadyCount() >= 2)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
