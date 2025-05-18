using UnityEngine;
using System.Collections.Generic;
using System;
using static MultiMaterialRefraction;
using static DebugDrawer;
using System.Linq;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public partial class MultiMaterialRefraction : MonoBehaviour
{
    [System.Serializable]
    public struct LensSettings
    {
       
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
    [SerializeField] private bool clearDebugOnDraw;
    [Header("Настройки луча")]
    public Vector3 rayStart = Vector3.zero;
    [Range(-89f, 89)]
    public float angle = 0f;
    public float maxRayLength = 100f;
    public int maxBounces = 10;

    [SerializeField] private bool isEnabled= false;
    private LineRenderer lineRenderer;
    private IRayPathCalculator rayPathCalculator;
    public CalculationMode calculationMode =  CalculationMode.physics;
    public RayTracerObject rayTracerObject = RayTracerObject.materials;

    [Header("Материалы (создаются автоматически)")]
    public List<RefractiveMaterial> materials = new List<RefractiveMaterial>();
    [Header("Настройки линзы")]
    public List<RefractiveLens> lensMaterials = new List<RefractiveLens>();
    public BiconvexLensGenerator biconvexLensMesh;
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
        isEnabled = false;
        SetupMaterialObjects();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (isEnabled)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    OnEnabled();
                }
            };
        }
    }
#endif

    public void OnDisabled()
    {
        isEnabled = false;
        gameObject.SetActive(false);
    }

    public void OnEnabled()
    {
        isEnabled = true;
        gameObject.SetActive(true);
        lineRenderer = GetComponent<LineRenderer>();
        RefractiveLens lens = lensMaterials[0];
        SetupRayTracer();
        SetupMaterials(lens);
        UpdateRayPath();
    }

    private void SetupMaterials(RefractiveLens lens)
    {
        if (rayTracerObject == RayTracerObject.lens)
        {
            ToggleMaterials(false);
            ToggleLens(true);
            biconvexLensMesh.GenerateLensMesh(lens.radius, lens.distance, lens.width, lens.position);
        }
        else
        {
            ToggleMaterials(true);
            ToggleLens(false);
            SetupMaterialObjects(); 
        }
    }

    private void SetupRayTracer()
    {
        if (calculationMode == CalculationMode.mathematic && rayTracerObject == RayTracerObject.materials)
        {
            calculationMode = CalculationMode.physics;
        }
        if (calculationMode == CalculationMode.physics)
        {
            List<IRefractivePhysicMaterial> Imaterials = materials.Cast<IRefractivePhysicMaterial>().ToList();
            //List<IRefractivePhysicMaterial> Ilensmaterials = lensMaterials.Cast<IRefractivePhysicMaterial>().ToList();
            List<IRefractivePhysicMaterial> Ilensmaterials = lensMaterials.Cast<IRefractivePhysicMaterial>().ToList();
            List<IRefractivePhysicMaterial> IRefractiveMaterials = new();
            IRefractiveMaterials.AddRange(Imaterials);
            IRefractiveMaterials.AddRange(Ilensmaterials);
            rayPathCalculator = new PhysicsRayPathCalculator(IRefractiveMaterials, maxRayLength);
        }
        else
        {
            RefractiveLens lens = lensMaterials[0];
            rayPathCalculator = new RayTracer(lens.radius, lens.distance, lens.refractiveIndex, lens.position);
        }

       
    }

    public void SetAngle(float angle)
    {
        this.angle = angle;
        UpdateRayPath();
    }

    public void SetLensRadius(float value)
    {
        biconvexLensMesh.SetRadius(value);
        UpdateRayPath();
    }

    public void SetLensDistance(float value)
    {
        biconvexLensMesh.SetDistance(value);
        SetupRayTracer();
        UpdateRayPath();
    }

    public void SetLensPosition(Vector3 value)
    {
        biconvexLensMesh.SetPosition(value); 
    }
    public void SetLensRefractiveIndex(float value)
    {
        lensMaterials[0].refractiveIndex = value;
    }

    public void SetCalculationMode(CalculationMode calculationMode)
    {
        this.calculationMode = calculationMode;
        SetupRayTracer();
        UpdateRayPath();
    }
    public void SetTracerObject (RayTracerObject rayTracerObject)
    {
        this.rayTracerObject = rayTracerObject;
        SetupMaterials(lensMaterials[0]);
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
        if (biconvexLensMesh != null)
            biconvexLensMesh.gameObject.SetActive(state);   
    }

    void UpdateRayPath()
    {
        if (rayPathCalculator == null)
            return;
        DebugDrawer.Clear();

        List<Vector3> points = rayPathCalculator.CalculateRayPath(rayStart, rayDirection, maxRayLength, maxBounces);
        DebugDrawer.AddPoints(points,Color.yellow,0.2f);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Sphere"))
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
            if (genNewMats &&  mat.formType == FormType.material)
                GenerateVisualObject(mat);
            if(mat.formType == FormType.material)
                SetupVisualObject(ref yOffset, mat);
        }
    }

    void GenerateVisualObject(RefractiveMaterial mat)
    {
        if (mat.generate)
        {
            mat.generatedObject = new GameObject($"Sphere{mat.name}");
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
        DebugDrawer.DrawGizmos(maxRayLength);
    }
}
