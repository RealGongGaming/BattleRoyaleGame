using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        else
        {
            StartText.text = "Player Ready: " + DataManager.instance.ReadyCount() + "/4." + " Need At Least 2 Players";
            StartText.color = Color.orange;
        }
    }
    public void Play(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (DataManager.instance.ReadyCount() >= 2)
        {
            PlayerInput[] allInputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
            foreach (var input in allInputs)
            {
                if (input != null)
                {
                    input.DeactivateInput();
                    input.enabled = false;
                }
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            AudioManager.instance.PlayBGM(AudioManager.instance.gameBGM);
        }
    }
}