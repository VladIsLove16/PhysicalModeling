using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitMotionModel", menuName = "MotionModelsDropdown/HitMotionModel")]
public class HitMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        return Vector3.zero;
    }

    public override Vector3 CalculatePosition(float time)
    {
        return Vector3.zero;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.velocity, FieldType.Vector3,false),
           new TopicField(ParamName.time, FieldType.Float,false),
           new TopicField(ParamName.mass, FieldType.Vector3,false),
           new TopicField(ParamName.mass2, FieldType.Vector3,false),
        };
    }
}
