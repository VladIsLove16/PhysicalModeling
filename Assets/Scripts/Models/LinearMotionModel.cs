using NUnit.Framework;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[CreateAssetMenu(fileName = "LinearMotionModel", menuName = "MotionModelsDropdown/Linear")]
public class LinearMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 speed = (Vector3)parameters[ParamName.velocity].Value;
        Vector3 newPosition = (Vector3)parameters[ParamName.position].Value + speed * deltaTime;

        parameters[ParamName.time].SetValueAndForceNotify((float)parameters[ParamName.time].Value + deltaTime);
        parameters[ParamName.position].SetValueAndForceNotify((Vector3)parameters[ParamName.position].Value + speed * deltaTime);
        parameters[ParamName.pathTraveled].SetValueAndForceNotify((float)parameters[ParamName.pathTraveled].Value + speed.magnitude * deltaTime);
        parameters[ParamName.distance].SetValueAndForceNotify(newPosition.magnitude);
        Debug.Log("speed" + speed);
        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 speed = (Vector3)parameters[ParamName.velocity].Value;
        Vector3 deltaPosition = speed * time;
        Vector3 oldPosition = (Vector3)parameters[ParamName.position].Value;
        Vector3 newPosition = oldPosition + deltaPosition;
        parameters[ParamName.time].SetValueAndForceNotify(time);
        parameters[ParamName.position].SetValueAndForceNotify(newPosition);
        parameters[ParamName.pathTraveled].SetValueAndForceNotify(deltaPosition.magnitude);
        parameters[ParamName.distance].SetValueAndForceNotify(newPosition.magnitude);
        return newPosition;
    }

    public override List<TopicField> GetRequiredParams()
    {
        throw new NotImplementedException();
    }
}
