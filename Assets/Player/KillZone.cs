using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player == null) player = other.GetComponentInParent<PlayerStats>();

        if (player != null && !player.IsDead())
        {
            player.TakeDamage(999999f, Vector3.zero, 0f);
        }
    }
}