using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
public class LobbyController : MonoBehaviour
{
    public bool isReady = false;
    public bool canCycleWeapon = true;
    public TextMeshProUGUI ReadyText;
    public WeaponSelector WeaponSelectorP;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (context.canceled || (input.x < 0.6f && input.x > -0.6f))
        {
            canCycleWeapon = true;
            return;
        }

        if (!context.performed) return;

        if (canCycleWeapon && !isReady)
        {
            if (input.x >= 0.6f)
            {
                CycleWeaponForward();
                canCycleWeapon = false;
            }
            else if (input.x <= -0.6f)
            {
                CycleWeaponBackward();
                canCycleWeapon = false;
            }
        }
    }

    void CycleWeaponForward()
    {
        switch (WeaponSelectorP.currentWeapon)
        {
            case WeaponType.Hammer:
                WeaponSelectorP.currentWeapon = WeaponType.Polearm;
                break;
            case WeaponType.Polearm:
                WeaponSelectorP.currentWeapon = WeaponType.SwordAndShield;
                break;
            case WeaponType.SwordAndShield:
                WeaponSelectorP.currentWeapon = WeaponType.Dualsword;
                break;
            case WeaponType.Dualsword:
                WeaponSelectorP.currentWeapon = WeaponType.Hammer;
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
                break;
            case WeaponType.Polearm:
                WeaponSelectorP.currentWeapon = WeaponType.Hammer;
                break;
            case WeaponType.SwordAndShield:
                WeaponSelectorP.currentWeapon = WeaponType.Polearm;
                break;
            case WeaponType.Dualsword:
                WeaponSelectorP.currentWeapon = WeaponType.SwordAndShield;
                break;
        }
        WeaponSelectorP.ApplyWeapon();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isReady = !isReady;

        canCycleWeapon = !isReady;

        DataManager.instance.SetPlayerData(
            gameObject.name,
            isReady,
            WeaponSelectorP.currentWeapon
        );

        // UI feedback
        ReadyText.text = isReady ? "Ready" : "Not Ready";
        ReadyText.color = isReady ? Color.green : Color.red;
    }
}
