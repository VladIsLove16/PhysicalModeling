using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RampMeshGenerator : MonoBehaviour
{
    public Action regenerated;
    private const int MAXANGLE = 60;
    private const int MINANGLE = 1;
    [Range(MINANGLE, MAXANGLE)]
    [SerializeField] private float angle = 30f; // угол наклона в градусах
    [SerializeField] private float width = 2f;  // ширина пандуса (ось X)
    [SerializeField] private float length = 5f; // длина основани€ (ось Z)
    private float height; // длина основани€ (ось Z)
    private Mesh mesh;
    internal void SetAngle(float value, bool v = false)
    {
        angle = value;
        if (v)
        {
            GenerateRamp();
        }
    }
    public float Height => height;
    public Vector3 GetUpperMiddle()
    {
        return new Vector3(-width, height, length/2);      // середина верхушки
    }
    public Vector3 GetDownMiddle()
    {
        return new Vector3(-width, 0, length/2);      // середина верхушки
    }
    public Vector3 GetBackMiddle()
    {
        return new Vector3(-width, height/2, length/2);      // середина верхушки
    }

    public Vector3 GetDownRight()
    {
        return new Vector3(width, 0, length);      // нижний правый 
    }
    public Vector3 GetUpperRight()
    {
        return new Vector3(width, height, length);      // верхний правый 
    }
    public Vector3 GetCenterOnIncline()
    {
        return new Vector3(-width/2, height/2, length/2);      // верхний правый 
    }

    [ContextMenu("MoveZero")]
    public void Move()
    {
        Move(Vector3.zero);
    }
    public void Move(Vector3 to)
    {
        Vector3 result = to - GetCenterOnIncline();
        transform.position = result;
    }

    [ContextMenu("GenerateRamp")]
    void GenerateRamp()
    {
        height = Mathf.Tan(angle * Mathf.Deg2Rad) * width;

        Vector3[] vertices = new Vector3[6]
        {
            new Vector3(0, 0, 0),                // 0: нижний левый
            new Vector3(0, 0, length),            // 1: нижний правый
            new Vector3(-width, 0, 0),      // 2: нижний левый (угол)
            new Vector3(-width, 0, length),  // 3: нижний правый (угол)
            new Vector3(-width, height, 0),           // 4: верхний левый 
            new Vector3(-width, height, length)        // 5: верхний правый 
        };

        int[] triangles = new int[]
        {
            // Ѕокова€ грань слева
            0, 2, 4,
            // Ѕокова€ грань справа
            1, 5, 3,
            // ќснование
            0, 1, 5,
            0, 5, 4,
            // Ќаклонна€ плоскость
            2, 3, 5,
            2, 5, 4,
            // ¬ерхн€€ задн€€ грань
            2, 0, 1,
            2, 1, 3
        };

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        regenerated?.Invoke();
    }
}
