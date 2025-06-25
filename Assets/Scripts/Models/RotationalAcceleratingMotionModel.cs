using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "RotationalAcceleratingMotionModel", menuName = "MotionModelsDropdown/RotationalAcceleratingMotionModel")]
public class RotationalAcceleratingMotionModel : RotationalMotionModel
{
    protected override MotionDeltaResult ComputeMotion(float deltaTime)
    {
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        float rotationFrequencyAcceleration = (float)GetParam(ParamName.rotationFrequencyAcceleration);
        float rotationFrequencyJerk = (float)GetParam(ParamName.rotationFrequencyJerk);
        float radius = (float)GetParam(ParamName.radius);

        float deltaRotationFrequency = rotationFrequencyAcceleration * deltaTime +
                                        rotationFrequencyJerk * deltaTime * deltaTime / 2f;
        float deltaRotationFrequencyAcceleration = rotationFrequencyJerk * deltaTime;

        float deltaAngleRad = (rotationFrequency * deltaTime +
                                rotationFrequencyAcceleration * deltaTime * deltaTime / 2f +
                                rotationFrequencyJerk * deltaTime * deltaTime * deltaTime / 6f) * 2 * Mathf.PI;

        float deltaPath = deltaAngleRad * radius;
        float deltaNumberOfRevolutions = deltaAngleRad / (2 * Mathf.PI);
        float deltaAngularVelocity = deltaAngleRad / deltaTime;

        return new MotionDeltaResult
        {
            AngularVelocity = deltaAngularVelocity,
            AngleRad = deltaAngleRad,
            Path = deltaPath,
            RotationFrequency = rotationFrequency + deltaRotationFrequency,
            RotationFrequencyAcceleration = rotationFrequencyAcceleration + deltaRotationFrequencyAcceleration,
            NumberOfRevolutions = deltaNumberOfRevolutions
        };
    }

    public override List<TopicField> GetRequiredParams()
    {
        List<TopicField> fields = new();
        fields.Add(new TopicField(ParamName.rotationFrequencyJerk, FieldType.Float, false));
        fields.Add(new TopicField(ParamName.rotationFrequencyAcceleration, FieldType.Float, false));
        fields.AddRange(base.GetRequiredParams());
        return fields;
    }
}