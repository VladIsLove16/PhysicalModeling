using System;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclesMotionModel", menuName = "MotionModelsDropdown/ObstaclesMotionModel")]
public class ObstaclesMotionModel : MotionModel
{
    private Rigidbody movingObject;
    public void Init(Rigidbody movingObject, PointA pointA)
    {
        this.movingObject = movingObject;
        pointA.pointReached = () => SetParam(ParamName.pointAReached, true);//явное задание на эту функцию
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        SetParam(ParamName.velocityMagnitude, movingObject.linearVelocity.magnitude);
        SetParam(ParamName.position, movingObject.position);
        movingObject.constraints = RigidbodyConstraints.None;
        return movingObject.position;
    }

    public override Vector3 CalculatePosition(float time)
    {
        float angle = (float)GetParam(ParamName.angle);
        float radAngle = angle * 2f * (float)Math.PI;
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        movingObject.transform.eulerAngles = new Vector3(x * 90, 0, y * 90);
        movingObject.linearVelocity = new Vector3(x, 0, y);
        return movingObject.position;
    }
    public override void ResetParams()
    {
        base.ResetParams();
        movingObject.constraints = RigidbodyConstraints.FreezePosition;
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.position, FieldType.Float,false),
           new TopicField(ParamName.velocityMagnitude, FieldType.Float,false),
           new TopicField(ParamName.angle, FieldType.Float,false),
           new TopicField(ParamName.mass, FieldType.Float,false),
           new TopicField(ParamName.obstaclesMass, FieldType.Float,false),
           new TopicField(ParamName.pointAReached, FieldType.Bool,false, ViewType.Toggle),
           new TopicField(ParamName.seed, FieldType.Int,false,ViewType.Slider),
        };
    }
}
