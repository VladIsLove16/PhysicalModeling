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

    public List<Vector3> CalculateRayPath(Vector3 start, Vector3 rayDirection, float maxLength, int maxBounces)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 currentOrigin = start;
        rayDirection = rayDirection.normalized;
        float remainingLength = maxLength;
        points.Add(currentOrigin);

        for (int bounce = 0; bounce < maxBounces; bounce++)
        {

            if (!RaycastToNextMaterial(currentOrigin, rayDirection, out RaycastHit hit, out MultiMaterialRefraction.IRefractivePhysicMaterial hitMaterial))
                break;
            DebugDrawer.AddRay(new Ray(hit.point + 0.01f * Vector3.one, rayDirection), Color.blue);
            DebugDrawer.AddRay(new Ray(hit.point, -hit.normal), Color.red);

            points.Add(hit.point);

            if (!RayPhysics.ComputeSneliusRefractedDirection(rayDirection, hit.normal, IRayPathCalculator.AIRREFRACTION, hitMaterial.RefractiveIndex(), out Vector3 directionInside))
                break;
            DebugDrawer.AddRay(new Ray(hit.point, directionInside), Color.yellow);

            if (!TryFindExitFromMaterial(hit.point, directionInside, hitMaterial.GetCollider(), out RaycastHit exitHit))
                break;

            if (!RayPhysics.ComputeSneliusRefractedDirection(directionInside, -exitHit.normal, hitMaterial.RefractiveIndex(), IRayPathCalculator.AIRREFRACTION, out Vector3 directionOutside))
                break;

            DebugDrawer.AddRay(new Ray(exitHit.point + 0.01f * Vector3.one, directionInside), Color.blue);
            DebugDrawer.AddRay(new Ray(exitHit.point, exitHit.normal), Color.red);
            DebugDrawer.AddRay(new Ray(exitHit.point, directionOutside), Color.yellow);
            points.Add(exitHit.point);
            CalculateRayLegth(ref remainingLength, hit, ref exitHit);
            if (remainingLength <= 0)
                break;
            currentOrigin = exitHit.point;
            rayDirection = directionOutside;
        }

        points.Add(currentOrigin + rayDirection * remainingLength);
        return points;
    }

    private static void CalculateRayLegth(ref float remainingLength, RaycastHit hit, ref RaycastHit exitHit)
    {
        float segmentLength = Vector3.Distance(exitHit.point, hit.point);
        remainingLength -= segmentLength;
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
