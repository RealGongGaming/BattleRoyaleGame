using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        DisableRagdoll();
    }

    public void DisableRagdoll()
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb.gameObject == gameObject) continue;
            rb.isKinematic = true;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject == gameObject) continue;
            if (col.isTrigger) continue;
            col.enabled = false;
        }
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;

        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb.gameObject == gameObject) continue;
            rb.isKinematic = false;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject == gameObject) continue;
            if (col.isTrigger) continue;
            col.enabled = true;
        }
    }

    public void EnableRagdollWithForce(Vector3 force)
    {
        EnableRagdoll();

        if (ragdollBodies.Length > 1)
        {
            ragdollBodies[1].AddForce(force, ForceMode.Impulse);
        }
    }

    public void RefreshBones()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        DisableRagdoll();
    }
}