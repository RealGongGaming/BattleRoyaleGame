using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class PlayerVisualEffects : MonoBehaviour
{
    [Header("Slash VFX")]
    public VisualEffect hammerSlashVFX;
    public VisualEffect swordSlashVFX;
    public VisualEffect polearmSlashVFX;
    public VisualEffect dualswordSlashVFX_1;
    public VisualEffect dualswordSlashVFX_2;

    [Header("Weapon Selector")]
    public WeaponSelector weaponSelector;

    [Header("Trail Materials")]
    public Material Shadow_Hammer;
    public Material Shadow_SwordAndShield;
    public Material Shadow_Polearm;
    public Material Shadow_Dualsword;

    [Header("Dust Settings")]
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private float minMoveSpeed = 1.0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0);

    [Header("Trail Mesh Settings")]
    public float meshRefreshRate = 0.15f;
    public float meshDestroyDelay = 1.5f;

    [Header("Trail Shader Settings")]
    public Material mat;
    public string shaderVarRef = "_Alpha";
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    [Header("Parry Effect")]
    public GameObject parryRaysEffect;

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
        if (weaponSelector == null) weaponSelector = GetComponent<WeaponSelector>();
        if (skinnedMeshRenderers == null)
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
    }

    void Update()
    {
        CheckStatus();
        HandleDust();
        HandleTrail();
    }

    public void ShowParryEffect()
    {
        if (parryRaysEffect != null)
            parryRaysEffect.SetActive(true);
    }

    public void HideParryEffect()
    {
        if (parryRaysEffect != null)
            parryRaysEffect.SetActive(false);
    }

    public void PlaySlashEffect(int count)
    {
        if (count == 0) count = 1;

        switch (weaponSelector.currentWeapon)
        {
            case WeaponType.Hammer:
                hammerSlashVFX.Play();
                break;
            case WeaponType.SwordAndShield:
                swordSlashVFX.Play();
                break;
            case WeaponType.Polearm:
                polearmSlashVFX.Play();
                break;
            case WeaponType.Dualsword:
                if (count == 1)
                {
                    dualswordSlashVFX_1.Play();
                }
                else if (count == 2)
                {
                    dualswordSlashVFX_2.Play();
                }
                break;
        }
    }

    void CheckStatus()
    {
        isGrounded = Physics.Raycast(transform.position + groundCheckOffset, Vector3.down, groundCheckDistance, groundLayer);
        float distance = (transform.position - lastPosition).magnitude;
        currentSpeed = distance / Time.deltaTime;

        if (isGrounded)
        {
            isMoving = currentSpeed > minMoveSpeed;
        }
        else
        {
            isMoving = currentSpeed > (minMoveSpeed * 0.5f);
        }

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

    Material GetCurrentMaterial()
    {
        switch (weaponSelector.currentWeapon)
        {
            case WeaponType.Hammer:
                return Shadow_Hammer;

            case WeaponType.SwordAndShield:
                return Shadow_SwordAndShield;

            case WeaponType.Polearm:
                return Shadow_Polearm;

            case WeaponType.Dualsword:
                return Shadow_Dualsword;

            default:
                return Shadow_Hammer;
        }
    }

    void SpawnTrailMesh()
    {
        if (skinnedMeshRenderers == null) return;

        Material selectedMat = GetCurrentMaterial();
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            if (!skinnedMeshRenderers[i].gameObject.activeInHierarchy) continue;

            GameObject gObj = new GameObject("Trail_Ghost");
            Transform targetBone = skinnedMeshRenderers[i].transform;

            gObj.transform.SetPositionAndRotation(targetBone.position, targetBone.rotation);
            gObj.transform.localScale = Vector3.one;

            MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
            MeshFilter mf = gObj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedMeshRenderers[i].BakeMesh(mesh);

            mf.mesh = mesh;
            mr.material = selectedMat;

            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
            Destroy(gObj, meshDestroyDelay);
        }
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        if (mat.HasProperty(shaderVarRef))
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