using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    private bool isActive = false;
    private PlayerController owner;
    private PlayerStats ownerStats;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    private Collider col;

    void Start()
    {
        owner = GetComponentInParent<PlayerController>();
        ownerStats = GetComponentInParent<PlayerStats>();
        col = GetComponent<Collider>();

        col.enabled = false;
    }

    public void EnableHitbox()
    {
        alreadyHit.Clear();

        col.enabled = true;
        isActive = true;
    }

    public void DisableHitbox()
    {
        isActive = false;
        col.enabled = false;

        alreadyHit.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.transform.root == owner.transform.root) return;

        PlayerController enemyController = other.transform.root.GetComponent<PlayerController>();

        if (enemyController != null)
        {
            GameObject rootObj = other.transform.root.gameObject;
            PlayerStats enemyStats = rootObj.GetComponent<PlayerStats>();

            if (enemyStats != null && enemyStats.IsDead())
            {
                if (alreadyHit.Contains(rootObj)) return;
                alreadyHit.Add(rootObj);

                Vector3 knockbackDir = (rootObj.transform.position - owner.transform.position).normalized;
                float deadKnockbackMultiplier = 0.1f;
                Rigidbody[] bones = rootObj.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody boneRb in bones)
                {
                    boneRb.AddForce(knockbackDir * ownerStats.knockback * deadKnockbackMultiplier, ForceMode.Impulse);
                }
            }
            else
            {
                if (alreadyHit.Contains(rootObj)) return;
                alreadyHit.Add(rootObj);

                Vector3 knockbackDir = (rootObj.transform.position - owner.transform.position).normalized;
                enemyController.ReceiveAttack(ownerStats.attack, knockbackDir, ownerStats.knockback, owner);
            }
        }
        else
        {
            if (alreadyHit.Contains(other.gameObject)) return;
            alreadyHit.Add(other.gameObject);

            Vector3 knockbackDir = (other.transform.position - owner.transform.position).normalized;
            Rigidbody hitRb = other.GetComponent<Rigidbody>();
            if (hitRb == null) hitRb = other.GetComponentInParent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddForce(knockbackDir * ownerStats.knockback, ForceMode.Impulse);
            }
        }
    }
}
