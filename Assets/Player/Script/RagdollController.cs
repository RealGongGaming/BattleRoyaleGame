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

        AttachNameTagToHip();
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

    private void AttachNameTagToHip()
    {
        PlayerNameTag nameTag = GetComponent<PlayerNameTag>();
        if (nameTag == null) return;

        foreach (var rb in ragdollBodies)
        {
            if (rb.gameObject == gameObject) continue;
            if (rb.gameObject.activeInHierarchy)
            {
                nameTag.FollowRagdoll(rb.transform);
                return;
            }
        }
    }

    private Transform FindBone(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name)) return child;
            Transform found = FindBone(child, name);
            if (found != null) return found;
        }
        return null;
    }
}