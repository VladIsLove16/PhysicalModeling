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
        Vector3 velocity = (Vector3)topicFields[ParamName.velocity].Value;
        Vector3 newPosition = (Vector3)topicFields[ParamName.position].Value + velocity * deltaTime;

        SetParam(ParamName.time,(float)topicFields[ParamName.time].Value + deltaTime);
        SetParam(ParamName.position,(Vector3)topicFields[ParamName.position].Value + velocity * deltaTime);
        SetParam(ParamName.pathTraveled,(float)topicFields[ParamName.pathTraveled].Value + velocity.magnitude * deltaTime);
        SetParam(ParamName.distance,newPosition.magnitude);
        Debug.Log(" velocity" + velocity);
        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 velocity = (Vector3)topicFields[ParamName.velocity].Value;
        Vector3 newPosition = velocity * time;

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.pathTraveled, newPosition.magnitude);
        SetParam(ParamName.distance, newPosition.magnitude);
        return newPosition;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.velocity, FieldType.Vector3,false),
           new TopicField(ParamName.time, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,false),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.distance,FieldType.Float, true),
        };
    }
}
