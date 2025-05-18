using UnityEngine;
using System.Collections.Generic;
using System;

public static class DebugDrawer
{
    public struct DebugPoint
    {
        public Vector3 pos;
        public Color color;
        public float radius;
        public DebugPoint(Vector3 pos, Color color, float radius)
        {
            this.pos = pos;
            this.color = color;
            this.radius = radius;
        }
    }
    public struct DebugRay
    {
        public Color color;
        public Ray Ray;
        public DebugRay(Ray Ray, Color color)
        {
            this.color = color;
            this.Ray = Ray;
        }
    }
    public static readonly List<DebugRay> debugRays = new List<DebugRay>();
    public static readonly List<DebugPoint> debugPoints = new List<DebugPoint>();
    public static void AddRay(Ray ray, Color color)
    {
        debugRays.Add(new DebugRay(ray, color));
    }

    public static void AddPoint(Vector3 position, Color color, float radius = 0.3f)
    {
        debugPoints.Add(new DebugPoint(position, color, radius));
    }

    public static void Clear()
    {
        debugRays.Clear();
        debugPoints.Clear();
    }

    public static void DrawGizmos(float rayLength)
    {
        Gizmos.color = Color.white;
        foreach (var ray in debugRays)
        {
            Gizmos.color = ray.color;
            Gizmos.DrawLine(ray.Ray.origin, ray.Ray.origin + ray.Ray. direction * (rayLength - 1));
        }
        foreach (var point in debugPoints)
        {
            Gizmos.color = point.color;
            Gizmos.DrawSphere(point.pos, point.radius);
        }
    }

    internal static void AddPoints(List<Vector3> points, Color color, float radius = 0.3f)
    {
        foreach (var point in points)
        {
            AddPoint(point, color, radius);
        }
    }
}