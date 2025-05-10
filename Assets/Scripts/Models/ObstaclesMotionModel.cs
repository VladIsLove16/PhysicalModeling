using System;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclesMotionModel", menuName = "MotionModelsDropdown/ObstaclesMotionModel")]
public class ObstaclesMotionModel : MotionModel
{
    private GameObject movingObject;
    private Rigidbody movingObjectrb;
    public void Init(GameObject movingObject, PointA pointA)
    {
        this.movingObject = movingObject;
        pointA.pointReached = () => TrySetParam(ParamName.pointAReached, true);
        movingObjectrb = movingObject.AddComponent<Rigidbody>();
        movingObjectrb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Time.timeScale = 0.0f;
        Debug.Log("ObstaclesMotionModel.Init(");
    }
    public override void OnDisabled()
    {
        Destroy(movingObjectrb);
        Time.timeScale = 1.0f;
    }
    public override void OnSimulationStateChanged(SimulationState value)
    {
        switch (value)
        {
            case SimulationState.paused:
                {
                    Time.timeScale = 0.0f;
                    break;
                }
            case SimulationState.stoped:
                {
                    Time.timeScale = 0.0f;
                    break;
                }
            case SimulationState.continued:
                {
                    Time.timeScale = 1.0f;
                    break;
                }
            case SimulationState.started:
                {
                    Time.timeScale = 1.0f;
                    SetupVelocity();
                    movingObjectrb.mass = (float)(GetParam(ParamName.mass));
                    break;
                }
        }
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        TrySetParam(ParamName.velocityMagnitude, movingObjectrb.linearVelocity.magnitude);
        TrySetParam(ParamName.position, movingObjectrb.position);
        return movingObjectrb.position;
    }

    public override Vector3 CalculatePosition(float time)
    {
        //SetupVelocity();
        //movingObjectrb.constraints = RigidbodyConstraints.FreezePosition;
        //movingObject.transform.position = (Vector3)TryGetParam(ParamName.position);
        return movingObjectrb.position;
    }

    private void SetupVelocity()
    {
        float VelocityMagnitude = (float)GetParam(ParamName.velocityMagnitude);
        float angle = (float)GetParam(ParamName.angle);
        float radAngle = angle * 2f * (float)Math.PI / 360;
        float x = (float)Math.Cos(radAngle);
        float y = (float)Math.Sin(radAngle);
        movingObject.transform.eulerAngles = new Vector3(x * 90, 0, y * 90);
        movingObjectrb.linearVelocity = new Vector3(x*VelocityMagnitude, 0, y*VelocityMagnitude);
    }

    public override void ResetParam(ParamName parametrName)
    {
        if(parametrName == ParamName.seed)
            return;
        if(parametrName == ParamName.respawnObstacles)
            return;
        if (parametrName == ParamName.position)
            base.ResetParam(parametrName);
        if (parametrName == ParamName.pointAReached)
            base.ResetParam(parametrName);
        if (parametrName == ParamName.velocityMagnitude)
            base.ResetParam(parametrName);
        //base.ResetParam(parametrName);
    }
    public override void ResetParams()
    {
        movingObject.transform.position = Vector3.zero;
        //base.ResetParams();
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.position, FieldType.Vector3,false),
           new TopicField(ParamName.velocityMagnitude, FieldType.Float,false),
           new TopicField(ParamName.angle, FieldType.Float,false),
           new TopicField(ParamName.mass, FieldType.Float,false),
           new TopicField(ParamName.obstaclesMass, FieldType.Float,false),
           new TopicField(ParamName.pointAReached, FieldType.Bool,true),
           new TopicField(ParamName.seed, FieldType.Int,false),
           new TopicField(ParamName.respawnObstacles, FieldType.Bool,false),
        };
    }
}
