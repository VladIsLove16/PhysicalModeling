using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RefractionLensMotionModel", menuName = "MotionModelsDropdown/RefractionLensMotionModel")]
public class RefractionLensMotionModel : MotionModel
{
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
            new TopicField(ParamName.angle,false),
            new TopicField(ParamName.radius,false),
            new TopicField(ParamName.distance,false),
            new TopicField(ParamName.position,false),
            new TopicField(ParamName.refractiveIndex,false),
            new TopicField(ParamName.unityPhycicsCalculation, false),
        };
    }
}