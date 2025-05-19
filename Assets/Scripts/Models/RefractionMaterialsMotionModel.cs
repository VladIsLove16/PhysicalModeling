 using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[CreateAssetMenu(fileName = "RefractionMaterialsMotionModel", menuName = "MotionModelsDropdown/RefractionMaterialsMotionModel")]
public class RefractionMaterialsMotionModel : MotionModel
{
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                { ParamName.material1_Size, new Vector3(10f,1f,0.5f)},
                { ParamName.material1_Position, -Vector3.up*3 },
                { ParamName.material1_RefractiveIndex, 1.2f },
                { ParamName.material2_Size, new Vector3(10f,1f,0.5f)},
                { ParamName.material2_Position, Vector3.zero },
                { ParamName.material2_RefractiveIndex, 1.5f },
                { ParamName.material3_Size, new Vector3(10f,1f,0.5f)},
                { ParamName.material3_Position, Vector3.up*3 },
                { ParamName.material3_RefractiveIndex, 1.9f }
            };
        }
    }

    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                 { ParamName.material1_RefractiveIndex, 5f },
                { ParamName.material2_RefractiveIndex, 5f },
                { ParamName.material3_RefractiveIndex, 5f },
            };
        }
    }

    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                { ParamName.material1_RefractiveIndex, 1f },
                { ParamName.material2_RefractiveIndex, 1f },
                { ParamName.material3_RefractiveIndex, 1f },
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
            new TopicField(ParamName.material1_Size,false),
            new TopicField(ParamName.material1_Position,false),
            new TopicField(ParamName.material1_RefractiveIndex,false),
            new TopicField(ParamName.material2_Size,false),
            new TopicField(ParamName.material2_Position,false),
            new TopicField(ParamName.material2_RefractiveIndex,false),
            new TopicField(ParamName.material3_Size,false),
            new TopicField(ParamName.material3_Position,false),
            new TopicField(ParamName.material3_RefractiveIndex,false),
        };
    }
}