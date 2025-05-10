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
    protected override void InitLinearMotionModelParams()
    {
        LinearMotionModel.TrySetParam(ParamName.velocity,GetParam(ParamName.velocity));
        LinearMotionModel.TrySetParam(ParamName.acceleration,GetParam(ParamName.acceleration));
    }
    protected override void InitRotationalMotionModelParams()
    {
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, GetParam(ParamName.rotationFrequency));
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyAcceleration, GetParam(ParamName.rotationFrequencyAcceleration));
        RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
        RotationalMotionModel.TrySetParam(ParamName.velocity, GetParam(ParamName.velocity));
    }
    public override List<TopicField> GetRequiredParams()
    {
        var newList = new List<TopicField>();

        newList.Add(new TopicField(ParamName.acceleration, FieldType.Vector3, false));
        newList.Add(new TopicField(ParamName.rotationFrequencyAcceleration, FieldType.Float, false));
        newList.AddRange(base.GetRequiredParams());
        return newList;
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 pos = base.UpdatePosition(deltaTime);
        TrySetParam(ParamName.velocity,LinearMotionModel.GetParam(ParamName.velocity));
        TrySetParam(ParamName.rotationFrequency,RotationalMotionModel.GetParam(ParamName.rotationFrequency));
        return pos;
    }
}

