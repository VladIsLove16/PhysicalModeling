using System;
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
        { ParamName.helicalAngle, 45f },
    };
    private static Dictionary<ParamName, object> maxValues = new Dictionary<ParamName, object>()
    {
        { ParamName.radius, 5f },
        { ParamName.helicalAngle, 89f },
    };
    private static Dictionary<ParamName, object> minValues = new Dictionary<ParamName, object>()
    {
        { ParamName.radius, 0f },
        { ParamName.helicalAngle, 0f },
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
        float velocityMagnitude = (float)GetParam(ParamName.velocityMagnitude);
        float helicalAngle = (float)GetParam(ParamName.helicalAngle);
        float time = (float)GetParam(ParamName.time) + deltaTime;

        Vector3 helicalDeltaPosition, helicalPosition;
        CalculateDeltaPosition(deltaTime, velocityMagnitude, helicalAngle, out helicalDeltaPosition, out helicalPosition);
        float helicalDeltaPath, helicalPath;
        CalculateDeltaPath(out helicalDeltaPath, out helicalPath);

        TrySetParam(ParamName.time, time);
        TrySetParam(ParamName.position, helicalPosition);
        TrySetParam(ParamName.pathTraveled, helicalPath);
        TrySetParam(ParamName.deltaPathTraveled, helicalDeltaPath);
        TrySetParam(ParamName.rotationFrequency, RotationalMotionModel.GetParam(ParamName.rotationFrequency));
        TrySetParam(ParamName.angularVelocity, RotationalMotionModel.GetParam(ParamName.angularVelocity));
        TrySetParam(ParamName.period, RotationalMotionModel.GetParam(ParamName.period));
        TrySetParam(ParamName.angleRadTraveled, RotationalMotionModel.GetParam(ParamName.angleRadTraveled));
        TrySetParam(ParamName.angleRad, RotationalMotionModel.GetParam(ParamName.angleRad));
        TrySetParam(ParamName.numberOfRevolutions, RotationalMotionModel.GetParam(ParamName.numberOfRevolutions));
        return helicalPosition;
    }
    protected float CalculateHelicalFrequencyFromVelocity(float velocityMagnitude, float helicalAngle)
    {
        float d = (float)GetParam(ParamName.radius) * 2;
        float angleRad = helicalAngle * Mathf.Deg2Rad;
        float frequency;
        if (d == 0)
            frequency = 0;
        else
            frequency = (velocityMagnitude * Mathf.Cos(angleRad)) / (Mathf.PI * d);

        return frequency;
    }

    protected float CalculateHelicalStepFromVelocity(float velocityMagnitude, float helicalAngle)
    {
        float angleRad = helicalAngle * Mathf.Deg2Rad;
        float step = velocityMagnitude * Mathf.Sin(angleRad);
        return step;
    }

    protected virtual void CalculateDeltaPosition(float deltaTime, float velocity, float angle, out Vector3 helicalDeltaPosition, out Vector3 helicalPosition)
    {
        Vector3 linearDeltaPosition = GetLinearDeltaPosition(deltaTime, velocity, angle);
        Vector3 rotationalDeltaPosition = GetRotationalDeltaPosition(deltaTime, velocity,  angle);
        helicalDeltaPosition = linearDeltaPosition + rotationalDeltaPosition;
        Vector3 pos = (Vector3)GetParam(ParamName.position);
        helicalPosition = pos + helicalDeltaPosition;
    }

    protected virtual Vector3 GetLinearDeltaPosition(float deltaTime, float velocity, float angle)
    {
        float step = CalculateHelicalStepFromVelocity(velocity, angle);
        LinearMotionModel.TrySetParam(ParamName.velocity, new Vector3(0,step, 0));
        LinearMotionModel.UpdatePosition(deltaTime);
        Vector3 linearDeltaPosition = (Vector3)LinearMotionModel.GetParam(ParamName.deltaPosition);
        return linearDeltaPosition;
    }

    protected virtual Vector3 GetRotationalDeltaPosition(float deltaTime, float velocity, float angle)
    {
        float rotationFrequency = CalculateHelicalFrequencyFromVelocity(velocity, angle);
        if(rotationFrequency == 0 )
        {
            RotationalMotionModel.TrySetParam(ParamName.velocity, Vector3.forward);
        }
        else
            RotationalMotionModel.TrySetParam(ParamName.velocity, Vector3.up);
        RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, rotationFrequency);
        RotationalMotionModel.UpdatePosition(deltaTime);
        Vector3 rotationalDeltaPosition = (Vector3)RotationalMotionModel.GetParam(ParamName.deltaPosition);
        return rotationalDeltaPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        float velocityMagnitude = (float)GetParam(ParamName.velocityMagnitude);
        float helicalAngle = (float)GetParam(ParamName.helicalAngle);

        Vector3 helicalDeltaPosition, helicalPosition;
        CalculateDeltaPosition(time, velocityMagnitude, helicalAngle, out helicalDeltaPosition, out helicalPosition);
        helicalDeltaPosition = SanitizeVector(helicalDeltaPosition);
        helicalPosition = SanitizeVector(helicalPosition);

        float helicalDeltaPath, helicalPath;
        CalculateDeltaPath(out helicalDeltaPath, out helicalPath);

        float rotationFrequency = CalculateHelicalFrequencyFromVelocity(velocityMagnitude, helicalAngle);

        TrySetParam(ParamName.position, helicalPosition);
        TrySetParam(ParamName.pathTraveled, helicalPath);
        TrySetParam(ParamName.deltaPathTraveled, helicalDeltaPath);
        TrySetParam(ParamName.deltaPosition, helicalDeltaPosition);
        TrySetParam(ParamName.angularVelocity, RotationalMotionModel.GetParam(ParamName.angularVelocity));
        TrySetParam(ParamName.period, RotationalMotionModel.GetParam(ParamName.period));
        //TrySetParam(ParamName.velocity, velocity);
        TrySetParam(ParamName.angleRadTraveled, RotationalMotionModel.GetParam(ParamName.angleRadTraveled));
        TrySetParam(ParamName.angleRad, RotationalMotionModel.GetParam(ParamName.angleRad));
        TrySetParam(ParamName.numberOfRevolutions, RotationalMotionModel.GetParam(ParamName.numberOfRevolutions));
        TrySetParam(ParamName.rotationFrequency, rotationFrequency);
        return helicalPosition;
    }

    private Vector3 SanitizeVector(Vector3 helicalPosition)
    {
        return helicalPosition;
    }

    protected void CalculateDeltaPath(out float helicalDeltaPath, out float helicalPath)
    {
        float linearDeltaPath = GetFloatParam(LinearMotionModel, ParamName.deltaPathTraveled);
        float rotationalDeltaPath = GetFloatParam(RotationalMotionModel, ParamName.deltaPathTraveled);
        helicalDeltaPath = Mathf.Sqrt(linearDeltaPath * linearDeltaPath + rotationalDeltaPath * rotationalDeltaPath);
        helicalPath = GetFloatParam(ParamName.pathTraveled) + helicalDeltaPath;
    }


    private static float GetFloatParam(MotionModel source, ParamName paramName)
    {
        if (source == null)
            return 0f;

        object value = source.GetParam(paramName);
        return value is float floatValue ? floatValue : 0f;
    }

    private float GetFloatParam(ParamName paramName)
    {
        object value = GetParam(paramName);
        return value is float floatValue ? floatValue : 0f;
    }
    //protected virtual void InitRotationalMotionModelParams()
    //{
    //    RotationalMotionModel.TrySetParam(ParamName.radius, GetParam(ParamName.radius));
    //    RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, GetParam(ParamName.rotationFrequency));
    //}
    private static Vector3 SafeDivide(Vector3 value, float divisor)
    {
        return Mathf.Approximately(divisor, 0f) ? Vector3.zero : value / divisor;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            new TopicField(ParamName.velocityMagnitude, FieldType.Float, false),
            new TopicField(ParamName.radius, FieldType.Float, false),
            new TopicField(ParamName.helicalAngle, FieldType.Float, false),

            new TopicField(ParamName.rotationFrequency, FieldType.Float, true),
            new TopicField(ParamName.time, FieldType.Float, true),
            new TopicField(ParamName.position, FieldType.Vector3, true),
            new TopicField(ParamName.pathTraveled, FieldType.Float, true),
            new TopicField(ParamName.deltaPathTraveled, FieldType.Float, true),
            new TopicField(ParamName.angularVelocity, FieldType.Float, true),
            new TopicField(ParamName.period, FieldType.Float, true),
            new TopicField(ParamName.angleRad, FieldType.Float, true),
            new TopicField(ParamName.angleRadTraveled, FieldType.Float, true),
            new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true)
        };
    }
}





















