using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterMotionModel", menuName = "MotionModelsDropdown/WaterMotionModel")]
public class WaterMotionModel : MotionModel
{
    private Dictionary<ParamName, object> defaultValues = new Dictionary<ParamName, object>()
        {
            { ParamName.density, 900f },
            { ParamName.volume, 8f },
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

            { ParamName.density, 0f },
            { ParamName.volume, 0f },
    };
        }
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
        return new List<TopicField>()
        {
            new TopicField(ParamName.density, FieldType.Float,false),
            new TopicField(ParamName.volume,false)
        };
    }
   
}

