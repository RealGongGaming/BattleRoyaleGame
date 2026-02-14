using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
public class LobbyController : MonoBehaviour
{
    private Vector2 IsCharacterCycling;
    public bool isReady = false;
    public TextMeshProUGUI ReadyText;
    public TextMeshProUGUI WeaponText;
    public WeaponSelector WeaponSelectorP;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.x > 0.5f)
            CycleWeaponForward();
        else if (input.x < -0.5f)
            CycleWeaponBackward();
    }

    void CycleWeaponForward()
    {
        switch (WeaponSelectorP.currentWeapon)
        {
            case WeaponType.Hammer:
                WeaponSelectorP.currentWeapon = WeaponType.Polearm;
                WeaponText.text = "Polearm";
                break;
            case WeaponType.Polearm:
                WeaponSelectorP.currentWeapon = WeaponType.SwordAndShield;
                WeaponText.text = "Sword & Shield";
                break;
            case WeaponType.SwordAndShield:
                WeaponSelectorP.currentWeapon = WeaponType.Dualsword;
                WeaponText.text = "Dual Sword";
                break;
            case WeaponType.Dualsword:
                WeaponSelectorP.currentWeapon = WeaponType.Hammer;
                WeaponText.text = "Hammer";
                break;
        }

        WeaponSelectorP.ApplyWeapon();
    }

    void CycleWeaponBackward()
    {
        switch (WeaponSelectorP.currentWeapon)
        {
            case WeaponType.Hammer:
                WeaponSelectorP.currentWeapon = WeaponType.Dualsword;
                WeaponText.text = "Dual Sword";
                break;
            case WeaponType.Polearm:
                WeaponSelectorP.currentWeapon = WeaponType.Hammer;
                WeaponText.text = "Hammer";
                break;
            case WeaponType.SwordAndShield:
                WeaponSelectorP.currentWeapon = WeaponType.Polearm;
                WeaponText.text = "Polearm";
                break;
            case WeaponType.Dualsword:
                WeaponSelectorP.currentWeapon = WeaponType.SwordAndShield;
                WeaponText.text = "Sword & Shield";
                break;
        }

        WeaponSelectorP.ApplyWeapon();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isReady = !isReady;
            if (isReady)
            {
                ReadyText.text = "Ready";
                ReadyText.color = Color.green;
            }
            else
            {
                ReadyText.text = "Not Ready";
                ReadyText.color = Color.red;
            }
        }
    }
}
