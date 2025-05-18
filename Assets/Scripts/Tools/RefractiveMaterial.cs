using System;
using UnityEngine;
using static MultiMaterialRefraction;
using static MultiMaterialRefraction.RefractiveMaterial;

public partial class MultiMaterialRefraction 
{
    public interface IRefractivePhysicMaterial
    {
        public float RefractiveIndex();
        public Collider GetCollider();
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
    }
    [System.Serializable]
    public class RefractiveMaterial : IRefractivePhysicMaterial
    {
        public string name = "Sphere";
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
