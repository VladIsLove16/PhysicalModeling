using UnityEngine;
using System.Collections.Generic;

public interface IRayPathCalculator
{
    static  float AIRREFRACTION { get { return 1f; } }
    List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces);
}
