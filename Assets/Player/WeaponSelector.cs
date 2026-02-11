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

    [Header("Body Models")]
    public GameObject heroGreatswordBody;
    public GameObject heroSwordAndShieldBody;
    public GameObject heroPolearmBody;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController greatswordController;
    public RuntimeAnimatorController swordAndShieldController;
    public RuntimeAnimatorController polearmController;

    [Header("Animation Avatars")]
    public Avatar greatswordAvatar;
    public Avatar swordAndShieldAvatar;
    public Avatar polearmAvatar;

    void Start()
    {
        ApplyWeapon();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(DelayedApplyWeapon());
        }
    }

    private System.Collections.IEnumerator DelayedApplyWeapon()
    {
        yield return null;
        ApplyWeapon();
    }

    public void ApplyWeapon()
    {
        Animator animator = GetComponent<Animator>();
        PlayerStats stats = GetComponent<PlayerStats>();

        if (heroGreatswordBody != null) heroGreatswordBody.SetActive(false);
        if (heroSwordAndShieldBody != null) heroSwordAndShieldBody.SetActive(false);
        if (heroPolearmBody != null) heroPolearmBody.SetActive(false);

        switch (currentWeapon)
        {
            case WeaponType.Greatsword:
                if (heroGreatswordBody != null) heroGreatswordBody.SetActive(true);
                animator.runtimeAnimatorController = greatswordController;
                animator.avatar = greatswordAvatar;
                animator.Rebind();
                animator.Update(0f);
                stats.maxHP = 120f;
                stats.currentHP = stats.maxHP;
                stats.moveSpeed = 4f;
                stats.attack = 15f;
                stats.baseAttackRange = 1.5f;
                stats.baseAttackSpeed = 0.8f;
                stats.baseAttackLength = 1.6f;
                stats.knockback = 8f;
                stats.knockbackResist = 0.2f;
                break;

            case WeaponType.SwordAndShield:
                if (heroSwordAndShieldBody != null) heroSwordAndShieldBody.SetActive(true);
                animator.runtimeAnimatorController = swordAndShieldController;
                animator.avatar = swordAndShieldAvatar;
                animator.Rebind();
                animator.Update(0f);
                stats.maxHP = 100f;
                stats.currentHP = stats.maxHP;
                stats.moveSpeed = 6f;
                stats.attack = 10f;
                stats.baseAttackRange = 1f;
                stats.baseAttackSpeed = 1.2f;
                stats.baseAttackLength = 1.4f;
                stats.knockback = 4f;
                stats.knockbackResist = 0.3f;
                break;

            case WeaponType.Polearm:
                if (heroPolearmBody != null) heroPolearmBody.SetActive(true);
                animator.runtimeAnimatorController = polearmController;
                animator.avatar = polearmAvatar;
                animator.Rebind();
                animator.Update(0f);
                stats.maxHP = 100f;
                stats.currentHP = stats.maxHP;
                stats.moveSpeed = 5f;
                stats.attack = 12f;
                stats.baseAttackRange = 2f;
                stats.baseAttackSpeed = 0.9f;
                stats.baseAttackLength = 1.4f;
                stats.knockback = 6f;
                stats.knockbackResist = 0.1f;
                break;
        }
    }
}