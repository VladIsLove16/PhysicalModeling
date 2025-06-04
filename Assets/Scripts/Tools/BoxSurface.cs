using UnityEngine;

public partial class MathematicalRayPathCalculator
{
    private struct BoxSurface : ISurface
    {
        public Bounds bounds;
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
        public BoxSurface(Bounds bounds, float refractiveIndex)
        {
            this.bounds = bounds;
            this.refractiveIndex = refractiveIndex;
        }

        public bool GetIntersection(Vector3 start, Vector3 direction, out Vector3 intersectionPoint)
        {
          return RayPhysics.RayBoxIntersect(new Ray(start, direction), bounds, out intersectionPoint);
        }

        public Vector3 GetNormal(Vector3 intersectionPoint)
        {
            return Vector3.up;
        }
    }

}
