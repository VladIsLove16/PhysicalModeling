using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
    public override void InitializeParameters(bool isForce = false)
    {
        base.InitializeParameters(isForce);
        actions[ParamName.rotationFrequencyJerk] = OnRotationFrequencyJerkChanged;
        actions[ParamName.jerk] = OnJerkChanged;
        foreach (var field in topicFields)
        {
            if (actions.ContainsKey(field.ParamName))
                field.Property.Subscribe(actions[field.ParamName]);
        }
    }

    private void OnJerkChanged(object value)
    {
        linearMotionModel?.TrySetParam(ParamName.jerk, value);
    }

    private void OnRotationFrequencyJerkChanged(object value)
    {
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequencyJerk, value);

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

