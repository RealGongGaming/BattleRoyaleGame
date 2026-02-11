using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    [Header("Settings")]
    public float minDistanceToSpawn = 0.001f; // ค่าที่ปรับแก้แล้ว

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private float spawnTimer;
    private Vector3 lastPosition;

    void Start()
    {
        if (positionToSpawn == null) positionToSpawn = transform;

        if (skinnedMeshRenderers == null)
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        lastPosition = positionToSpawn.position;
    }
    
    void Update()
    {
        float distanceMoved = Vector3.Distance(positionToSpawn.position, lastPosition);

        if (distanceMoved > minDistanceToSpawn && Time.time >= spawnTimer)
        {
            SpawnTrailMesh();
            spawnTimer = Time.time + meshRefreshRate;
        }

        lastPosition = positionToSpawn.position;
    }

    void SpawnTrailMesh()
    {
        if (skinnedMeshRenderers == null) return;

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            GameObject gObj = new GameObject("Trail_Ghost");
            
            gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
            gObj.transform.localScale = positionToSpawn.localScale;

            MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
            MeshFilter mf = gObj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedMeshRenderers[i].BakeMesh(mesh);

            mf.mesh = mesh;
            mr.material = mat;

            // --- ส่วนที่เพิ่มเข้ามาเพื่อปิดเงาครับ ---
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // ไม่ทอดเงาลงพื้น
            mr.receiveShadows = false; // ไม่รับเงาจากคนอื่น (ทำให้สีสดขึ้น ไม่โดนเงามืดบัง)
            // ------------------------------------

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