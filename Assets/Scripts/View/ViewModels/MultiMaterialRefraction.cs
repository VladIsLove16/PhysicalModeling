using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;
using NUnit.Framework.Internal;
using static Michsky.MUIP.RadialSlider;
using System.Net;
using System.Linq;
using System.Drawing;

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
        [HideInInspector] public BoxCollider collider;
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
    }
    [SerializeField] private bool clearDebug = false;
    [Header("Настройки луча")]
    public Vector3 rayStart = Vector3.zero;
    [Range(-89f, 89)]
    public float angle = 0f;
    public float maxRayLength = 100f;
    public int maxBounces = 10;

    [Header("Материалы (создаются автоматически)")]
    public List<RefractiveMaterial> materials = new List<RefractiveMaterial>();

    private LineRenderer lineRenderer;
    private Vector3 currentPoint2;
    private Vector3 currentPoint;
    private List<Ray> DebugRays= new List<Ray>();
    private List<Vector3> DebugPoints= new List<Vector3>();
    public Vector3 rayDirection { 
        get
        {
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Tan(rad);
            Vector3 vec = new Vector3(x, 1f,0);
            return vec.normalized;
        }
    }
    void Awake()
    {
        SetupMaterialObjects();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () => {
            if (this != null)
            {
                SetupMaterialObjects();
                UpdateRayPath();
            }
        };
    }
