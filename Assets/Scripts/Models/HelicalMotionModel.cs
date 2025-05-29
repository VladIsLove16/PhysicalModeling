using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HelicalMotionModel", menuName = "MotionModelsDropdown/HelicalMotionModel")]
public class HelicalMotionModel : MotionModel
{
    protected MotionModel rotationalMotionModel;
    protected MotionModel linearMotionModel;
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
            return maxValues;
        }
    }
    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return minValues;
        }
    }

    private static Dictionary<ParamName, object> defaultValues = new Dictionary<ParamName, object>()
    {
        { ParamName.radius, 1f },
        { ParamName.angularVelocity, 1f },
        { ParamName.rotationFrequency, 1f },
    };
    private static Dictionary<ParamName, object> maxValues = new Dictionary<ParamName, object>()
    {
        { ParamName.radius, 5f },
    };
    private static Dictionary<ParamName, object> minValues = new Dictionary<ParamName, object>()
    {
        { ParamName.radius, 0f },
    };
    protected virtual MotionModel RotationalMotionModel
    {
        get {
            if (rotationalMotionModel == null)
            { 
                rotationalMotionModel = ScriptableObject.CreateInstance<RotationalMotionModel>();
                rotationalMotionModel.InitializeParameters();
            }
            return rotationalMotionModel; 
        }
    }
    protected virtual MotionModel LinearMotionModel
    {
        get {
            if (linearMotionModel == null)
            {
                linearMotionModel = ScriptableObject.CreateInstance<LinearMotionModel>(); 
                linearMotionModel.InitializeParameters();
            }
            return linearMotionModel; 
        }
    }

    public override Vector3 UpdatePosition(float deltaTime)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        //Vector3 velocity = (Vector3)GetParam(ParamName.velocity);
        float time = (float)GetParam(ParamName.time)+deltaTime;

        Vector3 helicalDeltaPosition, helicalPosition;
        CalculateDeltaPosition(deltaTime, out helicalDeltaPosition, out helicalPosition);

        float helicalDeltaPath, helicalPath;
        CalculateDeltaPath(out helicalDeltaPath, out helicalPath);
        float
        //h;
        //if (velocity == Vector3.zero)
        //    h = 0;
        //else
            h = CalculateHelicalStepFromVelocity();
        float newHelicalAngle = CalculateAngle(h, radius);
        TrySetParam(ParamName.helicalAngle, newHelicalAngle);
        TrySetParam(ParamName.time, time);
        TrySetParam(ParamName.position, helicalPosition);
        TrySetParam(ParamName.pathTraveled, helicalPath);
        TrySetParam(ParamName.angularVelocity, RotationalMotionModel.GetParam(ParamName.angularVelocity));
        TrySetParam(ParamName.period, RotationalMotionModel.GetParam(ParamName.period));
        //TrySetParam(ParamName.velocity, velocity);
        TrySetParam(ParamName.angleRadTraveled, RotationalMotionModel.GetParam(ParamName.angleRadTraveled));
        TrySetParam(ParamName.angleRad, RotationalMotionModel.GetParam(ParamName.angleRad));
        TrySetParam(ParamName.numberOfRevolutions, RotationalMotionModel.GetParam(ParamName.numberOfRevolutions));
        TrySetParam(ParamName.rotationFrequency, rotationFrequency);
        return helicalPosition;
    }
    protected float CalculateHelicalStepFromVelocity()
    {
        Vector3 velocity = (Vector3)GetParam(ParamName.velocity);
        float frequency = (float)GetParam(ParamName.rotationFrequency);
        if (frequency == 0f) return 0f;

        float period = 1f / frequency;

        Vector3 axis = velocity.normalized;
        float axialSpeed = Vector3.Dot(velocity, axis.normalized);

        float step = axialSpeed * period;
        return step;
    }

    protected void CalculateDeltaPosition(float deltaTime, out Vector3 helicalDeltaPosition, out Vector3 helicalPosition)
    {
        Vector3 linearDeltaPosition = GetLinearDeltaPosition(deltaTime);
        Vector3 rotationalDeltaPosition = GetRotationalDeltaPosition(deltaTime);
        helicalDeltaPosition = linearDeltaPosition + rotationalDeltaPosition;
        Vector3 pos = (Vector3)GetParam(ParamName.position);
        helicalPosition = pos + helicalDeltaPosition;
    }

    protected Vector3 GetLinearDeltaPosition(float deltaTime)
    {
        InitLinearMotionModelParams();
        LinearMotionModel.UpdatePosition(deltaTime);
        Vector3 linearDeltaPosition = (Vector3)LinearMotionModel.GetParam(ParamName.deltaPosition);
        return linearDeltaPosition;
    }

    protected Vector3 GetRotationalDeltaPosition(float deltaTime)
    {
        InitRotationalMotionModelParams();
        RotationalMotionModel.UpdatePosition(deltaTime);
        Vector3 rotationalDeltaPosition = (Vector3)RotationalMotionModel.GetParam(ParamName.deltaPosition);
        return rotationalDeltaPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        float radius = (float)GetParam(ParamName.radius);
        float rotationFrequency = (float)GetParam(ParamName.rotationFrequency);
        //Vector3 velocity = (Vector3)GetParam(ParamName.velocity);

        Vector3 helicalDeltaPosition, helicalPosition;
        CalculateDeltaPosition(time, out helicalDeltaPosition, out helicalPosition);

        float helicalDeltaPath, helicalPath;
        CalculateDeltaPath(out helicalDeltaPath, out helicalPath);
        float
        //h;
        //if (velocity == Vector3.zero)
        //    h = 0;
        //else
            h = CalculateHelicalStepFromVelocity();
        float newHelicalAngle = CalculateAngle(h, radius);
        TrySetParam(ParamName.helicalAngle, newHelicalAngle);
        TrySetParam(ParamName.time, time);
        TrySetParam(ParamName.position, helicalPosition);
        TrySetParam(ParamName.pathTraveled, helicalPath);
        TrySetParam(ParamName.angularVelocity, RotationalMotionModel.GetParam(ParamName.angularVelocity));
        TrySetParam(ParamName.period, RotationalMotionModel.GetParam(ParamName.period));
        //TrySetParam(ParamName.velocity, velocity);
        TrySetParam(ParamName.angleRadTraveled, RotationalMotionModel.GetParam(ParamName.angleRadTraveled));
        TrySetParam(ParamName.angleRad, RotationalMotionModel.GetParam(ParamName.angleRad));
        TrySetParam(ParamName.numberOfRevolutions, RotationalMotionModel.GetParam(ParamName.numberOfRevolutions));
        TrySetParam(ParamName.rotationFrequency, rotationFrequency);
        return helicalPosition;
    }
    protected void CalculateDeltaPath(out float helicalDeltaPath, out float helicalPath)
    {
        float linearDeltaPath = (float)RotationalMotionModel.GetParam(ParamName.deltaPathTraveled);
        float rotationalDeltaPath = (float)RotationalMotionModel.GetParam(ParamName.deltaPathTraveled);
        helicalDeltaPath = (float)Mathf.Sqrt(linearDeltaPath * linearDeltaPath + rotationalDeltaPath * rotationalDeltaPath);
        helicalPath = (float)GetParam(ParamName.pathTraveled) + helicalDeltaPath;
    }
    protected virtual void InitLinearMotionModelParams()
    {
        LinearMotionModel.TrySetParam(ParamName.velocity, GetParam(ParamName.velocity));
    }


    protected virtual void InitRotationalMotionModelParams()
    {
        RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, GetParam(ParamName.rotationFrequency));
        RotationalMotionModel.TrySetParam(ParamName.velocity,GetParam(ParamName.velocity));
    }
    protected float CalculateAngle(float h, float radius)
    {
        float d = radius * 2;
        float tanAngle = h / Mathf.PI / d;
        float angle = Mathf.Atan(tanAngle) * Mathf.Rad2Deg;
        return angle;
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            new TopicField(ParamName.velocity, FieldType.Vector3, false),
            new TopicField(ParamName.radius, FieldType.Float, false),
            new TopicField(ParamName.rotationFrequency, FieldType.Float, false),

            new TopicField(ParamName.helicalAngle, FieldType.Float, true),
            new TopicField(ParamName.time, FieldType.Float, true),
            new TopicField(ParamName.position, FieldType.Vector3, true),
            new TopicField(ParamName.velocityMagnitude, FieldType.Float, true),
            new TopicField(ParamName.pathTraveled, FieldType.Float, true),
            new TopicField(ParamName.angularVelocity, FieldType.Float, true),
            new TopicField(ParamName.period, FieldType.Float, true),
            new TopicField(ParamName.angleRad, FieldType.Float, true),
            new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true)
        };
    }
}

    