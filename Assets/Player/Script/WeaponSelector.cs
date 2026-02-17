using UnityEngine;

public enum WeaponType
{
    Hammer,
    SwordAndShield,
    Polearm,
    Dualsword
}

public class WeaponSelector : MonoBehaviour
{
    public WeaponType currentWeapon;

    [Header("Body Models")]
    public GameObject heroHammerBody;
    public GameObject heroSwordAndShieldBody;
    public GameObject heroPolearmBody;
    public GameObject DualswordBody;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController hammerController;
    public RuntimeAnimatorController swordAndShieldController;
    public RuntimeAnimatorController polearmController;
    public RuntimeAnimatorController DualswordController;

    [Header("Animation Avatars")]
    public Avatar hammerAvatar;
    public Avatar swordAndShieldAvatar;
    public Avatar polearmAvatar;
    public Avatar DualswordAvatar;

    [Header("Weapon Models")]
    public GameObject hammerModel;
    public GameObject swordModel;
    public GameObject shieldModel;
    public GameObject polearmModel;
    public GameObject dualsword1Model;
    public GameObject dualsword2Model;

    private bool origScalesSaved = false;
    private Vector3 hammerOrigScale;
    private Vector3 swordOrigScale;
    private Vector3 shieldOrigScale;
    private Vector3 polearmOrigScale;
    private Vector3 dualsword1OrigScale;
    private Vector3 dualsword2OrigScale;

