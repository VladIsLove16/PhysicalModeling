using UnityEngine;

public interface IMovementStrategy
{
    Vector3 CalculatePosition(float deltaTime);
   void ResetParams();
   void ResetParam(ParamName parametrName);
}
