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
    private PlayerStats stats;

    private bool canAttack = true;
    private bool canDodge = true;
    private bool isAttacking = false;
    private bool isDodging = false;
    private bool isParrying = false;
    private bool isStunned = false;

    [Header("Ability Settings")]
    [HideInInspector] public bool canUseDodge = false;
    [HideInInspector] public bool canUseParry = false;

    [Header("Dodge Settings")]
    public float dodgeMultiplier = 2.5f;
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 3f;

    [Header("Parry Settings")]
    public float parryStartup = 0.2f;
    public float parryWindow = 0.4f;
    public float parryCooldown = 5f;
    public float stunDuration = 2.5f;

    private bool canParry = true;
    private Coroutine parryCoroutine;

    [Header("Weapon Hitbox")]
    public AnimationEventReceiver eventReceiver;

    [Header("Parry Effects")]
    public PlayerVisualEffects visualEffects;

    void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (!isDodging)
        {
            MovePlayer();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    void MovePlayer()
    {
        if (isStunned)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        if (move.sqrMagnitude > 0.1f)
        {
            Vector3 movement = new Vector3(move.x, 0, move.y) *
                               stats.moveSpeed *
                               Time.deltaTime;

            if (movement.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(movement),
                    0.15f
                );
            }

            transform.Translate(movement, Space.World);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }


    // Attack

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack && !isDodging && !isParrying && !isStunned && !stats.IsDead())
        {
            float finalSpeed = stats.attackSpeed;

            animator.SetFloat("AttackSpeed", stats.baseAttackLength * finalSpeed);
            animator.SetTrigger("Attack");

            StartCoroutine(AttackCooldown(finalSpeed));
            StartCoroutine(HitboxWindow(finalSpeed));
        }
    }

    IEnumerator HitboxWindow(float finalSpeed)
    {
        isAttacking = true;

        float startDelay = (stats.baseAttackLength / 4f) / finalSpeed;
        float activeTime = (stats.baseAttackLength / 4f) / finalSpeed;

        yield return new WaitForSeconds(startDelay);
        eventReceiver.EnableHitbox();

        yield return new WaitForSeconds(activeTime);
        eventReceiver.DisableHitbox();

        isAttacking = false;
    }

    IEnumerator AttackCooldown(float finalSpeed)
    {
        canAttack = false;
        yield return new WaitForSeconds((1f / finalSpeed) * 0.7f);
        canAttack = true;
    }

    // Dodge

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && canDodge && !isDodging && !isStunned && canUseDodge && !stats.IsDead())
        {
            StartCoroutine(DodgeRoutine());
        }
    }

    IEnumerator DodgeRoutine()
    {
        isDodging = true;
        canDodge = false;

        animator.SetFloat("DodgeSpeed", 1f / dodgeDuration);
        animator.SetTrigger("Dodge");

        Vector3 dodgeDir = move.sqrMagnitude > 0.1f
            ? new Vector3(move.x, 0, move.y).normalized
            : transform.forward;

        float dodgeSpeed = stats.moveSpeed * dodgeMultiplier;

        float timer = 0f;

        while (timer < dodgeDuration)
        {
            rb.linearVelocity = new Vector3(
                dodgeDir.x * dodgeSpeed,
                rb.linearVelocity.y,
                dodgeDir.z * dodgeSpeed
            );

            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }


    // Parry

    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed && canParry && !isDodging && !isParrying && !isAttacking && !isStunned && canUseParry && !stats.IsDead())
        {
            parryCoroutine = StartCoroutine(ParryRoutine());
        }
    }

    IEnumerator ParryRoutine()
    {
        canParry = false;

        animator.SetFloat("ParrySpeed", 0.9f / (parryWindow + parryStartup));
        animator.SetTrigger("Parry");

        if (parryStartup > 0f)
            yield return new WaitForSeconds(parryStartup);

        isParrying = true;
        visualEffects.ShowParryEffect();

        yield return new WaitForSeconds(parryWindow);

        visualEffects.HideParryEffect();
        isParrying = false;

        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    public bool TryParry(PlayerController attacker)
    {
        if (!isParrying) return false;

        if (attacker != null)
        {
            attacker.ApplyStun(stunDuration);
        }

        canParry = true;

        return true;
    }

    // Receive Attack

    public void ReceiveAttack(float damage, Vector3 knockbackDir, float knockbackForce, PlayerController attacker)
    {
        if (TryParry(attacker))
        {
            return;
        }

        stats.TakeDamage(damage, knockbackDir, knockbackForce);
    }

    // Stun

    public void ApplyStun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        animator.SetBool("IsStunned", true);
        rb.linearVelocity = Vector3.zero;
        visualEffects.ShowStunnedEffect();

        yield return new WaitForSeconds(duration);

        visualEffects.HideStunnedEffect();
        isStunned = false;
        animator.SetBool("IsStunned", false);
    }

    // Jump

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isStunned && !stats.IsDead())
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