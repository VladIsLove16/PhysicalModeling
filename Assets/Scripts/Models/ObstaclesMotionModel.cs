using System;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclesMotionModel", menuName = "MotionModelsDropdown/ObstaclesMotionModel")]
public class ObstaclesMotionModel : MotionModel
{
    protected override Dictionary<ParamName, object> DefaultValues => defaultValues;
    protected override Dictionary<ParamName, object> MaxValues => maxValues;
    protected override Dictionary<ParamName, object> MinValues => minValues;

    private static readonly Dictionary<ParamName, object> defaultValues = new()
    {
        { ParamName.seed, 3 },
        { ParamName.angleDeg, 0f },
    };

    private static readonly Dictionary<ParamName, object> maxValues = new()
    {
        { ParamName.seed, 1_000_000 },
        { ParamName.angleDeg, 359f },
    };

    private static readonly Dictionary<ParamName, object> minValues = new()
    {
        { ParamName.seed, 0 },
        { ParamName.angleDeg, 0f },
    };

    private GameObject movingObject;
    private Rigidbody movingObjectrb;
    private PointA pointAReference;
    private float initialTimeScale = 1.0f;
    private float timeScaleBeforeSimulation = 1.0f;

    public void Init(GameObject movingObject, PointA pointA)
    {
        if (movingObject == null)
        {
            Debug.LogError("ObstaclesMotionModel.Init called with null movingObject");
            return;
        }

        if (pointA == null)
        {
            Debug.LogError("ObstaclesMotionModel.Init called with null PointA");
            return;
        }

        this.movingObject = movingObject;
        pointAReference = pointA;
        pointAReference.pointReached = () => TrySetParam(ParamName.pointAReached, true);

        if (!movingObject.TryGetComponent(out movingObjectrb))
            movingObjectrb = movingObject.AddComponent<Rigidbody>();

        ConfigureRigidbody();
        ResetMovingObjectTransform();
        initialTimeScale = Time.timeScale;
        timeScaleBeforeSimulation = initialTimeScale;
        Time.timeScale = 0.0f;
        Debug.Log("ObstaclesMotionModel.Init");
    }

    public override void OnDisabled()
    {
        if (pointAReference != null)
        {
            pointAReference.pointReached = null;
            pointAReference = null;
        }

        if (movingObjectrb != null)
        {
            UnityEngine.Object.Destroy(movingObjectrb);
            movingObjectrb = null;
        }

        movingObject = null;
        Time.timeScale = initialTimeScale;
    }

    public override void OnSimulationStateChanged(SimulationState value)
    {
        switch (value)
        {
            case SimulationState.paused:
                Time.timeScale = 0.0f;
                break;
            case SimulationState.stoped:
                Time.timeScale = timeScaleBeforeSimulation;
                ResetMovingObjectTransform();
                break;
            case SimulationState.continued:
                Time.timeScale = 1.0f;
                break;
            case SimulationState.started:
                timeScaleBeforeSimulation = Time.timeScale;
                Time.timeScale = 1.0f;
                SetupVelocity();
                SetupMass();
                break;
        }
    }

    private void SetupMass()
    {
        if (movingObjectrb != null && GetParam(ParamName.mass) is float startMass)
            movingObjectrb.mass = startMass;
    }

    public override Vector3 CalculatePosition(float time)
    {
        return movingObjectrb != null ? movingObjectrb.position : Vector3.zero;
    }

    public override bool TrySetParam(ParamName paramName, object value, bool notify = true)
    {
        bool success = base.TrySetParam(paramName, value, notify);
        if (!success)
            return false;

        if (movingObjectrb == null)
            return success;

        switch (paramName)
        {
            case ParamName.velocityMagnitude:
                SetupVelocity();
                break;
            case ParamName.angleDeg:
                SetupVelocity();
                break;
            case ParamName.mass:
                if (GetParam(ParamName.mass) is float massValue)
                    movingObjectrb.mass = massValue;
                break;
        }

        return success;
    }

    private void SetupVelocity()
    {
        if (movingObject == null || movingObjectrb == null)
        {
            Debug.LogWarning("ObstaclesMotionModel.SetupVelocity called before initialization");
            return;
        }

        float velocityMagnitude =(float) GetParam(ParamName.velocityMagnitude);

        float angle = (float)GetParam(ParamName.angleDeg);

        float radAngle = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radAngle);
        float z = Mathf.Sin(radAngle);

        movingObject.transform.eulerAngles = new Vector3(x * 90f, 0f, z * 90f);
        Vector3 desiredVelocity = new Vector3(x * velocityMagnitude, 0f, z * velocityMagnitude);

        movingObjectrb.linearVelocity = desiredVelocity;
    }

    private void ConfigureRigidbody()
    {
        if (movingObjectrb == null)
            return;

        movingObjectrb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        movingObjectrb.linearVelocity = Vector3.zero;
    }

    private void ResetMovingObjectTransform()
    {
        if (movingObject == null)
            return;

        movingObject.transform.position = Vector3.zero;
        movingObject.transform.rotation = Quaternion.identity;

        if (movingObjectrb == null)
            return;

        movingObjectrb.linearVelocity = Vector3.zero;
        movingObjectrb.angularVelocity = Vector3.zero;
        movingObjectrb.position = Vector3.zero;
        movingObjectrb.rotation = Quaternion.identity;
    }

    public override void ResetParam(ParamName parametrName)
    {
        if (parametrName == ParamName.seed || parametrName == ParamName.respawnObstacles)
            return;

        if (parametrName == ParamName.position || parametrName == ParamName.pointAReached || parametrName == ParamName.velocityMagnitude)
            base.ResetParam(parametrName);
    }

    public override void ResetParams()
    {
        ResetMovingObjectTransform();
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
           new TopicField(ParamName.position, FieldType.Vector3,false),
           new TopicField(ParamName.velocityMagnitude, FieldType.Float,false),
           new TopicField(ParamName.angleDeg, FieldType.Float,false),
           new TopicField(ParamName.mass, FieldType.Float,false),
           new TopicField(ParamName.obstaclesMass, FieldType.Float,false),
           new TopicField(ParamName.pointAReached, FieldType.Bool,true),
           new TopicField(ParamName.seed, FieldType.Int,false),
           new TopicField(ParamName.respawnObstacles, FieldType.Bool,false),
        };
    }

    public override Vector3 UpdatePosition(float deltaTime)
    {
        return Vector3.zero;
    }
}