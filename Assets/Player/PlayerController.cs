using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float jumpForce = 10f;
    private Vector2 move;
    private bool isGrounded;
    private Animator animator;
    private bool canAttack = true;
    private bool canDodge = true;
    private bool isDodging = false;
    private bool isParrying = false;
    private bool isStunned = false;
    private PlayerStats stats;

    [Header("Ability Settings")]
    [HideInInspector] public bool canUseDodge = false;
    [HideInInspector] public bool canUseParry = false;

    [Header("Dodge Settings")]
    public float dodgeForce = 50f;
    public float dodgeDuration = 0.8f;
    public float dodgeCooldown = 3f;

    [Header("Parry Settings")]
    public float parryWindow = 0.8f;
    public float parryCooldown = 4f;
    public float stunDuration = 2.5f;
    private bool canParry = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (!isDodging)
        {
            movePlayer();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void movePlayer()
    {
        if (isStunned)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        if (move.sqrMagnitude > 0.1f)
        {
            Vector3 movement = new Vector3(move.x, 0, move.y) * stats.baseMoveSpeed * stats.moveSpeed * Time.deltaTime;
            if (movement.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
            }
            transform.Translate(movement, Space.World);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack && !isDodging && !isParrying && !isStunned)
        {
            float finalSpeed = stats.baseAttackSpeed * stats.attackSpeed;
            animator.SetFloat("AttackSpeed", stats.baseAttackLength * finalSpeed);
            animator.SetTrigger("Attack");
            StartCoroutine(AttackCooldown());
            StartCoroutine(DelayedDealDamage(finalSpeed));
        }
    }

    private IEnumerator DelayedDealDamage(float finalSpeed)
    {
        float hitDelay = (stats.baseAttackLength / 4f) / finalSpeed;
        yield return new WaitForSeconds(hitDelay);
        DealDamage();
    }

    private void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, stats.baseAttackRange * stats.attackRange);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector3 knockbackDir = hit.transform.position - transform.position;
            PlayerStats enemyStats = hit.GetComponent<PlayerStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(stats.attack, knockbackDir, stats.knockback, this);
            }
            else
            {
                Rigidbody hitRb = hit.GetComponent<Rigidbody>();
                if (hitRb != null)
                {
                    hitRb.AddForce(knockbackDir.normalized * stats.knockback, ForceMode.Impulse);
                }
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        float finalSpeed = stats.baseAttackSpeed * stats.attackSpeed;
        canAttack = false;
        yield return new WaitForSeconds((1f / finalSpeed) * 0.7f);
        canAttack = true;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && canDodge && !isDodging && !isStunned && canUseDodge)
        {
            StartCoroutine(DodgeRoutine());
        }
    }

    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        canDodge = false;
        animator.SetTrigger("Dodge");

        Vector3 dodgeDir;
        if (move.sqrMagnitude > 0.1f)
        {
            dodgeDir = new Vector3(move.x, 0, move.y).normalized;
            transform.rotation = Quaternion.LookRotation(dodgeDir);
        }
        else
        {
            dodgeDir = transform.forward;
        }

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        rb.AddForce(dodgeDir * dodgeForce, ForceMode.Impulse);

        yield return new WaitForSeconds(dodgeDuration);
        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown - dodgeDuration);
        canDodge = true;
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed && canParry && !isDodging && !isParrying && !isStunned && canUseParry)
        {
            StartCoroutine(ParryRoutine());
        }
    }

    private IEnumerator ParryRoutine()
    {
        isParrying = true;
        canParry = false;
        animator.SetTrigger("Parry");

        yield return new WaitForSeconds(parryWindow);
        isParrying = false;

        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    public bool IsParrying() => isParrying;

    public bool TryParry(Vector3 attackerPosition, PlayerController attacker)
    {
        if (!isParrying) return false;

        
        if (attacker != null)
        {
            attacker.ApplyStun(stunDuration);
        }

        StopCoroutine(nameof(ParryRoutine));
        isParrying = false;
        canParry = true;

        return true;
    }

    public bool IsStunned() => isStunned;

    public void ApplyStun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        animator.SetBool("IsStunned", true);
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        animator.SetBool("IsStunned", false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isStunned)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        animator.SetBool("IsJumping", false);
    }
}