using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
public class LobbyController : MonoBehaviour
{
    public bool isReady = false;
    public TextMeshProUGUI ReadyText;
    public TextMeshProUGUI WeaponText;
    public TextMeshProUGUI SkillText;
    public bool canCycleWeapon = true;
    public WeaponSelector WeaponSelectorP;

    void Start()
    {
        switch (WeaponSelectorP.currentWeapon)
        {
            case WeaponType.Hammer:
                WeaponText.text = "Weapon: Hammer";
                break;
            case WeaponType.Polearm:
                WeaponText.text = "Weapon: Polearm";
                break;
            case WeaponType.SwordAndShield:
                WeaponText.text = "Weapon: Sword & Shield";
                break;
            case WeaponType.Dualsword:
                WeaponText.text = "Weapon: Dual Sword";
                break;
        }
    }

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

            DetectSkill();
        }
    }

    void DetectSkill()
    {
        switch (WeaponSelectorP.currentWeapon)
        {
            case WeaponType.Hammer:
                SkillText.text = "Skill - Parry :  F";
                break;
            case WeaponType.SwordAndShield:
                SkillText.text = "Skill - Parry :  F";
                break;
            case WeaponType.Polearm:
                SkillText.text = "Skill - Dash :  Lshift";
                break;
            case WeaponType.Dualsword:
                SkillText.text = "Skill - Dash :  Lshift";
                break;
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
