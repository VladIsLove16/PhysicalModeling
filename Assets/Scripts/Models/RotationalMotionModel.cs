using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RotationalMotionModel", menuName = "MotionModelsDropdown/RotationalMotionModel")]
public class RotationalMotionModel : MotionModel
{
    protected virtual MotionDeltaResult ComputeMotion(float deltaTime)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        float angularVelocity = 2 * Mathf.PI * rotationFrequency;
        float deltaAngleRad = angularVelocity * deltaTime;
        float deltaPath = deltaAngleRad * radius;
        float deltaNumberOfRevolutions = deltaAngleRad / (2 * Mathf.PI);

        return new MotionDeltaResult
        {
            AngularVelocity = angularVelocity,
            AngleRad = deltaAngleRad,
            Path = deltaPath,
            RotationFrequency = rotationFrequency,
            RotationFrequencyAcceleration = 0,
            NumberOfRevolutions = deltaNumberOfRevolutions
        };
    }

    public override Vector3 UpdatePosition(float deltaTime)
    {
        MotionDeltaResult deltaMotion = ComputeMotion(deltaTime);

        float angleRadTraveled = (float)GetParam(ParamName.angleRadTraveled) + deltaMotion.AngleRad;
        float pathTraveled = (float)GetParam(ParamName.pathTraveled) + deltaMotion.Path;
        float numberOfRevolutions = (float)GetParam(ParamName.numberOfRevolutions) + deltaMotion.NumberOfRevolutions;
        float time = (float)GetParam(ParamName.time) + deltaTime;

        Vector3 newPosition, deltaPosition;
        GetDeltaPos((float)GetParam(ParamName.radius), angleRadTraveled, out newPosition, out deltaPosition);

        float velocityMagnitude = deltaMotion.Path / deltaTime;

        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.deltaPosition, deltaPosition);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.deltaPathTraveled, deltaMotion.Path);
        SetParam(ParamName.angularVelocity, deltaMotion.AngularVelocity);
        SetParam(ParamName.rotationFrequency, deltaMotion.RotationFrequency);
        SetParam(ParamName.rotationFrequencyAcceleration, deltaMotion.RotationFrequencyAcceleration);
        SetParam(ParamName.velocityMagnitude, velocityMagnitude);
        SetParam(ParamName.angleRadTraveled, angleRadTraveled);
        SetParam(ParamName.angleRad, angleRadTraveled % (2 * Mathf.PI));
        SetParam(ParamName.numberOfRevolutions, numberOfRevolutions);
        SetParam(ParamName.time, time);

        return newPosition;
    }

    private void GetDeltaPos(float radius, float angleRadTraveled, out Vector3 newPosition, out Vector3 deltaPosition)
    {
        // 1. Локальная точка на окружности (XY-плоскость)
        Vector3 localPoint = new Vector3(
            radius * Mathf.Cos(angleRadTraveled),
            radius * Mathf.Sin(angleRadTraveled),
            0
        );

        // 2. Вектор скорости = ось вращения (нормализуем)
        Vector3 velocity = ((Vector3)GetParam(ParamName.velocity)).normalized;
        if (velocity == Vector3.zero)
            velocity = Vector3.forward; // fallback, если скорость 0

        // 3. Строим вращение, которое ориентирует локальную XY плоскость перпендикулярно velocity
        Quaternion rotation = Quaternion.LookRotation(velocity);

        // 4. Преобразуем точку в глобальное пространство
        newPosition = rotation * localPoint;

        // 5. Вычисляем дельту позиции от предыдущей
        Vector3 previousPosition = (Vector3)GetParam(ParamName.position);
        deltaPosition = newPosition - previousPosition;
    }



    public override Vector3 CalculatePosition(float time)
    {
        float savedTime = (float)GetParam(ParamName.time);
        float deltaTime = time - savedTime;
        return UpdatePosition(deltaTime);
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.radius, FieldType.Float,false),
           new TopicField(ParamName.rotationFrequency, FieldType.Float,false),
           new TopicField(ParamName.velocity,FieldType.Vector3, false),
           new TopicField(ParamName.position, FieldType.Vector3,true),
           new TopicField(ParamName.deltaPosition, FieldType.Vector3,true),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.deltaPathTraveled ,FieldType.Float,true),
           new TopicField(ParamName.angularVelocity,FieldType.Float, true),
           new TopicField(ParamName.velocityMagnitude,FieldType.Float, true),
           new TopicField(ParamName.time,FieldType.Float, true),
           new TopicField(ParamName.angleRad, FieldType.Float, true),
           new TopicField(ParamName.period, FieldType.Float, true),
           new TopicField(ParamName.angleRadTraveled, FieldType.Float, true),
           new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true)
        };
    }
}
public struct MotionDeltaResult
{
    public float AngularVelocity;
    public float AngleRad;
    public float Path;
    public float RotationFrequency;
    public float RotationFrequencyAcceleration;
    public float NumberOfRevolutions;
}
