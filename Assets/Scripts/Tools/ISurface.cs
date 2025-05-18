using UnityEngine;

public partial class LensRayTracer
{
    public interface ISurface
    {
        public bool GetIntersection(Vector3 start, Vector3 direction, out Vector3 intersectionPoint);
        Vector3 GetNormal(Vector3 nextHit);
        public float RefractiveIndex {  get; set; }
    }

}