    private void SaveOriginalScales()
    {
        if (origScalesSaved) return;
        if (hammerModel != null) hammerOrigScale = new Vector3(230, 260, 230);
        if (swordModel != null) swordOrigScale = new Vector3(100, 100, 100);
        if (shieldModel != null) shieldOrigScale = new Vector3(100, 100, 100);
        if (polearmModel != null) polearmOrigScale = new Vector3(120, 150, 120);
        if (dualsword1Model != null) dualsword1OrigScale = new Vector3(75, 75, 75);
        if (dualsword2Model != null) dualsword2OrigScale = new Vector3(75, 75, 75);
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

    public void RescaleWeapons()
    {
        SaveOriginalScales();
        PlayerStats stats = GetComponent<PlayerStats>();

        switch (currentWeapon)
        {
            case WeaponType.Hammer:
                ScaleWeapon(hammerModel, hammerOrigScale, stats.attackRange);
                break;
            case WeaponType.SwordAndShield:
                ScaleWeapon(swordModel, swordOrigScale, stats.attackRange);
                ScaleWeapon(shieldModel, shieldOrigScale, stats.attackRange);
                break;
            case WeaponType.Polearm:
                ScaleWeapon(polearmModel, polearmOrigScale, stats.attackRange);
                break;
            case WeaponType.Dualsword:
                ScaleWeapon(dualsword1Model, dualsword1OrigScale, stats.attackRange);
                ScaleWeapon(dualsword2Model, dualsword2OrigScale, stats.attackRange);
                break;
        }
    }

    public void ApplyWeapon()
    {
        SaveOriginalScales();

        Animator animator = GetComponent<Animator>();
        PlayerStats stats = GetComponent<PlayerStats>();
        PlayerController controller = GetComponent<PlayerController>();
        AnimationEventReceiver eventReceiver = GetComponent<AnimationEventReceiver>();

        if (heroHammerBody != null) heroHammerBody.SetActive(false);
        if (heroSwordAndShieldBody != null) heroSwordAndShieldBody.SetActive(false);
        if (heroPolearmBody != null) heroPolearmBody.SetActive(false);
        if (DualswordBody != null) DualswordBody.SetActive(false);

        if (hammerModel != null) hammerModel.SetActive(false);
        if (swordModel != null) swordModel.SetActive(false);
        if (shieldModel != null) shieldModel.SetActive(false);
        if (polearmModel != null) polearmModel.SetActive(false);
        if (dualsword1Model != null) dualsword1Model.SetActive(false);
        if (dualsword2Model != null) dualsword2Model.SetActive(false);

        switch (currentWeapon)
        {
            case WeaponType.Hammer:

                heroHammerBody.SetActive(true);

                animator.runtimeAnimatorController = hammerController;
                animator.avatar = hammerAvatar;
                animator.Rebind();
                animator.Update(0f);

                stats.baseMaxHP = 120f;
                stats.baseMoveSpeed = 4f;
                stats.baseAttack = 15f;
                stats.baseAttackSpeed = 0.9f;
                stats.baseAttackRange = 1.5f;
                stats.baseAttackLength = 1.6f;
                stats.baseKnockback = 30f;
                stats.baseKnockbackResist = 0.2f;

                controller.canUseDodge = false;
                controller.canUseParry = true;

                eventReceiver.SetActiveWeapon(WeaponType.Hammer);

                break;


            case WeaponType.SwordAndShield:

                heroSwordAndShieldBody.SetActive(true);

                animator.runtimeAnimatorController = swordAndShieldController;
                animator.avatar = swordAndShieldAvatar;
                animator.Rebind();
                animator.Update(0f);

                stats.baseMaxHP = 100f;
                stats.baseMoveSpeed = 6f;
                stats.baseAttack = 12f;
                stats.baseAttackSpeed = 1.2f;
                stats.baseAttackRange = 1f;
                stats.baseAttackLength = 1.4f;
                stats.baseKnockback = 16f;
                stats.baseKnockbackResist = 0.3f;

                controller.canUseDodge = false;
                controller.canUseParry = true;

                eventReceiver.SetActiveWeapon(WeaponType.SwordAndShield);

                break;


            case WeaponType.Polearm:

                heroPolearmBody.SetActive(true);

                animator.runtimeAnimatorController = polearmController;
                animator.avatar = polearmAvatar;
                animator.Rebind();
                animator.Update(0f);

                stats.baseMaxHP = 100f;
                stats.baseMoveSpeed = 5f;
                stats.baseAttack = 12f;
                stats.baseAttackSpeed = 1.1f;
                stats.baseAttackRange = 1f;
                stats.baseAttackLength = 1.4f;
                stats.baseKnockback = 24f;
                stats.baseKnockbackResist = 0.1f;

                controller.canUseDodge = true;
                controller.canUseParry = false;

                eventReceiver.SetActiveWeapon(WeaponType.Polearm);

                break;


            case WeaponType.Dualsword:

                DualswordBody.SetActive(true);

                animator.runtimeAnimatorController = DualswordController;
                animator.avatar = DualswordAvatar;
                animator.Rebind();
                animator.Update(0f);

                stats.baseMaxHP = 90f;
                stats.baseMoveSpeed = 7f;
                stats.baseAttack = 10f;
                stats.baseAttackSpeed = 1.6f;
                stats.baseAttackRange = 1f;
                stats.baseAttackLength = 1.3f;
                stats.baseKnockback = 12f;
                stats.baseKnockbackResist = 0.4f;

                controller.canUseDodge = true;
                controller.canUseParry = false;

                eventReceiver.SetActiveWeapon(WeaponType.Dualsword);

                break;
        }

        stats.RecalculateStats();
        stats.currentHP = stats.maxHP;

        RagdollController ragdoll = GetComponent<RagdollController>();
        if (ragdoll != null) ragdoll.RefreshBones();

        if (hammerModel != null)
            ScaleWeapon(hammerModel, hammerOrigScale, stats.attackRange);

        if (swordModel != null)
            ScaleWeapon(swordModel, swordOrigScale, stats.attackRange);

        if (shieldModel != null)
            ScaleWeapon(shieldModel, shieldOrigScale, stats.attackRange);

        if (polearmModel != null)
            ScaleWeapon(polearmModel, polearmOrigScale, stats.attackRange);

        if (dualsword1Model != null)
            ScaleWeapon(dualsword1Model, dualsword1OrigScale, stats.attackRange);

        if (dualsword2Model != null)
            ScaleWeapon(dualsword2Model, dualsword2OrigScale, stats.attackRange);
    }
}