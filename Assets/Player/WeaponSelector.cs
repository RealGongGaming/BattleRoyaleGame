using UnityEngine;

public enum WeaponType
{
    Greatsword,
    SwordAndShield,
    Polearm
}

public class WeaponSelector : MonoBehaviour
{
    public WeaponType currentWeapon;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController greatswordController;
    public RuntimeAnimatorController swordAndShieldController;
    public RuntimeAnimatorController polearmController;

    [Header("Weapon Models (drag from the hand bone children)")]
    public GameObject greatswordModel;
    public GameObject swordModel;
    public GameObject shieldModel;
    public GameObject polearmModel;

    void Start()
    {
        ApplyWeapon();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyWeapon();
        }
    }

    public void ApplyWeapon()
    {
        Animator animator = GetComponent<Animator>();
        PlayerStats stats = GetComponent<PlayerStats>();

        // Hide all weapons first
        if (greatswordModel != null) greatswordModel.SetActive(false);
        if (swordModel != null) swordModel.SetActive(false);
        if (shieldModel != null) shieldModel.SetActive(false);
        if (polearmModel != null) polearmModel.SetActive(false);

        switch (currentWeapon)
        {
            case WeaponType.Greatsword:
                animator.runtimeAnimatorController = greatswordController;
                if (greatswordModel != null) greatswordModel.SetActive(true);
                stats.attack = 15f;
                stats.attackRange = 2.5f;
                stats.baseAttackSpeed = 0.8f;
                stats.baseAttackLength = 1.6f;
                stats.knockback = 8f;
                break;

            case WeaponType.SwordAndShield:
                animator.runtimeAnimatorController = swordAndShieldController;
                if (swordModel != null) swordModel.SetActive(true);
                if (shieldModel != null) shieldModel.SetActive(true);
                stats.attack = 10f;
                stats.attackRange = 1.8f;
                stats.baseAttackSpeed = 1.2f;
                stats.baseAttackLength = 1.4f;
                stats.knockback = 4f;
                break;

            case WeaponType.Polearm:
                animator.runtimeAnimatorController = polearmController;
                if (polearmModel != null) polearmModel.SetActive(true);
                stats.attack = 12f;
                stats.attackRange = 3f;
                stats.baseAttackSpeed = 0.9f;
                stats.baseAttackLength = 1.3f;
                stats.knockback = 6f;
                break;
        }
    }
}