using UnityEngine;

public interface IMovementStrategy
{
    Vector3 UpdatePosition(float deltaTime);
   void ResetParams();
   void ResetParam(ParamName parametrName);
}
