using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHP;
    public float currentHP;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Attack")]
    public float attack;
    public float baseAttackSpeed;   // base attacks per second
    public float attackSpeed;        // attack speed multiplier
    public float attackRange;
    public float baseAttackLength; // animation original length

    [Header("Knockback")]
    public float knockback;
    public float knockbackResist;   // 0 = no resist, 1 = full resist

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage, Vector3 knockbackDir, float knockbackForce)
    {
        if (isDead) return;

        currentHP -= damage;

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("GetHit");

        float actualKnockback = knockbackForce * (1f - Mathf.Clamp01(knockbackResist));
        if (actualKnockback > 0f)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(knockbackDir.normalized * actualKnockback, ForceMode.Impulse);
            }
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    private void Die()
    {
        isDead = true;

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Die");

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}