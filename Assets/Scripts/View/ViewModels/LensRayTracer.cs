using UnityEngine;
using System.Collections.Generic;

public class LensRayTracer : IRayPathCalculator
{
    public float radius = 5f;
    public float lensRefractiveIndex;

    private Vector3 centerFront;
    private Vector3 centerBack;

    public LensRayTracer(float radius, float distance, float lensRefractiveIndex,Vector3 lensPosition)
    {
        this.radius = radius;
        this.lensRefractiveIndex = lensRefractiveIndex;

        // Используем расстояние между центрами сфер
        float thickness = radius * 2 - distance;
        centerFront = new Vector3(0, radius - thickness / 2, 0) + lensPosition;
        centerBack = new Vector3(0, -radius + thickness / 2f, 0) + lensPosition;
    }

    public List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces, float initialRefractiveIndex = 1.0f)
    {
        List<Vector3> points = new List<Vector3> { start };

        Vector3 currentPos = start;
        Vector3 currentDir = direction.normalized;
        float remainingLength = maxLength;

        for (int bounce = 0; bounce < maxBounces && remainingLength > 0; bounce++)
        {
            Vector3 nextHit, normal;
            float n1, n2;

            if (bounce == 0)
            {
                if (!RayPhysics.RaySphereIntersect(currentPos, currentDir, centerFront, radius, out nextHit))
                    break;

                normal = (nextHit - centerFront).normalized;
                n1 = initialRefractiveIndex;
                n2 = lensRefractiveIndex;
            }
            else if (bounce == 1)
            {
                if (!RayPhysics.RaySphereIntersect(currentPos, currentDir, centerBack, radius, out nextHit))
                    break;

                normal = (nextHit - centerBack).normalized;
                n1 = lensRefractiveIndex;
                n2 = initialRefractiveIndex;
            }
            else break;
            // Автоматическая ориентация нормали
            if (Vector3.Dot(currentDir, normal) > 0)
                normal = -normal;

            DebugDrawer.AddRay(new Ray(nextHit, -normal), Color.red);   
            DebugDrawer.AddRay(new Ray(currentPos+0.01f *Vector3.one, currentDir), Color.blue);
            Vector3 refracted;
            if (!RayPhysics.ComputeSneliusRefractedDirection(currentDir, normal, n1 , n2, out refracted))
                break; // Полное внутреннее отражение

            DebugDrawer.AddRay(new Ray(nextHit, refracted), Color.yellow);
            float segmentLength = Vector3.Distance(currentPos, nextHit);
            remainingLength -= segmentLength;
            if (remainingLength <= 0) break;

            AddPoint(points, nextHit);
            currentPos = nextHit;
            currentDir = refracted;
        }

        AddPoint(points, currentPos + currentDir * remainingLength);
        return points;
    }

    private void AddPoint(List<Vector3> points, Vector3 point)
    {
        points.Add(point);
        DebugDrawer.AddPoint(point, Color.green, 0.2f);
    }

}
