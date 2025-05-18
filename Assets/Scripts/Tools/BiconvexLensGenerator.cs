using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BiconvexLensGenerator : MonoBehaviour
{
    [Header("Геометрия линзы")]
    public float radius = 2f;         // Радиус каждой сферы
    public float distance = 2f;       // Расстояние между центрами сфер (по X)
    public float width = 0.5f;      // Расстояние между центрами сфер (по X)
    public Vector3 position;        // Ширина линзы (экструзия по Z)
    public int arcSegments = 64;      // Кол-во сегментов дуги
    MeshFilter mf;    // Кол-во сегментов дуги
    MeshFilter MeshFilter
    {
        get { if (mf == null)
                mf = GetComponent<MeshFilter>(); 
            return mf;
        }
    }
    new MeshCollider collider;
    MeshCollider Collider
    {
        get
        {
            if (collider == null)
                collider = GetComponent<MeshCollider>();
            return collider;
        }
    }
    public void OnValidate()
    {
        GenerateLensMesh();
    }
    public void GenerateLensMesh(float radius,float distance,float witdh, Vector3 position)
    {
        this.distance = distance;
        this.width = witdh;
        this.radius = radius;
        this.position = position;
        GenerateLensMesh();
    }

    [ContextMenu("genLens")]
    void GenerateLensMesh()
    {
        try
        {
            List<Vector2> profile = new List<Vector2>();
            float halfWidth = width / 2f;
            float halfDist = distance / 2f;

            Vector2 centerLeft = new Vector2(-halfDist, 0);
            Vector2 centerRight = new Vector2(halfDist, 0);

            // ==== Верхняя дуга (левая сфера, правая часть) ====
            for (int i = 0; i <= arcSegments; i++)
            {
                float t = (float)i / arcSegments;
                float angle = Mathf.Lerp(-Mathf.Acos(halfDist / radius), Mathf.Acos(halfDist / radius), t);
                float x = centerLeft.x + Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                profile.Add(new Vector2(x, y));
            }

            // ==== Нижняя дуга (правая сфера, левая часть) ====
            for (int i = arcSegments; i >= 0; i--)
            {
                float t = (float)i / arcSegments;
                float angle = Mathf.Lerp(-Mathf.Acos(halfDist / radius), Mathf.Acos(halfDist / radius), t);
                float x = centerRight.x - Mathf.Cos(angle) * radius;
                float y = -Mathf.Sin(angle) * radius;
                profile.Add(new Vector2(x, y));
            }

            int profileCount = profile.Count;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // ==== Экструдируем вдоль Z ====
            for (int i = 0; i < profileCount; i++)
            {
                Vector2 p = profile[i];
                vertices.Add(new Vector3(p.x, p.y, -halfWidth)); // передняя сторона
            }
            for (int i = 0; i < profileCount; i++)
            {
                Vector2 p = profile[i];
                vertices.Add(new Vector3(p.x, p.y, halfWidth)); // задняя сторона
            }

            // ==== Треугольники: фронт и бэк ====
            for (int i = 0; i < profileCount - 1; i++)
            {
                // Перед
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add((i + 1) % profileCount);

                // Зад
                int offset = profileCount;
                triangles.Add(offset + i + 1);
                triangles.Add(offset + i);
                triangles.Add(offset + ((i + 1) % profileCount));
            }

            // ==== Боковины ====
            for (int i = 0; i < profileCount - 1; i++)
            {
                int a = i;
                int b = i + 1;
                int c = i + profileCount;
                int d = b + profileCount;

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);

                triangles.Add(b);
                triangles.Add(c);
                triangles.Add(d);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.name = "BiconvexLens";
            MeshFilter.mesh = mesh;
            Collider.sharedMesh = mesh;
            gameObject.transform.position = position;
        }
        catch (Exception e)
        {
            Debug.LogAssertion("LensMesh Generation failed with " + e.ToString());
        }
    }

    internal void SetRadius(object value)
    {
        radius = (float)value;
        GenerateLensMesh();
    }

    internal void SetDistance(object value)
    {
        distance = (float)value;
        GenerateLensMesh();
    }

    internal void SetPosition(object value)
    {
       position = (Vector3)   value; 
        GenerateLensMesh();
    }

}
