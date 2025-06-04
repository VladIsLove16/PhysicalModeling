using UnityEngine;

public static class RayPhysics
{
    public static bool ComputeSneliusRefractedDirection(Vector3 incident, Vector3 normal, float n1, float n2, out Vector3 refractedDir)
    {
        incident = incident.normalized;
        normal = normal.normalized;

        float n = n1 / n2;
        float cosI = -Vector3.Dot(normal, incident);
        float sinT2 = n * n * (1.0f - cosI * cosI);

        if (sinT2 > 1.0f)
        {
            refractedDir = incident - 2.0f * Vector3.Dot(incident, normal) * normal;
            return true;
        }

        float cosT = Mathf.Sqrt(1.0f - sinT2);
        refractedDir = n * incident + (n * cosI - cosT) * normal;
        refractedDir.Normalize();
        return true;
    }

    public static bool RayLensSurfaceIntersect(Vector3 rayOrigin, Vector3 rayDir, Vector3 sphereCenter, float radius, float yMin, float yMax, out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;

        Vector3 oc = rayOrigin - sphereCenter;
        float a = Vector3.Dot(rayDir, rayDir);
        float b = 2.0f * Vector3.Dot(oc, rayDir);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
            return false;

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrtDiscriminant) / (2 * a);
        float t2 = (-b + sqrtDiscriminant) / (2 * a);

        // Проверим оба варианта пересечения
        float[] tCandidates = new float[] { t1, t2 };

        foreach (float t in tCandidates)
        {
            if (t <= 0)
                continue;

            Vector3 point = rayOrigin + t * rayDir;
            float y = point.y;

            if (y >= yMin && y <= yMax)
            {
                hitPoint = point;
                return true;
            }
        }

        return false;
    }


    public static bool RayBoxIntersect(Ray ray, Bounds bounds, out Vector3 intersectionPoint)
    {
        intersectionPoint = Vector3.zero;

        Vector3 origin = ray.origin;
        Vector3 direction = ray.direction;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        float tmin = (min.x - origin.x) / direction.x;
        float tmax = (max.x - origin.x) / direction.x;
        if (tmin > tmax) Swap(ref tmin, ref tmax);

        float tymin = (min.y - origin.y) / direction.y;
        float tymax = (max.y - origin.y) / direction.y;
        if (tymin > tymax) Swap(ref tymin, ref tymax);

        if (tmin > tymax || tymin > tmax)
            return false;

        if (tymin > tmin)
            tmin = tymin;
        if (tymax < tmax)
            tmax = tymax;

        float tzmin = (min.z - origin.z) / direction.z;
        float tzmax = (max.z - origin.z) / direction.z;
        if (tzmin > tzmax) Swap(ref tzmin, ref tzmax);

        if (tmin > tzmax || tzmin > tmax)
            return false;

        if (tzmin > tmin)
            tmin = tzmin;
        if (tzmax < tmax)
            tmax = tzmax;

        if (tmax < 0)
            return false;

        float t = tmin >= 0 ? tmin : tmax;
        if (t < 0)
            return false;

        intersectionPoint = origin + t * direction;
        return true;
    }

    private static void Swap(ref float a, ref float b)
    {
        float temp = a;
        a = b;
        b = temp;
    }
}