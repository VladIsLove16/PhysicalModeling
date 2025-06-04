using UnityEngine;
using System.Collections.Generic;
using UnityEngine.LowLevelPhysics;
using Unity.VisualScripting;

public partial class MathematicalRayPathCalculator : IRayPathCalculator
{
   
    private List<ISurface> _surfaces = new List<ISurface>();
    private List<MultiMaterialRefraction.IRefractivePhysicMaterial> refractiveMaterials;
    private float maxRayLength;


    public MathematicalRayPathCalculator(List<MultiMaterialRefraction.IRefractivePhysicMaterial> refractiveMaterials, float maxRayLength)
    {
        this.refractiveMaterials = refractiveMaterials;
        this.maxRayLength = maxRayLength;

        foreach(var  material in refractiveMaterials)
        {
            _surfaces.AddRange(material.GetSurfaces());
        }
    }

    public List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces)
    {
        List<Vector3> points = new List<Vector3> { start };

        Vector3 currentPos = start;
        Vector3 currentDir = direction.normalized;
        float remainingLength = maxLength;
        float n1, n2;
        for (int bounce = 0; bounce < maxBounces; bounce++)
        {
            for (int i = 0; i < _surfaces.Count; i++)
            {
                Vector3 nextHit, normal;
                if (!_surfaces[i].GetIntersection(currentPos, currentDir, out nextHit))
                {
                    Debug.Log("GetIntersection not foubd on surface " + i + " continue");
                    continue;
                }
                else
                    Debug.Log("GetIntersection  found on surface " + i);
                normal = _surfaces[i].GetNormal(nextHit);
                
                n1 = bounce % 2 == 0 ? IRayPathCalculator.AIRREFRACTION : _surfaces[i].RefractiveIndex;
                n2 = bounce % 2 == 0 ? _surfaces[i].RefractiveIndex : IRayPathCalculator.AIRREFRACTION;
                Debug.Log("bounce + " + bounce + " with n1 : " + n1 + " n2 : " + n2 + " on surface " + i  + " in  " + nextHit);
                //ориентация нормали
                if (Vector3.Dot(currentDir, normal) > 0)
                    normal = -normal;

                DebugDrawer.AddRay(new Ray(nextHit, -normal), Color.red);
                DebugDrawer.AddRay(new Ray(currentPos + 0.01f * Vector3.one, currentDir), Color.blue);
                Vector3 refracted;
                if (!RayPhysics.ComputeSneliusRefractedDirection(currentDir, normal, n1, n2, out refracted))
                {
                    Debug.LogAssertion("Full refraction on surface " + i);
                    break; // Полное внутреннее отражение
                }

                DebugDrawer.AddRay(new Ray(nextHit, refracted), Color.yellow);
                remainingLength = CalculateRamainingLength(currentPos, remainingLength, nextHit);
                if (remainingLength <= 0) break;

                AddPoint(points, nextHit);
                currentPos = nextHit + 0.01f* currentDir;
                currentDir = refracted;
                i=_surfaces.Count;
            }
        }

        AddPoint(points, currentPos + currentDir * remainingLength);
        return points;
    }

    private static float CalculateRamainingLength(Vector3 currentPos, float remainingLength, Vector3 nextHit)
    {
        float segmentLength = Vector3.Distance(currentPos, nextHit);
        remainingLength -= segmentLength;
        return remainingLength;
    }

    private void AddPoint(List<Vector3> points, Vector3 point)
    {
        points.Add(point);
        DebugDrawer.AddPoint(point, Color.green, 0.2f);
    }

}
