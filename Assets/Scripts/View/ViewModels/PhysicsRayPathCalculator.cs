using System.Collections.Generic;
using UnityEngine;

public class PhysicsRayPathCalculator : IRayPathCalculator
{
    private readonly List<MultiMaterialRefraction.RefractiveMaterial> materials;
    private readonly float maxRayLength;

    public PhysicsRayPathCalculator(List<MultiMaterialRefraction.RefractiveMaterial> materials, float maxRayLength)
    {
        this.materials = materials;
        this.maxRayLength = maxRayLength;
    }

    public List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces, float initialRefractiveIndex = 1.0f)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 currentOrigin = start;
        Vector3 currentDirection = direction.normalized;
        float currentRefractiveIndex = initialRefractiveIndex;

        points.Add(currentOrigin);

        for (int bounce = 0; bounce < maxBounces; bounce++)
        {
            if (!RaycastToNextMaterial(currentOrigin, currentDirection, out RaycastHit hit, out var hitMaterial))
            {

                break;
            }                
                

            points.Add(hit.point);

            if (!ComputeRefractedDirection(currentDirection, hit.normal, currentRefractiveIndex, hitMaterial.refractiveIndex, out Vector3 directionInside))
                break;

            if (!TryFindExitFromMaterial(hit.point, directionInside, hitMaterial, out RaycastHit exitHit))
                break;

            points.Add(exitHit.point);

            currentOrigin = exitHit.point;
            currentDirection = directionInside;
            currentRefractiveIndex = 1.0f;
        }

        points.Add(currentOrigin + currentDirection * maxLength);
        return points;
    }

    private bool RaycastToNextMaterial(Vector3 origin, Vector3 direction, out RaycastHit hit, out MultiMaterialRefraction.RefractiveMaterial material)
    {
        material = null;
        if (Physics.Raycast(origin, direction, out hit, maxRayLength))
        {
            foreach (var mat in materials)
            {
                if (mat.collider == hit.collider)
                {
                    material = mat;
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryFindExitFromMaterial(Vector3 originPoint, Vector3 direction, MultiMaterialRefraction.RefractiveMaterial fromMaterial, out RaycastHit hitExit)
    {
        hitExit = new RaycastHit();
        Vector3 rayPoint = originPoint + direction * maxRayLength;
        Ray ray = new Ray(rayPoint, -direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRayLength);

        foreach (var hit in hits)
        {
            if (fromMaterial.collider == hit.collider)
            {
                hitExit = hit;
                return true;
            }
        }
        return false;
    }

    private bool ComputeRefractedDirection(Vector3 incident, Vector3 normal, float n1, float n2, out Vector3 refractedDir)
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
}
