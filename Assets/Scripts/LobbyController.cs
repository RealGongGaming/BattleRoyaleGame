using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
public class LobbyController : MonoBehaviour
{
    public bool isReady = false;
    public TextMeshProUGUI ReadyText;
    public TextMeshProUGUI WeaponText;
    public bool canCycleWeapon = true;
    public WeaponSelector WeaponSelectorP;


    public void OnMove(InputAction.CallbackContext context)
    {
        
        
        if (!context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();
        if (canCycleWeapon)
        {
            if (input.x > 0.5f)
                CycleWeaponForward();
            else if (input.x < -0.5f)
                CycleWeaponBackward();
        }
    }

    void CycleWeaponForward()
    {
        switch (WeaponSelectorP.currentWeapon)
        {
            case WeaponType.Hammer:
                WeaponSelectorP.currentWeapon = WeaponType.Polearm;
                WeaponText.text = "Weapon: Polearm";
                break;
            case WeaponType.Polearm:
                WeaponSelectorP.currentWeapon = WeaponType.SwordAndShield;
                WeaponText.text = "Weapon: Sword & Shield";
                break;
            case WeaponType.SwordAndShield:
                WeaponSelectorP.currentWeapon = WeaponType.Dualsword;
                WeaponText.text = "Weapon: Dual Sword";
                break;
            case WeaponType.Dualsword:
                WeaponSelectorP.currentWeapon = WeaponType.Hammer;
                WeaponText.text = "Weapon: Hammer";
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
                WeaponText.text = "Weapon: Dual Sword";
                break;
            case WeaponType.Polearm:
                WeaponSelectorP.currentWeapon = WeaponType.Hammer;
                WeaponText.text = "Weapon: Hammer";
                break;
            case WeaponType.SwordAndShield:
                WeaponSelectorP.currentWeapon = WeaponType.Polearm;
                WeaponText.text = "Weapon: Polearm";
                break;
            case WeaponType.Dualsword:
                WeaponSelectorP.currentWeapon = WeaponType.SwordAndShield;
                WeaponText.text = "Weapon: Sword & Shield";
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
