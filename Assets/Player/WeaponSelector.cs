using UnityEngine;

public enum WeaponType
{
    Greatsword,
    SwordAndShield,
    Polearm,
    Dualsword
}

public class WeaponSelector : MonoBehaviour
{
    public WeaponType currentWeapon;

    [Header("Body Models")]
    public GameObject heroGreatswordBody;
    public GameObject heroSwordAndShieldBody;
    public GameObject heroPolearmBody;
    public GameObject DualswordBody;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController greatswordController;
    public RuntimeAnimatorController swordAndShieldController;
    public RuntimeAnimatorController polearmController;
    public RuntimeAnimatorController DualswordController;

    [Header("Animation Avatars")]
    public Avatar greatswordAvatar;
    public Avatar swordAndShieldAvatar;
    public Avatar polearmAvatar;
    public Avatar DualswordAvatar;

    [Header("Weapon Models")]
    public GameObject greatswordModel;
    public GameObject swordModel;
    public GameObject shieldModel;
    public GameObject polearmModel;
    public GameObject dualsword1Model;
    public GameObject dualsword2Model;

    private bool origScalesSaved = false;
    private Vector3 greatswordOrigScale;
    private Vector3 swordOrigScale;
    private Vector3 shieldOrigScale;
    private Vector3 polearmOrigScale;
    private Vector3 dualsword1OrigScale;
    private Vector3 dualsword2OrigScale;

    private void SaveOriginalScales()
    {
        if (origScalesSaved) return;
        if (greatswordModel != null) greatswordOrigScale = greatswordModel.transform.localScale;
        if (swordModel != null) swordOrigScale = swordModel.transform.localScale;
        if (shieldModel != null) shieldOrigScale = shieldModel.transform.localScale;
        if (polearmModel != null) polearmOrigScale = polearmModel.transform.localScale;
        if (dualsword1Model != null) dualsword1OrigScale = dualsword1Model.transform.localScale;
        if (dualsword2Model != null) dualsword2OrigScale = dualsword2Model.transform.localScale;
        origScalesSaved = true;
    }

    private void ScaleWeapon(GameObject weapon, Vector3 origScale, float range)
    {
        if (weapon == null) return;
        weapon.SetActive(true);
        weapon.transform.localScale = origScale * range;
    }

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
        SaveOriginalScales();
        Animator animator = GetComponent<Animator>();
        PlayerStats stats = GetComponent<PlayerStats>();

        if (heroGreatswordBody != null) heroGreatswordBody.SetActive(false);
        if (heroSwordAndShieldBody != null) heroSwordAndShieldBody.SetActive(false);
        if (heroPolearmBody != null) heroPolearmBody.SetActive(false);
        if (DualswordBody != null) DualswordBody.SetActive(false);

        if (greatswordModel != null) greatswordModel.SetActive(false);
        if (swordModel != null) swordModel.SetActive(false);
        if (shieldModel != null) shieldModel.SetActive(false);
        if (polearmModel != null) polearmModel.SetActive(false);
        if (dualsword1Model != null) dualsword1Model.SetActive(false);
        if (dualsword2Model != null) dualsword2Model.SetActive(false);

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
                stats.knockback = 30f;
                stats.knockbackResist = 0.2f;
                ScaleWeapon(greatswordModel, greatswordOrigScale, stats.attackRange);
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
                stats.knockback = 16f;
                stats.knockbackResist = 0.3f;
                ScaleWeapon(swordModel, swordOrigScale, stats.attackRange);
                ScaleWeapon(shieldModel, shieldOrigScale, stats.attackRange);
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
                stats.knockback = 24f;
                stats.knockbackResist = 0.1f;
                ScaleWeapon(polearmModel, polearmOrigScale, stats.attackRange);
                break;

            case WeaponType.Dualsword:
                if (DualswordBody != null) DualswordBody.SetActive(true);
                animator.runtimeAnimatorController = DualswordController;
                animator.avatar = DualswordAvatar;
                animator.Rebind();
                animator.Update(0f);
                stats.maxHP = 90f;
                stats.currentHP = stats.maxHP;
                stats.moveSpeed = 7f;
                stats.attack = 8f;
                stats.baseAttackRange = 0.6f;
                stats.baseAttackSpeed = 1.6f;
                stats.baseAttackLength = 1.3f;
                stats.knockback = 12f;
                stats.knockbackResist = 0.4f;
                ScaleWeapon(dualsword1Model, dualsword1OrigScale, stats.attackRange);
                ScaleWeapon(dualsword2Model, dualsword2OrigScale, stats.attackRange);
                break;
        }
    }
}