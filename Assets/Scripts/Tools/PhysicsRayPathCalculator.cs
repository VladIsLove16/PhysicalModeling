using System.Collections.Generic;
using UnityEngine;

public class PhysicsRayPathCalculator : IRayPathCalculator
{
    private readonly List<MultiMaterialRefraction.IRefractivePhysicMaterial> materials;
    private readonly float maxRayLength;

    public PhysicsRayPathCalculator(List<MultiMaterialRefraction.IRefractivePhysicMaterial> materials, float maxRayLength)
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
        float remainingLength = maxLength;
        points.Add(currentOrigin);

        for (int bounce = 0; bounce < maxBounces; bounce++)
        {
            if (!RaycastToNextMaterial(currentOrigin, currentDirection, out RaycastHit hit, out MultiMaterialRefraction.IRefractivePhysicMaterial hitMaterial))
            {
                break;
            }                
            points.Add(hit.point);

            if (!RayPhysics.ComputeSneliusRefractedDirection(currentDirection, hit.normal, currentRefractiveIndex, hitMaterial.RefractiveIndex(), out Vector3 directionInside))
                break;

            if (!TryFindExitFromMaterial(hit.point, directionInside, hitMaterial.GetCollider(), out RaycastHit exitHit))
                break;

            points.Add(exitHit.point);
            float segmentLength = Vector3.Distance(exitHit.point, hit.point);
            remainingLength -= segmentLength;
            if (remainingLength <= 0) break;
            currentOrigin = exitHit.point;
            currentRefractiveIndex = 1.0f;
        }

        points.Add(currentOrigin + currentDirection * remainingLength);
        return points;
    }

    private bool RaycastToNextMaterial(Vector3 origin, Vector3 direction, out RaycastHit hit, out MultiMaterialRefraction.IRefractivePhysicMaterial material)
    {
        material = null;
        origin = origin-0.01f*direction;
        if (Physics.Raycast(origin, direction, out hit, maxRayLength))
        {
            foreach (var mat in materials)
            {
                if (mat.GetCollider() == hit.collider)
                {
                    DebugDrawer.AddPoint(hit.point,Color.green,0.1f);
                    material = mat;
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryFindExitFromMaterial(Vector3 originPoint, Vector3 direction, Collider lookingCollider, out RaycastHit hitExit)
    {
        hitExit = new RaycastHit();
        Vector3 rayPoint = originPoint + direction * maxRayLength;
        Ray ray = new Ray(rayPoint, -direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRayLength);

        foreach (var hit in hits)
        {
            if (lookingCollider == hit.collider)
            {
                DebugDrawer.AddPoint(hit.point, Color.red, 0.2f);
                hitExit = hit;
                return true;
            }
        }
        return false;
    }
}
