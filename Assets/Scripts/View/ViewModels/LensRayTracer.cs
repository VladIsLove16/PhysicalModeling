using UnityEngine;
using System.Collections.Generic;

public class LensRayTracer : IRayPathCalculator
{
    public float radius = 5f;
    public float lensRefractiveIndex = 1.5f;

    private Vector3 centerFront;
    private Vector3 centerBack;

    public LensRayTracer(float radius, float distance, float lensRefractiveIndex)
    {
        this.radius = radius;
        this.lensRefractiveIndex = lensRefractiveIndex;

        // Используем расстояние между центрами сфер
        float thickness = radius * 2 - distance;
        centerFront = new Vector3(0, radius - thickness/2, 0);
        centerBack = new Vector3(0, -radius + thickness / 2f, 0);
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
                if (!RaySphereIntersect(currentPos, currentDir, centerFront, radius, out nextHit))
                    break;

                normal = (nextHit - centerFront).normalized;
                n1 = initialRefractiveIndex;
                n2 = lensRefractiveIndex;
            }
            else if (bounce == 1)
            {
                if (!RaySphereIntersect(currentPos, currentDir, centerBack, radius, out nextHit))
                    break;

                normal = (nextHit - centerBack).normalized;
                n1 = lensRefractiveIndex;
                n2 = initialRefractiveIndex;
            }
            else break;

            // Автоматическая ориентация нормали
            if (Vector3.Dot(currentDir, normal) > 0)
                normal = -normal;

            Vector3 refracted;
            if (!Refract(currentDir, normal, n1 / n2, out refracted))
                break; // Полное внутреннее отражение

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

    bool RaySphereIntersect(Vector3 rayOrigin, Vector3 rayDir, Vector3 sphereCenter, float radius, out Vector3 hitPoint)
    {
        Vector3 oc = rayOrigin - sphereCenter;
        float a = Vector3.Dot(rayDir, rayDir);
        float b = 2.0f * Vector3.Dot(oc, rayDir);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            hitPoint = Vector3.zero;
            return false;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrtDiscriminant) / (2 * a);
        float t2 = (-b + sqrtDiscriminant) / (2 * a);
        float t = (t1 > 0) ? t1 : t2;

        if (t > 0)
        {
            hitPoint = rayOrigin + t * rayDir;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }

    bool Refract(Vector3 incident, Vector3 normal, float eta, out Vector3 refracted)
    {
        float cosi = Mathf.Clamp(Vector3.Dot(incident, normal), -1f, 1f);
        float sin2t = eta * eta * (1f - cosi * cosi);

        if (sin2t > 1f)
        {
            refracted = Vector3.zero;
            return false;
        }

        refracted = eta * incident + (eta * cosi - Mathf.Sqrt(1f - sin2t)) * normal;
        refracted.Normalize();
        return true;
    }
}
