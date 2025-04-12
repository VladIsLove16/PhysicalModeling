using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HelicalMotionModel", menuName = "MotionModelsDropdown/HelicalMotionModel")]
public class HelicalMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float radius = (float)GetParam(ParamName.radius);
        float stepPerTurn = (float)GetParam(ParamName.step); // подъём за 1 оборот
        float time = (float)GetParam(ParamName.time) + deltaTime;
        float frequency;
        frequency = (float)GetParam(ParamName.rotationFrequency); // об/с
        float linearSpeed = (float)GetParam(ParamName.speed); 

        // Угловая скорость
        float angularVelocity = 2 * Mathf.PI * frequency; // рад/с
        float angleTraveled = angularVelocity * time;

        float x = radius * Mathf.Cos(angleTraveled);
        float y = radius * Mathf.Sin(angleTraveled);
        float z = stepPerTurn * frequency * time;

        Vector3 position = new Vector3(x, y, z);
        float pathTraveled = (float)GetParam(ParamName.speed) * time;

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, position);
        SetParam(ParamName.angleRadTraveled, angleTraveled);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.rotationFrequency, frequency);

        return position;
    }

    public override Vector3 CalculatePosition(float time)
    {
        float radius = (float)GetParam(ParamName.radius);
        float stepPerTurn = (float)GetParam(ParamName.step);
        float frequency;

        if (HasParam(ParamName.rotationFrequency))
        {
            frequency = (float)GetParam(ParamName.rotationFrequency);
        }
        else
        {
            float linearSpeed = (float)GetParam(ParamName.speed);
            float loopLength = Mathf.Sqrt((2 * Mathf.PI * radius) * (2 * Mathf.PI * radius) + stepPerTurn * stepPerTurn);
            frequency = linearSpeed / loopLength;
        }

        float angularVelocity = 2 * Mathf.PI * frequency;
        float angleTraveled = angularVelocity * time;

        float x = radius * Mathf.Cos(angleTraveled);
        float y = radius * Mathf.Sin(angleTraveled);
        float z = stepPerTurn * frequency * time;

        Vector3 position = new Vector3(x, y, z);
        float pathTraveled = (float)GetParam(ParamName.speed) * time;

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, position);
        SetParam(ParamName.angleRadTraveled, angleTraveled);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.rotationFrequency, frequency);

        return position;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            new TopicField(ParamName.radius, FieldType.Float, false),
            new TopicField(ParamName.step, FieldType.Float, false), // подъём за 1 оборот
            new TopicField(ParamName.velocity, FieldType.Float, false), // общая скорость
            new TopicField(ParamName.position, FieldType.Vector3, true),
            new TopicField(ParamName.pathTraveled, FieldType.Float, true),
            new TopicField(ParamName.angleRadTraveled, FieldType.Float, true),
            new TopicField(ParamName.time, FieldType.Float, true),
            new TopicField(ParamName.rotationFrequency, FieldType.Float, true)
        };
    }
}
