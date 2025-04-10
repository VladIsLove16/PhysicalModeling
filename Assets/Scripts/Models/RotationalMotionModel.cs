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
        float x = radius * Mathf.Cos(angleRadTraveled);
        float y = radius * Mathf.Sin(angleRadTraveled);
        Vector3 newPosition = new Vector3(x, y);
        float period = 2 * Mathf.PI * radius / rotationFrequency;
        float angleRad = angleRadTraveled % Mathf.PI;

        SetParam(ParamName.time, (float)GetParam (ParamName.time) + deltaTime);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.angularVelocity, angularVelocity);
        SetParam(ParamName.radius, radius);
        SetParam(ParamName.period, period);
        SetParam(ParamName.velocity, angularVelocity);
        SetParam(ParamName.angleRadTraveled, angleRadTraveled);
        SetParam(ParamName.angleRad, angleRad);

        return newPosition;
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
        float angleRad = angleRadTraveled % Mathf.PI;

        SetParam(ParamName.time,time);
        SetParam(ParamName.position,newPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.angularVelocity, angularVelocity);
        SetParam(ParamName.radius, radius);
        SetParam(ParamName.period, period);
        SetParam(ParamName.velocity, angularVelocity);
        SetParam(ParamName.angleRadTraveled, angleRadTraveled);
        SetParam(ParamName.angleRad, angleRad);

        return newPosition;
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.radius, FieldType.Float,false),
           new TopicField(ParamName.rotationFrequency, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,true),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.angularVelocity,FieldType.Float, true),
           new TopicField(ParamName.velocity,FieldType.Vector3, true),
           new TopicField(ParamName.time,FieldType.Float, true),
           new TopicField(ParamName.angleRad, FieldType.Float, true),
           new TopicField(ParamName.period, FieldType.Float, true),
           new TopicField(ParamName.angleRadTraveled, FieldType.Float, true)
        };
    }
    public override void InitializeParameters()
    {
        parameters.Clear();
        foreach (var field in GetRequiredParams())
        {
            parameters[field.ParamName] = CreateReactiveProperty(field.Type);
        }
    }
}