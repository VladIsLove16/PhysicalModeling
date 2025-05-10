using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HelicalJerkMotionModel", menuName = "MotionModelsDropdown/HelicalJerkMotionModel")]
public class HelicalJerkMotionModel : HelicalAcceleratingMotionModel
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
    protected override void InitLinearMotionModelParams()
    {
        LinearMotionModel.TrySetParam(ParamName.velocity, GetParam(ParamName.velocity));
        LinearMotionModel.TrySetParam(ParamName.acceleration, GetParam(ParamName.acceleration));
        LinearMotionModel.TrySetParam(ParamName.jerk, GetParam(ParamName.jerk));
    }
    protected override void InitRotationalMotionModelParams()
    {
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, GetParam(ParamName.rotationFrequency));
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyAcceleration, GetParam(ParamName.rotationFrequencyAcceleration));
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyJerk , GetParam(ParamName.rotationFrequencyJerk));
        RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
        RotationalMotionModel.TrySetParam(ParamName.velocity, GetParam(ParamName.velocity));
    }
    public override List<TopicField> GetRequiredParams()
    {
        List<TopicField> newList = new();
        newList.Add(new TopicField(ParamName.rotationFrequencyJerk, FieldType.Float, false));
        newList.Add(new TopicField(ParamName.jerk, FieldType.Vector3, false));
        newList.AddRange(base.GetRequiredParams());
        return newList;
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 pos = base.UpdatePosition(deltaTime);
        TrySetParam(ParamName.acceleration, LinearMotionModel.GetParam(ParamName.acceleration));
        TrySetParam(ParamName.rotationFrequencyAcceleration, RotationalMotionModel.GetParam(ParamName.rotationFrequencyAcceleration));
        return pos;
    }
}

