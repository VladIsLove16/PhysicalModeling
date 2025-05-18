using UnityEngine;
using System.Collections.Generic;
using UnityEngine.LowLevelPhysics;

public partial class RayTracer : IRayPathCalculator
{
    private struct LensSurface : ISurface
    {
        public float radius;
        public Vector3 center;
        public float refractiveIndex;
        public float RefractiveIndex { 
            get
            {
                return refractiveIndex;
            }
            set
            {
                refractiveIndex = value;
            }
        }
        public LensSurface(Vector3 center, float radius, float refractiveIndex)
        {
            this.center = center;
            this.radius = radius;
            this.refractiveIndex = refractiveIndex;
        }
        public bool GetIntersection(Vector3 start, Vector3 direction, out Vector3 intersectionPoint)
        {
          return RayPhysics.RaySphereIntersect(start, direction, center, radius, out intersectionPoint);
        }
        public Vector3 GetNormal(Vector3 intersectionPoint)
        {
            return (intersectionPoint-center).normalized;
        }
    } 
    private List<ISurface> _surfaces = new List<ISurface>();
    public RayTracer()
    {
    }

    public RayTracer(float radius, float distance, float lensRefractiveIndex,Vector3 lensPosition)
    {
        float thickness = radius * 2 - distance;
        var  centerFront = new Vector3(0, radius - thickness / 2, 0) + lensPosition;
        var  centerBack = new Vector3(0, -radius + thickness / 2f, 0) + lensPosition;
        LensSurface lensSurface1 = new LensSurface(centerFront,radius, lensRefractiveIndex);
        LensSurface lensSurface2 = new LensSurface(centerBack, radius, 1f);
        _surfaces.Add(lensSurface1);
        _surfaces.Add(lensSurface2);
    }
    public List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces, float initialRefractiveIndex = 1.0f)
    {
        List<Vector3> points = new List<Vector3> { start };

        Vector3 currentPos = start;
        Vector3 currentDir = direction.normalized;
        float remainingLength = maxLength;

        for (int i = 0; i < _surfaces.Count; i++)
        {
            Vector3 nextHit, normal;
            float n1, n2;
            if (!_surfaces[i].GetIntersection(currentPos, currentDir, out nextHit))
            {
                Debug.LogAssertion("GetIntersection not foubd on surface " + i);
                continue;
            }
            normal = _surfaces[i].GetNormal(nextHit);
            n1 = i == 0 ? initialRefractiveIndex : _surfaces[i - 1].RefractiveIndex;
            n2 = _surfaces[i].RefractiveIndex;
            // Автоматическая ориентация нормали
            if (Vector3.Dot(currentDir, normal) > 0)
                normal = -normal;

            DebugDrawer.AddRay(new Ray(nextHit, -normal), Color.red);   
            DebugDrawer.AddRay(new Ray(currentPos+0.01f *Vector3.one, currentDir), Color.blue);
            Vector3 refracted;
            if (!RayPhysics.ComputeSneliusRefractedDirection(currentDir, normal, n1 , n2, out refracted))
            {

                Debug.LogAssertion("Full refraction on surface " + i);
                break; // Полное внутреннее отражение
            }

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