#endif

    void OnEnable()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        SetupMaterialObjects();
        UpdateRayPath();
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
            {
                GenerateVisualObject(mat);
            }
            SetupVisualObject(ref yOffset, mat);
        }
    }

    private void GenerateVisualObject(RefractiveMaterial mat)
    {
        if(mat.generate)
        {
            mat.generatedObject = new GameObject($"Material{mat.name}");
            mat.generatedObject.transform.parent = transform;
            mat.collider = mat.generatedObject.AddComponent<BoxCollider>();
            mat.meshRenderer = mat.generatedObject.AddComponent<MeshRenderer>();
            mat.meshFilter = mat.generatedObject.AddComponent<MeshFilter>();
        }
    }

    private void SetupVisualObject(ref float yOffset, RefractiveMaterial mat)
    {
        if (mat.generate)
        {
            GameObject obj = mat.generatedObject;
            obj.hideFlags = HideFlags.DontSave;
            //obj.transform.localPosition = Vector3.zero + new Vector3(0, yOffset + mat.size.y / 2, 0);
            obj.transform.localPosition = mat.position;
            obj.transform.localRotation = Quaternion.identity;
            yOffset += mat.size.y;
            mat.meshFilter.mesh = mat.mesh;
            Debug.Log(mat.meshFilter.sharedMesh.bounds.size);
            mat.meshRenderer.material = mat.material;

            obj.transform.localScale = mat.size;
            mat.generatedObject = obj;
        }
    }

    void UpdateRayPath()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        if (clearDebug)
        {
            DebugRays.Clear();
            DebugPoints.Clear();
        }
        ClearLine();

        List<Vector3> points = new List<Vector3>();
        Vector3 currentOrigin = rayStart;
        Vector3 currentDirection = rayDirection.normalized;
        float currentRefractiveIndex = 1.0f;

        points.Add(currentOrigin);
        for (int bounce = 0; bounce < maxBounces; bounce++)
        {
            bool didBounce = Bounce(points, ref currentOrigin, currentDirection, ref currentRefractiveIndex);
            Debug.Log("bounce " + bounce + " " + didBounce);
            if (!didBounce)
            {
                // Если не было попадания, но мы В МАТЕРИАЛЕ, симулируем выход в воздух
                if (currentRefractiveIndex != 1.0f)
                {
                    Vector3 normal = Vector3.right; // нормаль в направлении противоположном лучу
                    float nextRefractiveIndex = 1.0f;

                    if (ComputeRefractedDirection(currentDirection, normal, currentRefractiveIndex, nextRefractiveIndex, out Vector3 exitDir))
                    {
                        currentOrigin += currentDirection * 0.01f;
                        currentDirection = exitDir;
                        currentRefractiveIndex = nextRefractiveIndex;

                        points.Add(currentOrigin);
                        Debug.Log("new point");
                    }
                }
                break;
            }
        }

        points.Add(currentOrigin + currentDirection * maxRayLength);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
    private bool Bounce(List<Vector3> points, ref Vector3 currentOrigin, Vector3 currentDirection, ref float currentRefractiveIndex)
    {
        Debug.Log("currentOrigin " + currentOrigin);
        // Попадание в материал
        if (!RaycastToNextMaterial(currentOrigin, currentDirection, out RaycastHit hit, out RefractiveMaterial hitMaterial))
        {
            Debug.Log("RaycastToNextMaterial failed");
            return false;
        }
        points.Add(hit.point); // точка входа

        Vector3 normalIn = hit.normal;
        float nextRefractiveIndex = hitMaterial.refractiveIndex;    

        // Преломление при входе
        if (!ComputeRefractedDirection(currentDirection, normalIn, currentRefractiveIndex, nextRefractiveIndex, out Vector3 directionInside))
        {
            Debug.Log("ComputeRefractedDirection failed " + directionInside);
            return false;

        }

        // Выход из материала — точка выхода, направление не меняется
        if (!TryFindExitFromMaterial(hit.point, directionInside, hitMaterial, out RaycastHit exitHit))
        {
            Debug.Log("TryFindExitFromMaterial failed with dir " + directionInside);
            return false;
        }
        Vector3 insideOrigin = hit.point;

        points.Add(exitHit.point); // точка выхода

        // Обновление состояния — направление не меняется при выходе
        currentOrigin = exitHit.point;
        Debug.Log("exitHit " + exitHit);
        currentRefractiveIndex = 1.0f;

        return true;
    }

    bool RaycastToNextMaterial(Vector3 origin, Vector3 direction, out RaycastHit hit, out RefractiveMaterial material)
    {
        material = null;
        Ray ray = new Ray(origin,direction);
        DebugRays.Add(ray);
        if (Physics.Raycast(ray, out hit, maxRayLength))
        {
            DebugPoints.Add(hit.point);
            Debug.Log("Пересечение с коллайдером: " + hit.point + "  " + hit.collider.name);
            foreach (var mat in materials)
                if (mat.collider == hit.collider)
                { 
                    material = mat; 
                    return true; 
                }
        }
        return false;
    }
    bool TryFindExitFromMaterial(Vector3 originPoint, Vector3 direction, RefractiveMaterial fromMaterial, out RaycastHit hitExit)
    {
        hitExit = new RaycastHit();
        Ray ray = new Ray(originPoint,direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRayLength);
        DebugRays.Add(ray);
        foreach (RaycastHit hit in hits)
        {
            foreach (var mat in materials)
            {
                if (mat.collider == hit.collider)
                {
                    Debug.Log("Выход найден : " + hit.point);
                    hitExit = hit;
                    return true;
                }
            }
        }
        return false;
    }

    private void RaycastAll(Vector3 point, Vector3 direction, out List<RefractiveMaterial>  hitMaterials, out List<Vector3> hitPoints )
    {
        hitMaterials = new List<RefractiveMaterial>();
        hitPoints = new List<Vector3>();
        //point += 0.01f * direction;
        Ray ray = new Ray(point, direction);
        Physics.queriesHitBackfaces = true;
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRayLength);
        foreach (RaycastHit hit in hits)
        {
            hitPoints.Add(hit.point);
            Debug.Log("Пересечение с коллайдером: " + hit.point + "  " + hit.collider.name);
            foreach(var mat in materials)
            {
                if (mat.collider == hit.collider)
                {
                    DebugPoints.Add(hit.point);
                    hitMaterials.Add(mat);
                    Debug.Log("Пересечение с нужным коллайдером: " + hit.point);
                }
            }
        }
    }
    private void Raycast(Vector3 direction, RefractiveMaterial lookingMaterial, Vector3 point)
    {
        //point += 0.01f * direction;
        Ray ray = new Ray(point, direction);
        if(Physics.Raycast(ray, out RaycastHit hit, maxRayLength))
        {
            DebugPoints.Add(hit.point);
            Debug.Log("Пересечение с коллайдером: " + hit.point + "  " + hit.collider.name);
            if(lookingMaterial!=null)
                if (hit.collider == lookingMaterial.collider)
                {
                    Debug.Log("Пересечение с нужным коллайдером: " + hit.point);
                }
        }
    }

    bool ComputeRefractedDirection(Vector3 incident, Vector3 normal, float n1, float n2, out Vector3 refractedDir)
    {
        incident = incident.normalized;
        normal = normal.normalized;

        float n = n1 / n2;
        float cosI = -Vector3.Dot(normal, incident);
        float sinT2 = n * n * (1.0f - cosI * cosI);

        if (sinT2 > 1.0f)
        {
            refractedDir = Vector3.zero;
            return false;
        }

        float cosT = Mathf.Sqrt(1.0f - sinT2);
        refractedDir = n * incident + (n * cosI - cosT) * normal;
        refractedDir.Normalize();
        return true;
    }

    void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }
    private void OnDrawGizmos()
    {
        foreach(var ray in DebugRays)
        {
            Gizmos.DrawLine(ray.origin, ray.origin+ray.direction*(maxRayLength-1));
        }
        foreach(var point in DebugPoints)
        {
            Gizmos.DrawSphere(point,0.3f);
        }
    }
}
