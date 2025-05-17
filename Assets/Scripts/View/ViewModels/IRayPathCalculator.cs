using UnityEngine;
using System.Collections.Generic;

public interface IRayPathCalculator
{
    List<Vector3> CalculateRayPath(Vector3 start, Vector3 direction, float maxLength, int maxBounces, float initialRefractiveIndex = 1.0f);
}
