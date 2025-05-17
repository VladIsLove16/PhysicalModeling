using UnityEngine;
using System.Collections.Generic;
using System;
using static MultiMaterialRefraction;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class MultiMaterialRefraction : MonoBehaviour
{
    [System.Serializable]
    public class RefractiveMaterial
    {
        public string name = "Material";
        public Vector3 size = new Vector3(1, 1, 1);
        public Vector3 position = new Vector3(0, 0, 0);
        public float refractiveIndex = 1.5f;
        public Material material;
        public Mesh mesh;
        public bool generate = false;

        public GameObject generatedObject = null;
        [HideInInspector] public MeshCollider collider;
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
    }
   [System.Serializable]
    public struct LensSettings
    {
        public float radius;
        public float distance;
        public float width;
        public float lensRefractiveIndex;
    }
    public enum CalculationMode
    {
        physics,
        mathematic
    }
    public enum RayTracerObject
    {
        lens,
        materials
    }
    [Header("Настройки луча")]
    public Vector3 rayStart = Vector3.zero;
    [Range(-89f, 89)]
    public float angle = 0f;
    public float maxRayLength = 100f;
    public int maxBounces = 10;

    private LineRenderer lineRenderer;
    private IRayPathCalculator rayPathCalculator;
    public CalculationMode calculationMode =  CalculationMode.physics;
    public RayTracerObject rayTracerObject = RayTracerObject.materials;

    [Header("Материалы (создаются автоматически)")]
    public List<RefractiveMaterial> materials = new List<RefractiveMaterial>();
    [Header("Настройки линзы")]
    public BiconvexLensGenerator biconvexLensMesh;
    public LensSettings lensSettings;
    public Vector3 rayDirection
    {
        get
        {
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Tan(rad);
            return new Vector3(x, 1f, 0).normalized;
        }
    }

    void Awake()
    {
        SetupMaterialObjects();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                OnEnabled();
            }
        };
    }
#endif

    void OnEnable()
    {
        OnEnabled();
    }

    private void OnEnabled()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (calculationMode == CalculationMode.physics)
        {
            rayPathCalculator = new PhysicsRayPathCalculator(materials, maxRayLength); // ← Можно заменить на формульную реализацию
        }
        else
        {
            rayPathCalculator = new LensRayTracer(lensSettings.radius,  lensSettings.distance, lensSettings.lensRefractiveIndex);

        }
        if(rayTracerObject == RayTracerObject.lens)
        {
            ToggleMaterials(false);
            ToggleLens(true);
            biconvexLensMesh.GenerateLensMesh(lensSettings.radius, lensSettings.distance,lensSettings.width);
        }
        else
        {
            ToggleMaterials(true);
            ToggleLens(false);
            SetupMaterialObjects();
        }
        if(calculationMode == CalculationMode.mathematic && rayTracerObject == RayTracerObject.materials)
        {
            calculationMode = CalculationMode.physics;
        }
        UpdateRayPath();
    }

    private void ToggleMaterials(bool state)
    {
        foreach(var material in materials)
        {
            if(material.generatedObject!=null)
            material.generatedObject.SetActive(state);
        }
    }

    private void ToggleLens(bool state)
    {
        biconvexLensMesh.gameObject.SetActive(state);
    }

    void UpdateRayPath()
    {
        if (rayPathCalculator == null)
            return;

        List<Vector3> points = rayPathCalculator.CalculateRayPath(rayStart, rayDirection, maxRayLength, maxBounces);
        DebugDrawer.AddPoints(points,Color.yellow,0.3f);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Material"))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
#endif
                    Destroy(child.gameObject);
            }
        }
    }

    void SetupMaterialObjects()
    {
        float yOffset = 0f;
        bool genNewMats = false;

        foreach (var mat in materials)
        {
            if (mat.generatedObject == null)
            {
                ClearChildren();
                genNewMats = true;
                break;
            }
        }

        foreach (var mat in materials)
        {
            if (genNewMats)
                GenerateVisualObject(mat);

            SetupVisualObject(ref yOffset, mat);
        }
    }

    void GenerateVisualObject(RefractiveMaterial mat)
    {
        if (mat.generate)
        {
            mat.generatedObject = new GameObject($"Material{mat.name}");
            mat.generatedObject.transform.parent = transform;
            mat.collider = mat.generatedObject.AddComponent<MeshCollider>();
            mat.collider.convex = true;
            mat.collider.sharedMesh = mat.mesh;
            mat.meshRenderer = mat.generatedObject.AddComponent<MeshRenderer>();
            mat.meshFilter = mat.generatedObject.AddComponent<MeshFilter>();
        }
    }

    void SetupVisualObject(ref float yOffset, RefractiveMaterial mat)
    {
        if (mat.generate && mat.generatedObject != null)
        {
            var obj = mat.generatedObject;
            obj.hideFlags = HideFlags.DontSave;
            obj.transform.localPosition = mat.position;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = mat.size;
            mat.meshFilter.mesh = mat.mesh;
            mat.meshRenderer.material = mat.material;
            yOffset += mat.size.y;
        }
    }
    private void OnDrawGizmos()
    {
        DebugDrawer.Clear();
        DebugDrawer.DrawGizmos(maxRayLength);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
