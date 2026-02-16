using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsPopup : MonoBehaviour
{
    [Header("Popup")]
    public GameObject popupPanel;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void TogglePopup()
    {
        if (popupPanel == null) return;
        bool show = !popupPanel.activeSelf;
        popupPanel.SetActive(show);
    }

    public void ClosePopup()
    {
        if (popupPanel == null) return;
        popupPanel.SetActive(false);
    }
}