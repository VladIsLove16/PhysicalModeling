using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HelicalAcceleratingMotionModel", menuName = "MotionModelsDropdown/HelicalAcceleratingMotionModel")]
public class HelicalAcceleratingMotionModel : HelicalMotionModel
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
                linearMotionModel = ScriptableObject.CreateInstance<LinearAcceleratingMotionModel>();
                linearMotionModel.InitializeParameters();
            }
            return linearMotionModel;
        }
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float acceleration = (float)GetParam(ParamName.acceleration);
        float velocityMagnitude = (float)GetParam(ParamName.velocityMagnitude);
        float newVelocityMagnitude = velocityMagnitude + acceleration * deltaTime;
        TrySetParam(ParamName.velocityMagnitude, newVelocityMagnitude);
        return base.UpdatePosition(deltaTime);
    }
    public override List<TopicField> GetRequiredParams()
    {
        var newList = new List<TopicField>();

        newList.Add(new TopicField(ParamName.acceleration, FieldType.Float, false));
        newList.AddRange(base.GetRequiredParams());
        return newList;
    }
    protected override Vector3 GetLinearDeltaPosition(float deltaTime, float velocity, float angle)
    {
        float step = CalculateHelicalStepFromVelocity(velocity, angle);
        float acceleration = (float)GetParam(ParamName.acceleration);

        float stepacceleration = CalculateHelicalStepFromVelocity(acceleration, angle);
        LinearMotionModel.TrySetParam(ParamName.velocity, new Vector3(0, step, 0));
        LinearMotionModel.TrySetParam(ParamName.acceleration, new Vector3(0, stepacceleration, 0));
        LinearMotionModel.UpdatePosition(deltaTime);
        Vector3 linearDeltaPosition = (Vector3)LinearMotionModel.GetParam(ParamName.deltaPosition);
        return linearDeltaPosition;
    }

    protected override Vector3 GetRotationalDeltaPosition(float deltaTime, float velocity, float angle)
    {
        float rotationFrequency = CalculateHelicalFrequencyFromVelocity(velocity, angle);
        float acceleration = (float)GetParam(ParamName.acceleration);
        float rotationFrequencyacceleration = CalculateHelicalFrequencyFromVelocity(acceleration, angle);
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, rotationFrequency);
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyAcceleration, rotationFrequencyacceleration);
        RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
        RotationalMotionModel.UpdatePosition(deltaTime);
        Vector3 rotationalDeltaPosition = (Vector3)RotationalMotionModel.GetParam(ParamName.deltaPosition);
        return rotationalDeltaPosition;
    }
}

