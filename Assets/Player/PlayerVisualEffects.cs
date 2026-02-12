using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualEffects : MonoBehaviour
{
    public bool enableDust = true;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private float dustMinSpeed = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, 0.2f, 0);

    public bool enableTrail = true;
    public float trailMinSpeed = 2.0f;
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;

    public Material mat; 
    public string shaderVarRef = "_Alpha";
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private float spawnTimer;
    private Vector3 lastPosition;
    private float currentSpeed;
    private bool isGrounded;

    void Start()
    {
        lastPosition = transform.position;

        if (dust == null) dust = GetComponentInChildren<ParticleSystem>();
        if (skinnedMeshRenderers == null) skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        CalculateState();

        if (enableDust) HandleDustLogic();
        if (enableTrail) HandleTrailLogic();
    }

    void CalculateState()
    {
        currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        isGrounded = Physics.Raycast(transform.position + groundCheckOffset, Vector3.down, groundCheckDistance, groundLayer);
    }

    void HandleDustLogic()
    {
        if (isGrounded && currentSpeed > dustMinSpeed)
        {
            if (!dust.isPlaying) dust.Play();
        }
        else
        {
            if (dust.isPlaying) dust.Stop();
        }
    }

    void HandleTrailLogic()
    {
        if (currentSpeed > trailMinSpeed && Time.time >= spawnTimer)
        {
            SpawnTrailMesh();
            spawnTimer = Time.time + meshRefreshRate;
        }
    }

    void SpawnTrailMesh()
    {
        if (skinnedMeshRenderers == null) return;

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            GameObject gObj = new GameObject("Trail_Ghost");
            
            Transform targetBone = skinnedMeshRenderers[i].transform;
            gObj.transform.SetPositionAndRotation(targetBone.position, targetBone.rotation);
            
            Vector3 targetScale = targetBone.lossyScale;
            gObj.transform.localScale = targetScale;

            MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
            MeshFilter mf = gObj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedMeshRenderers[i].BakeMesh(mesh);
            mf.mesh = mesh;

            mr.material = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; 
            mr.receiveShadows = false; 

            StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

            Destroy(gObj, meshDestroyDelay);
        }
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        if(mat.HasProperty(shaderVarRef))
        {
            float valueToAnimate = mat.GetFloat(shaderVarRef);

            while (valueToAnimate > goal)
            {
                valueToAnimate -= rate;
                mat.SetFloat(shaderVarRef, valueToAnimate);
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + groundCheckOffset, transform.position + groundCheckOffset + (Vector3.down * groundCheckDistance));
    }
}