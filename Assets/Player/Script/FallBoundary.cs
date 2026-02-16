using UnityEngine;
using System.Collections.Generic;

public class FallBoundary : MonoBehaviour
{
    public float killY = -25f;

    private List<Rigidbody> trackedBodies = new List<Rigidbody>();

    void Start()
    {
        trackedBodies.AddRange(FindObjectsByType<Rigidbody>(FindObjectsSortMode.None));
    }

    void Update()
    {
        for (int i = trackedBodies.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = trackedBodies[i];
            if (rb == null)
            {
                trackedBodies.RemoveAt(i);
                continue;
            }

            if (rb.transform.position.y < killY)
            {
                PlayerStats player = rb.GetComponent<PlayerStats>();
                if (player != null && !player.IsDead())
                {
                    player.TakeDamage(999999f, Vector3.zero, 0f);
                }

                FreezeRigidbody(rb);
                Rigidbody[] childRbs = rb.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody childRb in childRbs)
                {
                    FreezeRigidbody(childRb);
                }

                trackedBodies.RemoveAt(i);
            }
        }
    }

    void FreezeRigidbody(Rigidbody rb)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}