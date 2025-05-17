using UnityEngine;
using System.Collections.Generic;

public class LensRayTracer : IRayPathCalculator
{
    public float radius = 5f;
    public float lensRefractiveIndex = 1.5f;

    private Vector3 centerFront;
    private Vector3 centerBack;
    public LensRayTracer(
        float radius,
        float distance,///between sphere centers
        float lensRefractiveIndex)
    {
        this.radius = radius;
        this.lensRefractiveIndex = lensRefractiveIndex;
        centerFront = new Vector3(0, -radius, 0);
        centerBack = new Vector3(0, radius,  0);
    }
    public List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces, float initialRefractiveIndex = 1.0f)
    {
        List<Vector3> points = new List<Vector3> { start };

        Vector3 currentPos = start;
        Vector3 currentDir = direction.normalized;
        float remainingLength = maxLength;
        float n1 = initialRefractiveIndex;
        float n2 = lensRefractiveIndex;

        for (int bounce = 0; bounce < maxBounces && remainingLength > 0; bounce++)
        {
            Vector3 nextHit;
            Vector3 normal;
            // Step 1: Intersect with front surface
            if (bounce == 0)
            {
                DebugDrawer.AddRay(new Ray(currentPos, currentDir));
                if (!RaySphereIntersect(currentPos, currentDir, centerFront, radius, out nextHit))
                    break;
                normal = (nextHit - centerFront).normalized;
            }
            // Step 2: Intersect with back surface (inside lens)
            else if (bounce == 1)
            {
                DebugDrawer.AddRay(new Ray(currentPos, currentDir));
                if (!RaySphereIntersect(currentPos, currentDir, centerBack,radius , out nextHit))
                    break;
                normal = -(nextHit - centerBack).normalized; // нормаль внутрь линзы
                (n1, n2) = (n2, 1.0f); // из стекла в воздух
            }
            else break;

            // Step 3: Преломление
            Vector3 refracted;
            if (!Refract(currentDir, normal, n1 / n2, out refracted))
                break; // Полное внутреннее отражение — остановка

            float segmentLength = Vector3.Distance(currentPos, nextHit);
            remainingLength -= segmentLength;
            if (remainingLength <= 0) break;

            AddPoint(points, nextHit);
            currentPos = nextHit;
            currentDir = refracted;
        }

        // Добавляем конец луча
        AddPoint(points,currentPos + currentDir * remainingLength);
        return points;
    }
    private void AddPoint(List<Vector3> points,Vector3 point)
    {
        points.Add(point);
        DebugDrawer.AddPoint(point, Color.yellow, 0.3f);
    }
    // Пересечение луча со сферой
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

    // Закон Снеллиуса: расчет преломленного луча
    bool Refract(Vector3 incident, Vector3 normal, float eta, out Vector3 refracted)
    {
        float cosi = Mathf.Clamp(Vector3.Dot(-incident, normal), -1f, 1f);
        float sin2t = eta * eta * (1f - cosi * cosi);

        if (sin2t > 1f)
        {
            refracted = Vector3.zero;
            return false; // полное внутреннее отражение
        }

        refracted = eta * incident + (eta * cosi - Mathf.Sqrt(1f - sin2t)) * normal;
        refracted.Normalize();
        return true;
    }
}
