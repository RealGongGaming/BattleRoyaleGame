using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    private Vector2 move;
    private bool isGrounded;
    private Animator animator;
    private bool canAttack = true;
    private PlayerStats stats;

    void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        movePlayer();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void movePlayer()
    {
        if (move.sqrMagnitude > 0.1f)
        {
            Vector3 movement = new Vector3(move.x, 0, move.y) * stats.moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
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
        if (context.performed && canAttack)
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
                enemyStats.TakeDamage(stats.attack, knockbackDir, stats.knockback);
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}