using System.Collections.Generic;
using UniRx;
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

    public override void InitializeParameters(bool isForce = false)
    {
        base.InitializeParameters(isForce);
        actions[ParamName.acceleration] = OnAccelerationChanged;
        actions[ParamName.rotationFrequencyAcceleration] = OnRotationFrequencyAccelerationChanged;
        foreach (var field in topicFields)
        {
            if (actions.ContainsKey(field.ParamName))
                field.Property.Subscribe(actions[field.ParamName]);
        }
    }

    private void OnRotationFrequencyAccelerationChanged( object value)
    {
        rotationalMotionModel.TrySetParam(ParamName.rotationFrequencyAcceleration, value);
    }

    private void OnAccelerationChanged(object value)
    {
        LinearMotionModel.TrySetParam(ParamName.acceleration, value);
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

