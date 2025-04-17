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
        Vector3 velocity = (Vector3)GetParam(ParamName.velocity);
        Vector3 postion = (Vector3)GetParam(ParamName.position);

        Vector3 deltaPosition = velocity * deltaTime;
        Vector3 newPosition = postion + deltaPosition;

        SetParam(ParamName.time, (float)GetParam(ParamName.time) + deltaTime);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.pathTraveled, (float)GetParam(ParamName.pathTraveled) + deltaPosition.magnitude);
        SetParam(ParamName.distance,newPosition.magnitude);
        SetParam(ParamName.deltaPosition, deltaPosition);
        SetParam(ParamName.velocityMagnitude, velocity.magnitude);
        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 velocity = (Vector3)GetParam(ParamName.velocity);
        Vector3 deltaPosition = velocity * time;
        Vector3 newPosition = deltaPosition;

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.pathTraveled, newPosition.magnitude);
        SetParam(ParamName.distance, newPosition.magnitude);
        SetParam(ParamName.deltaPosition, deltaPosition);
        return newPosition;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.velocity, FieldType.Vector3,false),
           new TopicField(ParamName.time, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,false),
           new TopicField(ParamName.velocityMagnitude, FieldType.Float,true),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.distance,FieldType.Float, true),
           new TopicField(ParamName.deltaPosition,FieldType.Vector3, true),
        };
    }
}
