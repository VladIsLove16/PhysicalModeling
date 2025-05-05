using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitMotionModel", menuName = "MotionModelsDropdown/HitMotionModel")]
public class HitMotionModel : MotionModel
{
    private Rigidbody movingObject;
    private Rigidbody hittedObject;
    public void Init(Rigidbody movingObject, Rigidbody hittedObject)
    {
        this.movingObject = movingObject;
        this.hittedObject = hittedObject;
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        SetParam(ParamName.velocity, movingObject.linearVelocity);
        SetParam(ParamName.velocity2, hittedObject.linearVelocity);
        SetParam(ParamName.position, movingObject.position);
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
           new TopicField(ParamName.position, FieldType.Vector3,false),
           new TopicField(ParamName.velocity, FieldType.Vector3,false),
           new TopicField(ParamName.velocity2, FieldType.Vector3,false),
           new TopicField(ParamName.mass, FieldType.Float,false),
           new TopicField(ParamName.mass2, FieldType.Float,false),
        };
    }
}
