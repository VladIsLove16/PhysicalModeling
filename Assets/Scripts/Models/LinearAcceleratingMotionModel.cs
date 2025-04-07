using UnityEngine;

[CreateAssetMenu(fileName = "LinearAcceleratingMotionModel", menuName = "MotionModels/LinearAccelerating")]
public class LinearAcceleratingMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 acceleration = (Vector3)parameters[ParamName.acceleration].Value;
        Vector3 speed = (Vector3)parameters[ParamName.velocity].Value;
        Vector3 newPosition = (Vector3)parameters[ParamName.position].Value + speed * deltaTime;

        parameters[ParamName.time].SetValueAndForceNotify((float)parameters[ParamName.time].Value + deltaTime);
        parameters[ParamName.position].SetValueAndForceNotify((Vector3)parameters[ParamName.position].Value + speed * deltaTime + acceleration*deltaTime*deltaTime/2);
        parameters[ParamName.pathTraveled].SetValueAndForceNotify((float)parameters[ParamName.pathTraveled].Value + speed.magnitude * deltaTime + acceleration.magnitude * deltaTime * deltaTime / 2);
        parameters[ParamName.distance].SetValueAndForceNotify(newPosition.magnitude);
        parameters[ParamName.velocity].SetValueAndForceNotify(speed+acceleration*deltaTime);
        Debug.Log("speed" + speed);
        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 speed = (Vector3)parameters[ParamName.velocity].Value;
        Vector3 acceleration = (Vector3)parameters[ParamName.acceleration].Value;
        Vector3 deltaPosition = speed * time + acceleration * time * time / 2;
        Vector3 oldPosition = (Vector3)parameters[ParamName.position].Value;
        Vector3 newPosition = oldPosition + deltaPosition;
        parameters[ParamName.time].SetValueAndForceNotify(time);
        parameters[ParamName.position].SetValueAndForceNotify(newPosition);
        parameters[ParamName.pathTraveled].SetValueAndForceNotify(deltaPosition.magnitude);
        parameters[ParamName.distance].SetValueAndForceNotify(newPosition.magnitude);
        parameters[ParamName.velocity].SetValueAndForceNotify(speed + acceleration * time);
        return newPosition;
    }

}