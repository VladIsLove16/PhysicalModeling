using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinearJerkMotionModel", menuName = "MotionModelsDropdown/LinearJerkMotionModel")]
public class LinearJerkMotionModel : LinearAcceleratingMotionModel
{
    protected override Vector3 GetAcceleration(float deltaTime)
    {
        return (Vector3)GetParam(ParamName.acceleration) + deltaTime * (Vector3)GetParam(ParamName.jerk);
    }
    
    protected override Vector3 GetDeltaPosition(float deltaTime)
    {

        Vector3 deltaPos = (Vector3)GetParam(ParamName.velocity) * deltaTime
                         + (Vector3)GetParam(ParamName.acceleration) * deltaTime * deltaTime / 2f
                         + (Vector3)GetParam(ParamName.jerk) * deltaTime * deltaTime * deltaTime / 6f;
        return deltaPos;
    }
    protected override Vector3 GetVelocity(float deltaTime)
    {

        Vector3 velocity = (Vector3)GetParam(ParamName.velocity)
                         + (Vector3)GetParam(ParamName.acceleration) * deltaTime
                         + (Vector3)GetParam(ParamName.jerk)  * deltaTime * deltaTime /2f;
        return velocity;
    }

    public override List<TopicField> GetRequiredParams()
    {
       
        var newList = new List<TopicField>();
        newList.Add(new TopicField(ParamName.jerk, FieldType.Vector3, false));
        newList.AddRange(base.GetRequiredParams());
        return newList;
    }
}
