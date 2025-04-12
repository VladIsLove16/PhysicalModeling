using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RotationalAcceleratingMotionModel", menuName = "MotionModelsDropdown/RotationalAcceleratingMotionModel")]
public class RotationalAcceleratingMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        float acceleration = (float)GetParam(ParamName.acceleration); // угловое ускорение

        float time = (float)GetParam(ParamName.time) + deltaTime;

        float initialAngularVelocity = 2 * Mathf.PI * rotationFrequency;
        float angularVelocity = initialAngularVelocity + acceleration * deltaTime;
        float angleRadTraveled = (float)GetParam(ParamName.angleRadTraveled) +
                                 initialAngularVelocity * deltaTime + 0.5f * acceleration * deltaTime * deltaTime;
        float pathTraveled = angleRadTraveled * radius;

        float x = radius * Mathf.Cos(angleRadTraveled);
        float y = radius * Mathf.Sin(angleRadTraveled);
        Vector3 newPosition = new Vector3(x, y);

        float numberOfRevolutions = angleRadTraveled / (2 * Mathf.PI);
        float angleRad = angleRadTraveled % (Mathf.PI * 2);

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.angularVelocity, angularVelocity);
        SetParam(ParamName.angleRadTraveled, angleRadTraveled);
        SetParam(ParamName.angleRad, angleRad);
        SetParam(ParamName.numberOfRevolutions, numberOfRevolutions);

        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        float acceleration = (float)GetParam(ParamName.acceleration);

        float initialAngularVelocity = 2 * Mathf.PI * rotationFrequency;
        float angularVelocity = initialAngularVelocity + acceleration * time;

        float angleRadTraveled = initialAngularVelocity * time + 0.5f * acceleration * time * time;
        float pathTraveled = angleRadTraveled * radius;

        float x = radius * Mathf.Cos(angleRadTraveled);
        float y = radius * Mathf.Sin(angleRadTraveled);
        Vector3 newPosition = new Vector3(x, y);

        float numberOfRevolutions = angleRadTraveled / (2 * Mathf.PI);
        float angleRad = angleRadTraveled % (Mathf.PI * 2);

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.angularVelocity, angularVelocity);
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
           new TopicField(ParamName.acceleration, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,true),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.angularVelocity,FieldType.Float, true),
           new TopicField(ParamName.velocity,FieldType.Vector3, true),
           new TopicField(ParamName.time,FieldType.Float, true),
           new TopicField(ParamName.angleRad, FieldType.Float, true),
           new TopicField(ParamName.period, FieldType.Float, true),
           new TopicField(ParamName.angleRadTraveled, FieldType.Float, true),
           new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true)
        };
    }
}