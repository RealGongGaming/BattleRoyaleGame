using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualEffects : MonoBehaviour
{
    [Header("Dust Settings")]
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private float minMoveSpeed = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0);

    [Header("Trail Mesh Settings")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;

    [Header("Trail Shader Settings")]
    public Material mat; 
    public string shaderVarRef = "_Alpha"; 
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private float spawnTimer;
    private Vector3 lastPosition;
    private bool isMoving;
    private bool isGrounded;
    private float currentSpeed;

    void Start()
    {
        lastPosition = transform.position;

        if (dust == null) dust = GetComponentInChildren<ParticleSystem>();
        if (skinnedMeshRenderers == null) skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        CheckStatus();
        HandleDust();
        HandleTrail();
    }

    void CheckStatus()
    {
        float distance = (transform.position - lastPosition).magnitude;
        currentSpeed = distance / Time.deltaTime;
        
        isMoving = currentSpeed > minMoveSpeed;

        isGrounded = Physics.Raycast(transform.position + groundCheckOffset, Vector3.down, groundCheckDistance, groundLayer);

        lastPosition = transform.position;
    }

    void HandleDust()
    {
        if (dust == null) return;

        if (isMoving && isGrounded)
        {
            if (!dust.isPlaying) dust.Play();
        }
        else
        {
            if (dust.isPlaying) dust.Stop();
        }
    }

    void HandleTrail()
    {
        if (isMoving && Time.time >= spawnTimer)
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
            
            // 1. วางตำแหน่งและหมุนให้ตรง
            gObj.transform.SetPositionAndRotation(targetBone.position, targetBone.rotation);
            
            // =========================================================
            // [จุดที่แก้]: เปลี่ยนจาก targetBone.lossyScale เป็น Vector3.one
            // เพื่อไม่ให้สเกลเบิ้ล หรือกลับด้านผิดๆ
            // =========================================================
            gObj.transform.localScale = Vector3.one; 

            MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
            MeshFilter mf = gObj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedMeshRenderers[i].BakeMesh(mesh); // BakeMesh มันรวมสเกลมาให้แล้ว

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
}