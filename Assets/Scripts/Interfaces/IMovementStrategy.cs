using UnityEngine;

public interface IMovementStrategy
{
    Vector3 UpdatePosition(float deltaTime);
    Vector3 CalculatePosition(float time);
   void ResetParams();
   void ResetParam(ParamName parametrName);
}
