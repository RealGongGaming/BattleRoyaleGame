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

        if (other.transform.root == owner.transform.root)
            return;

        GameObject rootObj = other.transform.root.gameObject;

        if (alreadyHit.Contains(rootObj))
            return;

        alreadyHit.Add(rootObj);

        Vector3 knockbackDir =
            (rootObj.transform.position - owner.transform.position).normalized;

        PlayerController enemyController =
            rootObj.GetComponent<PlayerController>();

        if (enemyController != null)
        {
            enemyController.ReceiveAttack(
                ownerStats.attack,
                knockbackDir,
                ownerStats.knockback,
                owner
            );
        }
    }
}
