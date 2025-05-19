using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RefractionLensMotionModel", menuName = "MotionModelsDropdown/RefractionLensMotionModel")]
public class RefractionLensMotionModel : MotionModel
{

    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
            { ParamName.radius, 10f},
            { ParamName.distance, 18f },
            { ParamName.rayAngle, 15f},
            { ParamName.position, Vector3.up*2 },
            { ParamName.xPosition, 1f },
            { ParamName.refractiveIndex, 1f },
            { ParamName.unityPhycicsCalculation, false },
            };
        }
    }

    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                { ParamName.radius, 40f },
                { ParamName.distance, 78f },
                { ParamName.rayAngle, 89f },
                { ParamName.xPosition, 10f },
                { ParamName.refractiveIndex, 5f }
            };
        }
    }

    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                { ParamName.radius, 0f},
                { ParamName.distance, 0f },
                { ParamName.rayAngle, -89f},
                { ParamName.xPosition, -10f },
                { ParamName.refractiveIndex, 1f }
            };
        }
    }

    public override void OnEnabled()
    {
        base.OnEnabled();
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
       return Vector3.zero;
    }

    public override Vector3 CalculatePosition(float Time)
    {
        return Vector3.zero;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>()
        {
            new TopicField(ParamName.rayAngle,false),
            new TopicField(ParamName.radius,false),
            new TopicField(ParamName.distance,false),
            new TopicField(ParamName.position,false),
            new TopicField(ParamName.refractiveIndex,false),
            new TopicField(ParamName.unityPhycicsCalculation, false),
            new TopicField(ParamName.xPosition, false),
        };
    }
}
