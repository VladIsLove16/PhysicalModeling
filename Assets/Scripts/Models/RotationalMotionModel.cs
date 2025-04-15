using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RotationalMotionModel", menuName = "MotionModelsDropdown/RotationalMotionModel")]
public class RotationalMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);


        float numberOfRevolutions = (float)GetParam(ParamName.numberOfRevolutions) + rotationFrequency * deltaTime;
        float angularVelocity = 2 * Mathf.PI * rotationFrequency;
        float angleRadTraveled = (float)GetParam(ParamName.angleRadTraveled) + angularVelocity * deltaTime;
        float pathTraveled = angleRadTraveled * radius;
        float deltaPathTraveled = pathTraveled - (float)GetParam(ParamName.pathTraveled);

        Vector3 newPosition, deltaPosition;
        GetDeltaPos(radius, angleRadTraveled, out newPosition, out deltaPosition);
        float period = 2 * Mathf.PI * radius / rotationFrequency;
        float angleRad = angleRadTraveled % (Mathf.PI * 2);
        float velocityMagnitude = deltaPathTraveled / deltaTime;

        SetParam(ParamName.time, (float)GetParam(ParamName.time) + deltaTime);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.deltaPosition, deltaPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.deltaPathTraveled, deltaPathTraveled);
        SetParam(ParamName.angularVelocity, angularVelocity);
        SetParam(ParamName.period, period);
        SetParam(ParamName.angularVelocity, velocityMagnitude);
        SetParam(ParamName.angleRadTraveled, angleRadTraveled);
        SetParam(ParamName.angleRad, angleRad);
        SetParam(ParamName.numberOfRevolutions, numberOfRevolutions);

        return newPosition;
    }

    private void GetDeltaPos(float radius, float angleRadTraveled, out Vector3 newPosition, out Vector3 deltaPosition)
    {
        Vector3 localPoint = new Vector3(
            radius * Mathf.Cos(angleRadTraveled),
            radius * Mathf.Sin(angleRadTraveled),
            0
        );

        Vector3 velocity = ((Vector3)GetParam(ParamName.velocity)).normalized;
        if (velocity == Vector3.zero)
            velocity = Vector3.forward; // fallback, если скорость 0

        Quaternion rotation = Quaternion.LookRotation(velocity);

        newPosition = rotation * localPoint;

        Vector3 previousPosition = (Vector3)GetParam(ParamName.position);
        deltaPosition = newPosition - previousPosition;
    }


    public override Vector3 CalculatePosition(float time)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);

        float numberOfRevolutions = rotationFrequency * time;
        float angularVelocity = 2 * Mathf.PI * rotationFrequency;
        float angleRadTraveled = angularVelocity * time;
        float pathTraveled = angleRadTraveled * radius;
        float x = radius * Mathf.Cos(angleRadTraveled);
        float y = radius * Mathf.Sin(angleRadTraveled);
        Vector3 newPosition = new Vector3(x,y);
        float period = time / numberOfRevolutions;
        float angleRad = angleRadTraveled % (Mathf.PI*2);
        float velocityMagnitude = pathTraveled / time;

        SetParam(ParamName.time,time);
        SetParam(ParamName.position,newPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.deltaPathTraveled, pathTraveled);
        SetParam(ParamName.angularVelocity, angularVelocity);
        SetParam(ParamName.period, period);
        SetParam(ParamName.velocityMagnitude, velocityMagnitude);
        SetParam(ParamName.angleRadTraveled, angleRadTraveled);
        SetParam(ParamName.angleRad, angleRad);
        SetParam(ParamName.numberOfRevolutions, numberOfRevolutions);

        return newPosition;
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.radius, FieldType.Float,false),
           new TopicField(ParamName.rotationFrequency, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,true),
           new TopicField(ParamName.deltaPosition, FieldType.Vector3,true),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.deltaPathTraveled ,FieldType.Float,true),
           new TopicField(ParamName.angularVelocity,FieldType.Float, true),
           new TopicField(ParamName.velocity,FieldType.Vector3, true),
           new TopicField(ParamName.velocityMagnitude,FieldType.Float, true),
           new TopicField(ParamName.time,FieldType.Float, true),
           new TopicField(ParamName.angleRad, FieldType.Float, true),
           new TopicField(ParamName.period, FieldType.Float, true),
           new TopicField(ParamName.angleRadTraveled, FieldType.Float, true),
           new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true)
        };
    }
}
