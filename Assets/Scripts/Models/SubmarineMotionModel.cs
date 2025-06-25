using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "SubmarineMotionModel", menuName = "MotionModelsDropdown/SubmarineMotionModel")]
public class SubmarineMotionModel : MotionModel
{
    private Dictionary<ParamName, object> defaultValues = new Dictionary<ParamName, object>()
    {
        { ParamName.velocityMagnitude,1f },
        { ParamName.volume,1f },
        { ParamName.density,500f },
        { ParamName.startTime1,1f },
        { ParamName.density1,1400f },
        { ParamName.startTime2,3f },
        { ParamName.density2,600f },
        { ParamName.startTime3,5f },
        { ParamName.density3,1500f },
    };
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
           
            return defaultValues;
        }
    }
    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
            {

            };
        }
    }
    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
        {

    };
        }
    }
    public override void InitializeParameters(bool isForce = false)
    {
        base.InitializeParameters(isForce);
    }

    private void OnTimeChanged(object value, Submarine.SubmarineInfo info)
    {
        Debug.Log(" OnTimeChanged " + value + "  with time " + info.time);
        info.SetTime((float)value);
    }
    private void OnDensityChanged(object value, Submarine.SubmarineInfo info)
    {
        Debug.Log(" OnDensityChanged " + value  + "  with time " + info.time);
        info.SetDensity((float)value);
    }

    public override Vector3 CalculatePosition(float Time)
    {
        return Vector3.zero;
    }

    public override Vector3 UpdatePosition(float deltaTime)
    {
        return Vector3.zero;
    }
    public override List<TopicField> GetRequiredParams()
    {
        var list = new List<TopicField>()
        {
            new TopicField(ParamName.volume,FieldType.Float,false),
            new TopicField(ParamName.velocityMagnitude,FieldType.Float,false),
            new TopicField(ParamName.density,FieldType.Float,false),
            new TopicField(ParamName.density1,FieldType.Float,false),
            new TopicField(ParamName.startTime1,FieldType.Float,false),
            new TopicField(ParamName.density2,FieldType.Float,false),
            new TopicField(ParamName.startTime2,FieldType.Float,false),
            new TopicField(ParamName.density3,FieldType.Float,false),
            new TopicField(ParamName.startTime3,FieldType.Float,false),
        };
        return list;
    }
}
