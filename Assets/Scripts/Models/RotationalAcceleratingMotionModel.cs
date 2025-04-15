using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "RotationalAcceleratingMotionModel", menuName = "MotionModelsDropdown/RotationalAcceleratingMotionModel")]
public class RotationalAcceleratingMotionModel : MotionModel
{
    private struct MotionInput
    {
        public float InitialRotationFrequency;
        public float RotationFrequencyAcceleration;
        public float RotationFrequencyJerk;
        public float Radius;
        public float TimeDelta;
    }
   
    private MotionDeltaResult ComputeMotion(MotionInput input)
    {
        float deltaRotationFrequency = (input.RotationFrequencyAcceleration * input.TimeDelta +
             input.RotationFrequencyJerk * input.TimeDelta * input.TimeDelta /2f) ;
        float deltaRotationFrequencyAcceleration = input.RotationFrequencyJerk * input.TimeDelta;
        float deltaAngleRad = (input.InitialRotationFrequency * input.TimeDelta +
                              input.RotationFrequencyAcceleration * input.TimeDelta * input.TimeDelta / 2f +
                              input.RotationFrequencyJerk * input.TimeDelta * input.TimeDelta * input.TimeDelta / 6f) * 2 * Mathf.PI;
        float deltaPath = deltaAngleRad * input.Radius;
        float deltaNumberOfRevolutions = deltaAngleRad / (2 * Mathf.PI);
        float deltaAngularVelocity = deltaAngleRad / input.TimeDelta;

        return new MotionDeltaResult
        {
            AngularVelocity = deltaAngularVelocity,
            AngleRad = deltaAngleRad,
            Path = deltaPath,
            RotationFrequency = deltaRotationFrequency,
            RotationFrequencyAcceleration = deltaRotationFrequencyAcceleration,
            NumberOfRevolutions = deltaNumberOfRevolutions
        };
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        return UpdatePosition(deltaTime);
    }
    private struct MotionDeltaResult
    {
        public float AngularVelocity;
        public float AngleRad;
        public float Path;
        public float RotationFrequency;
        public float RotationFrequencyAcceleration;
        public float NumberOfRevolutions;
    }

    public Vector3 UpdatePosition(float deltaTime,bool consideringExistingParams = true)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        float rotationFrequencyAcceleration = (float)GetParam(ParamName.rotationFrequencyAcceleration);
        float rotationFrequencyJerk = (float)GetParam(ParamName.rotationFrequencyJerk);

        float angleRadTraveled = (float)GetParam(ParamName.angleRadTraveled);
        float time = (float)GetParam(ParamName.time);
        float pathTraveled = (float)GetParam(ParamName.pathTraveled);
        Vector3 prevPosition = (Vector3)GetParam(ParamName.position);
        float numberOfRevolutions = (float)GetParam(ParamName.numberOfRevolutions);

        var input = new MotionInput
        {
            InitialRotationFrequency = rotationFrequency,
            RotationFrequencyAcceleration = rotationFrequencyAcceleration,
            RotationFrequencyJerk = rotationFrequencyJerk,
            Radius = radius,
            TimeDelta = deltaTime
        };

        MotionDeltaResult deltaMotionResult = ComputeMotion(input);
        float newTime = time;
        if (consideringExistingParams)
        {
            deltaMotionResult.AngleRad += angleRadTraveled;
            deltaMotionResult.Path += pathTraveled;
            deltaMotionResult.RotationFrequency += rotationFrequency;
            deltaMotionResult.RotationFrequencyAcceleration += rotationFrequencyAcceleration;
            deltaMotionResult.NumberOfRevolutions += numberOfRevolutions;
            newTime = time + deltaTime;
        }
        Vector3 newPosition = GetPosition(deltaMotionResult.AngleRad, input.Radius);
        Vector3 deltaPosition = newPosition - (Vector3)GetParam(ParamName.position);

        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.deltaPosition, deltaPosition);
        SetParam(ParamName.time, newTime);
        SetParam(ParamName.rotationFrequency, deltaMotionResult.RotationFrequency);
        SetParam(ParamName.rotationFrequencyAcceleration, deltaMotionResult.RotationFrequencyAcceleration);
        SetParam(ParamName.pathTraveled, deltaMotionResult.Path);
        SetParam(ParamName.angleRad, deltaMotionResult.AngleRad % (2 * Mathf.PI));
        SetParam(ParamName.angleRadTraveled, deltaMotionResult.AngleRad);
        SetParam(ParamName.angularVelocity, deltaMotionResult.AngularVelocity);
        SetParam(ParamName.numberOfRevolutions, deltaMotionResult.AngleRad / (2 * Mathf.PI));
        SetParam(ParamName.velocityMagnitude, deltaMotionResult.Path/newTime);

        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        return UpdatePosition(time,false);
    }

    private Vector3 GetPosition(float angleRad,float radius)
    {
        float x = radius * Mathf.Cos(angleRad);
        float y = radius * Mathf.Sin(angleRad);
        Vector3 deltaPosition = new Vector3(x, y);
        return deltaPosition;
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.radius, FieldType.Float,false),
           new TopicField(ParamName.rotationFrequency, FieldType.Float,false),
           new TopicField(ParamName.rotationFrequencyAcceleration, FieldType.Float,false),
           new TopicField(ParamName.rotationFrequencyJerk, FieldType.Float,false),
           new TopicField(ParamName.position, FieldType.Vector3,true),
           new TopicField(ParamName.time,FieldType.Float, true),
           new TopicField(ParamName.pathTraveled,FieldType.Float,true),
           new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true),
           new TopicField(ParamName.period, FieldType.Float, true),
           new TopicField(ParamName.angleRadTraveled, FieldType.Float, true),
           new TopicField(ParamName.angularVelocity,FieldType.Float, true),
           new TopicField(ParamName.velocityMagnitude,FieldType.Float, true),
           new TopicField(ParamName.angleRad, FieldType.Float, true),
           new TopicField(ParamName.deltaPosition, FieldType.Vector3,true),
           new TopicField(ParamName.deltaPathTraveled ,FieldType.Float,true),

        };
    }
}