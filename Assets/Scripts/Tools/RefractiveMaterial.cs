using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static MultiMaterialRefraction;
using static MultiMaterialRefraction.RefractiveMaterial;

public partial class MultiMaterialRefraction 
{
    public interface IRefractivePhysicMaterial
    {
        public float RefractiveIndex();
        public Collider GetCollider();
        public List<ISurface> GetSurfaces();
    }
    public struct LensSurface : ISurface
    {
        public float radius;
        public float thickness;
        public Vector3 center;
        public float refractiveIndex;
        public float RefractiveIndex
        {
            get
            {
                return refractiveIndex;
            }
            set
            {
                refractiveIndex = value;
            }
        }
        public LensSurface(Vector3 center, float radius, float refractiveIndex, float thickness)
        {
            this.center = center;
            this.radius = radius;
            this.refractiveIndex = refractiveIndex;
            this.thickness = -thickness;
        }
        public bool GetIntersection(Vector3 start, Vector3 direction,  out Vector3 intersectionPoint)
        {
            float yMax, yMin;
            if (thickness < 0)
            {
                yMax = center.y - radius - thickness / 2;
                yMin = center.y - radius;
            }
            else
            {
                 yMax = center.y + radius;
                 yMin = center.y + radius  - thickness / 2;
            }
            DebugDrawer.AddPoint(new Vector3(center.x, yMax, center.z), Color.black, 0.5f);
            DebugDrawer.AddPoint(new Vector3(center.x, yMin, center.z),Color.gray,0.4f);
            return RayPhysics.RayLensSurfaceIntersect(start, direction, center, radius, yMin, yMax, out intersectionPoint);
        }
        public Vector3 GetNormal(Vector3 intersectionPoint)
        {
            return (intersectionPoint - center).normalized;
        }
    }

    [System.Serializable]
    public struct CubeSurface : ISurface
    {
        public Vector3 normal;
        public Vector3 position;
        public Vector2 size;
        public float refractiveIndex;

        public float RefractiveIndex
        {
            get => refractiveIndex;
            set => refractiveIndex = value;
        }

        public CubeSurface(Vector3 normal, Vector3 position, Vector2 size, float refractiveIndex)
        {
            this.normal = normal.normalized;
            this.position = position;
            this.size = size;
            this.refractiveIndex = refractiveIndex;
        }

        public bool GetIntersection(Vector3 start, Vector3 direction, out Vector3 intersectionPoint)
        {
            return RayPhysics.RayBoxIntersect(new Ray(start, direction), new Bounds(position, size), out intersectionPoint);
        }

        public Vector3 GetNormal(Vector3 intersectionPoint)
        {
            return normal;
        }
    }

    [System.Serializable]
    public class RefractiveLens : IRefractivePhysicMaterial
    {
        [SerializeField] public MeshCollider collider;
        public float refractiveIndex = 1.5f;
        public Vector3 position;
        public float radius;
        public float distance;
        public float width;
        public Collider GetCollider()
        {
            return collider;
        }
        public float RefractiveIndex()
        {
            return refractiveIndex;
        }
        List<ISurface> IRefractivePhysicMaterial.GetSurfaces()
        {
            float thickness = radius * 2 - distance;
            var centerFront = new Vector3(0, radius - thickness / 2, 0) + position;
            var centerBack = new Vector3(0, -radius + thickness / 2f, 0) + position;
            LensSurface lensSurface1 = new LensSurface(centerFront, radius, refractiveIndex, thickness);
            LensSurface lensSurface2 = new LensSurface(centerBack, radius, refractiveIndex, -thickness);
            List<LensSurface> surfaces = new List<LensSurface>()
            {
                lensSurface1,
                lensSurface2
            };
            var SurfacesList =  surfaces.Cast <ISurface>().ToList(); 
            return SurfacesList;
        }
    }
    [System.Serializable]
    public class RefractiveMaterial : IRefractivePhysicMaterial
    {
        public string name = "Cube";
        public Vector3 size = new Vector3(1, 1, 1);
        public Vector3 position = new Vector3(0, 0, 0);
        public float refractiveIndex = 1.5f;
        public Material material;
        public Mesh mesh;
        public bool generate = false;
        public FormType formType = FormType.lens;
        public GameObject generatedObject = null;
        [HideInInspector] public MeshCollider collider;
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;

        public Collider GetCollider()
        {
            return collider;
        }

        public List<ISurface> GetSurfaces()
        {
            List<ISurface> surfaces = new List<ISurface>();
            Vector3 halfSize = size * 0.5f;

            Vector3[] normals = {
        Vector3.forward, Vector3.back,
        Vector3.left, Vector3.right,
        Vector3.up, Vector3.down
    };

            Vector3[] offsets = {
        new Vector3(0, 0, halfSize.z), new Vector3(0, 0, -halfSize.z),
        new Vector3(-halfSize.x, 0, 0), new Vector3(halfSize.x, 0, 0),
        new Vector3(0, halfSize.y, 0), new Vector3(0, -halfSize.y, 0)
    };

            Vector2[] sizes = {
        new Vector2(size.x, size.y), new Vector2(size.x, size.y),
        new Vector2(size.z, size.y), new Vector2(size.z, size.y),
        new Vector2(size.x, size.z), new Vector2(size.x, size.z)
    };

            for (int i = 0; i < 6; i++)
            {
                CubeSurface surface = new CubeSurface(normals[i], position + offsets[i], sizes[i], refractiveIndex);
                surfaces.Add(surface);
            }

            return surfaces;
        }

        public float RefractiveIndex()
        {
            return refractiveIndex;
        }
    }
    
    public enum FormType
    {
        lens,
        material
    }
}
