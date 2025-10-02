using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HelicalJerkMotionModel", menuName = "MotionModelsDropdown/HelicalJerkMotionModel")]
public class HelicalJerkMotionModel : HelicalMotionModel
{
    protected override MotionModel RotationalMotionModel
    {
        get
        {
            if (rotationalMotionModel == null)
            {
                rotationalMotionModel = ScriptableObject.CreateInstance<RotationalAcceleratingMotionModel>();
                rotationalMotionModel.InitializeParameters();
            }
            return rotationalMotionModel;
        }
    }
    protected override MotionModel LinearMotionModel
    {
        get
        {
            if (linearMotionModel == null)
            {
                linearMotionModel = ScriptableObject.CreateInstance<LinearJerkMotionModel>();
                linearMotionModel.InitializeParameters();
            }
            return linearMotionModel;
        }
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float acceleration = (float)GetParam(ParamName.acceleration);
        float jerk = (float)GetParam(ParamName.jerk);
        float velocityMagnitude = (float)GetParam(ParamName.velocityMagnitude);
        float newacceleration = acceleration + jerk * deltaTime;
        float newVelocityMagnitude = velocityMagnitude + acceleration * deltaTime + jerk * 0.5f * deltaTime * deltaTime;
        TrySetParam(ParamName.velocityMagnitude, newVelocityMagnitude);
        TrySetParam(ParamName.acceleration, newacceleration);
        return base.UpdatePosition(deltaTime);
    }
    public override List<TopicField> GetRequiredParams()
    {
        List<TopicField> newList = new();
        newList.Add(new TopicField(ParamName.acceleration, FieldType.Float, false));
        newList.Add(new TopicField(ParamName.jerk, FieldType.Float, false));
        newList.AddRange(base.GetRequiredParams());
        return newList;
    }

    protected override Vector3 GetLinearDeltaPosition(float deltaTime, float velocity, float angle)
    {
        float step = CalculateHelicalStepFromVelocity(velocity, angle);
        float acceleration = (float)GetParam(ParamName.acceleration);
        float jerk = (float)GetParam(ParamName.jerk);
        float stepacceleration = CalculateHelicalStepFromVelocity(acceleration, angle);
        float stepjerk = CalculateHelicalStepFromVelocity(jerk, angle);
        LinearMotionModel.TrySetParam(ParamName.velocity, new Vector3(0, step, 0));
        LinearMotionModel.TrySetParam(ParamName.acceleration, new Vector3(0, stepacceleration, 0));
        LinearMotionModel.TrySetParam(ParamName.jerk, new Vector3(0, stepjerk, 0));
        LinearMotionModel.UpdatePosition(deltaTime);
        Vector3 linearDeltaPosition = (Vector3)LinearMotionModel.GetParam(ParamName.deltaPosition);
        return linearDeltaPosition;
    }

    protected override Vector3 GetRotationalDeltaPosition(float deltaTime, float velocity, float angle)
    {
        float rotationFrequency = CalculateHelicalFrequencyFromVelocity(velocity, angle);
        float acceleration = (float)GetParam(ParamName.acceleration);
        float jerk = (float)GetParam(ParamName.jerk); 
        float rotationFrequencyAcceleration = CalculateHelicalFrequencyFromVelocity(acceleration, angle);
        float rotationFrequencyjerk = CalculateHelicalFrequencyFromVelocity(jerk, angle);
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, rotationFrequency);
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyAcceleration, rotationFrequencyAcceleration);
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyJerk, rotationFrequencyjerk);
        RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
        RotationalMotionModel.UpdatePosition(deltaTime);
        Vector3 rotationalDeltaPosition = (Vector3)RotationalMotionModel.GetParam(ParamName.deltaPosition);
        return rotationalDeltaPosition;
    }
}

