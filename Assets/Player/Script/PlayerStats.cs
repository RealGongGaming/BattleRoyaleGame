using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Base stats
    [Header("Base Stats")]
    public float baseMaxHP;
    public float baseMoveSpeed;
    public float baseAttack;
    public float baseAttackSpeed;
    public float baseAttackRange;
    public float baseAttackLength;
    public float baseKnockback;
    public float baseKnockbackResist;

    // Multiplier stats
    [Header("Multipliers")]
    public float hpMultiplier = 1f;
    public float moveSpeedMultiplier = 1f;
    public float attackMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float attackRangeMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public float knockbackResistBonus = 0f;

    // Final stats

    [Header("Final Stats")]
    public float maxHP;
    public float currentHP;
    public float moveSpeed;
    public float attack;
    public float attackSpeed;
    public float attackRange;
    public float knockback;
    public float knockbackResist;

    private bool isDead = false;
    public bool IsDead() => isDead;

    void Start()
    {
        RecalculateStats();
        currentHP = maxHP;
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            RecalculateStats();
            WeaponSelector ws = GetComponent<WeaponSelector>();
            if (ws != null) ws.RescaleWeapons();
        }
    }

    public void RecalculateStats()
    {
        maxHP = baseMaxHP * hpMultiplier;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
        attack = baseAttack * attackMultiplier;
        attackSpeed = baseAttackSpeed * attackSpeedMultiplier;
        attackRange = baseAttackRange * attackRangeMultiplier;
        knockback = baseKnockback * knockbackMultiplier;
        knockbackResist = baseKnockbackResist + knockbackResistBonus;
    }

    public void TakeDamage(float damage, Vector3 knockbackDir, float knockbackForce)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("GetHit");

        float actualKnockback = knockbackForce * (1f - Mathf.Clamp01(knockbackResist));
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(knockbackDir.normalized * actualKnockback, ForceMode.Impulse);

        if (currentHP <= 0)
            Die(knockbackDir.normalized * actualKnockback);
    }

    void Die(Vector3 deathForce = default)
    {
        isDead = true;

        PlayerVisualEffects vfx = GetComponent<PlayerVisualEffects>();
        if (vfx != null) vfx.DisableEffectsOnDeath();

        RagdollController ragdoll = GetComponent<RagdollController>();
        if (ragdoll != null)
            ragdoll.EnableRagdollWithForce(deathForce * 2f);

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;
    }
}
