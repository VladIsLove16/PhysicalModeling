using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinearAcceleratingMotionModel", menuName = "MotionModelsDropdown/LinearAcceleratingMotionModel")]
public class LinearAcceleratingMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 acceleration = GetAcceleration(deltaTime);
        Vector3 deltaPos = GetDeltaPosition(deltaTime);
        Vector3 newPosition = (Vector3)GetParam(ParamName.position) + deltaPos;

        SetParam(ParamName.time,(float)GetParam(ParamName.time) + deltaTime);
        SetParam(ParamName.position,newPosition);
        SetParam(ParamName.pathTraveled,(float)GetParam(ParamName.pathTraveled) + deltaPos.magnitude);
        SetParam(ParamName.distance,(newPosition.magnitude));
        SetParam(ParamName.velocity,GetVelocity(deltaTime));
        SetParam(ParamName.acceleration,acceleration);
        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 acceleration = GetAcceleration(time);
        Vector3 velocity = (Vector3)GetParam(ParamName.velocity);
        Vector3 deltaPos = GetDeltaPosition(time);
        Vector3 newPosition = deltaPos;

        SetParam(ParamName.time, (float)GetParam(ParamName.time));
        SetParam(ParamName.position, newPosition) ;
        SetParam(ParamName.pathTraveled, (float)GetParam(ParamName.pathTraveled));
        SetParam(ParamName.distance, (newPosition.magnitude));
        SetParam(ParamName.velocity, velocity + acceleration * time);
        SetParam(ParamName.acceleration, acceleration);
        return newPosition;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.velocity, FieldType.Vector3,false),
           new TopicField(ParamName.acceleration, FieldType.Vector3,false),
           new TopicField(ParamName.time, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,false),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.distance,FieldType.Float, true),
        };
    }
    protected virtual Vector3 GetAcceleration(float deltaTime)
    {
        return (Vector3)GetParam(ParamName.acceleration);
    }
    protected virtual Vector3 GetDeltaPosition(float deltaTime)
    {
        return (Vector3)GetParam(ParamName.velocity) * deltaTime + (Vector3)GetParam(ParamName.acceleration) * deltaTime * deltaTime / 2;
    }
    protected virtual Vector3 GetVelocity(float deltaTime)
    {
        return (Vector3)GetParam(ParamName.velocity) + (Vector3)GetParam(ParamName.acceleration) * deltaTime;
    }
}
