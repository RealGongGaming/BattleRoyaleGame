using UnityEngine;

public class PlayerEffectHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem dust;

    [SerializeField] private float minMoveSpeed = 0.1f;
    
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0);

    private Vector3 lastPosition;
    private bool isMoving;
    private bool isGrounded;

    void Start()
    {
        lastPosition = transform.position;

        if (dust == null)
        {
            dust = GetComponentInChildren<ParticleSystem>();
        }
    }

    void Update()
    {
        CheckStatus();
        HandleDust();
    }

    void CheckStatus()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        
        isMoving = speed > minMoveSpeed;

        isGrounded = Physics.Raycast(transform.position + groundCheckOffset, Vector3.down, groundCheckDistance);

        lastPosition = transform.position;
    }

    void HandleDust()
    {
        if (isMoving && isGrounded)
        {
            if (!dust.isPlaying)
            {
                dust.Play();
            }
        }
        else
        {
            if (dust.isPlaying)
            {
                dust.Stop();
            }
        }
    }
}