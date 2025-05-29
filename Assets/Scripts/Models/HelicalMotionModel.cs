using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "HelicalMotionModel", menuName = "MotionModelsDropdown/HelicalMotionModel")]
public class HelicalMotionModel : MotionModel
{
    protected MotionModel rotationalMotionModel;
    protected MotionModel linearMotionModel;
    protected Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
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
        { ParamName.rotationFrequency, 1f },
        { ParamName.helicalAngle, 10f },
    };
  
    private static Dictionary<ParamName, object> minValues = new Dictionary<ParamName, object>()
    {
        { ParamName.helicalAngle, 0f },
        { ParamName.radius,  0f},
    };
    private static Dictionary<ParamName, object> maxValues = new Dictionary<ParamName, object>()
    {
        { ParamName.helicalAngle, 89f },
        { ParamName.radius, 5f },
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

    public override void InitializeParameters(bool isForce = false)
    {
        base.InitializeParameters(isForce);
        actions[ParamName.helicalAngle] = OnHelicalAngleChanged;
        actions[ParamName.step] = OnStepChanged;
        actions[ParamName.radius] = OnRadiusChanged;
        actions[ParamName.rotationFrequency] = OnRotationFrequencyChanged;
        foreach (var field in topicFields)
        {
            if(actions.ContainsKey( field.ParamName ))
            {
                Debug.Log("field name " +  field.ParamName);
                field.Property.Skip(1).Subscribe(actions[field.ParamName]);

            }
        }
    }

    private void OnRotationFrequencyChanged(object value)
    {
        //if ((float)value == (float)GetParam(ParamName.rotationFrequency))
        //    return;
        RotationalMotionModel.TrySetParam(ParamName.rotationFrequency, value);
    }

    private void OnRadiusChanged(object value)
    {
        Debug.Log("radius changed");
        //if ((float)value == (float)GetParam(ParamName.radius))
        //    return;
        RotationalMotionModel.TrySetParam(ParamName.radius, value);
    }

    private void OnStepChanged(object value)
    {
        float newStep = (float)value;
        float radius = (float)GetParam(ParamName.radius);
        float newAngle = CalculateAngle(newStep, radius);
        Vector3 axis = (Vector3)GetParam(ParamName.rotationalAxis);
        Vector3 newVelocity;
        if (axis == Vector3.zero || axis.magnitude == 0f)
            newVelocity = newStep * Vector3.up;
        else
            newVelocity = newStep * axis.normalized;
        LinearMotionModel.TrySetParam(ParamName.velocity, newVelocity);
        if(newAngle != (float)GetParam(ParamName.helicalAngle))
            base.TrySetParam(ParamName.helicalAngle, newAngle,false);
    }

    private void OnHelicalAngleChanged(object value)
    {
        float newAngle = (float)value;
        float oldAngle = (float)GetParam(ParamName.helicalAngle);
        //if (newAngle == oldAngle)
        //    return;
        float newStep = CalculateStep(newAngle, (float)GetParam(ParamName.radius));
        Vector3 axis = (Vector3) GetParam(ParamName.rotationalAxis);
        Vector3 newVelocity;
        if (axis == Vector3.zero || axis.magnitude == 0f)
            newVelocity = newStep * Vector3.up;
        else
            newVelocity = newStep * axis.normalized;
        LinearMotionModel.TrySetParam(ParamName.velocity, newVelocity);
        base.TrySetParam(ParamName.step, newStep, false);
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Debug.Log(GetParam(ParamName.radius) + " current radius");
        Debug.Log(RotationalMotionModel.GetParam(ParamName.radius) + "rotationalMotionModel current radius");
        float time = (float)GetParam(ParamName.time)+deltaTime;
        Vector3 helicalDeltaPosition, newHelicalPosition;
        GetTotalDeltaPosition(deltaTime, out helicalDeltaPosition, out newHelicalPosition);

        float helicalDeltaPath, helicalPath;
        CalculateDeltaPath(out helicalDeltaPath, out helicalPath);

        TrySetParam(ParamName.time, time);
        TrySetParam(ParamName.position, newHelicalPosition);
        TrySetParam(ParamName.pathTraveled, helicalPath);

        TrySetParam(ParamName.angularVelocity, RotationalMotionModel.GetParam(ParamName.angularVelocity));
        TrySetParam(ParamName.period, RotationalMotionModel.GetParam(ParamName.period));
        TrySetParam(ParamName.angleDegTraveled, (float)RotationalMotionModel.GetParam(ParamName.angleRadTraveled ) * Mathf.Rad2Deg);
        TrySetParam(ParamName.numberOfRevolutions, RotationalMotionModel.GetParam(ParamName.numberOfRevolutions));
        return newHelicalPosition;
    }
    protected float CalculateStep(float angle, float radius)
    {
        float d = radius * 2f;
        float h = Mathf.Tan(angle) * Mathf.PI * d;
        return h;
    }
    protected float CalculateAngle(float h,float radius)
    {
        float d = radius * 2;
        float tanAngle = h / Mathf.PI / d;
        float angle = Mathf.Atan(tanAngle)*Mathf.Rad2Deg;
        return angle;
    }
    protected void GetTotalDeltaPosition(float deltaTime, out Vector3 helicalDeltaPosition, out Vector3 helicalPosition)
    {
        Vector3 linearDeltaPosition = GetLinearDeltaPosition(deltaTime);
        Vector3 rotationalDeltaPosition = GetRotationalDeltaPosition(deltaTime);
        helicalDeltaPosition = linearDeltaPosition + rotationalDeltaPosition;
        Vector3 pos = (Vector3)GetParam(ParamName.position);
        helicalPosition = pos + helicalDeltaPosition;
    }

    protected Vector3 GetLinearDeltaPosition(float deltaTime)
    {
        LinearMotionModel.UpdatePosition(deltaTime);
        Vector3 linearDeltaPosition = (Vector3)LinearMotionModel.GetParam(ParamName.deltaPosition);
        return linearDeltaPosition;
    }

    protected Vector3 GetRotationalDeltaPosition(float deltaTime)
    {
        RotationalMotionModel.UpdatePosition(deltaTime);
        Vector3 rotationalDeltaPosition = (Vector3)RotationalMotionModel.GetParam(ParamName.deltaPosition);
        return rotationalDeltaPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        return UpdatePosition(time);
    }
    protected void CalculateDeltaPath(out float helicalDeltaPath, out float helicalPath)
    {
        float linearDeltaPath = (float)RotationalMotionModel.GetParam(ParamName.deltaPathTraveled);
        float rotationalDeltaPath = (float)RotationalMotionModel.GetParam(ParamName.deltaPathTraveled);
        helicalDeltaPath = (float)Mathf.Sqrt(linearDeltaPath * linearDeltaPath + rotationalDeltaPath * rotationalDeltaPath);
        helicalPath = (float)GetParam(ParamName.pathTraveled) + helicalDeltaPath;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            new TopicField(ParamName.helicalAngle, FieldType.Float, false),
            new TopicField(ParamName.radius, FieldType.Float, false),
            new TopicField(ParamName.rotationFrequency, FieldType.Float, false),
            new TopicField(ParamName.step, FieldType.Float, false),
            new TopicField(ParamName.rotationalAxis, FieldType.Vector3, false),

            new TopicField(ParamName.time, FieldType.Float, true),
            new TopicField(ParamName.position, FieldType.Vector3, true),
            new TopicField(ParamName.velocityMagnitude, FieldType.Float, true),
            new TopicField(ParamName.pathTraveled, FieldType.Float, true),
            new TopicField(ParamName.angularVelocity, FieldType.Float, true),
            new TopicField(ParamName.period, FieldType.Float, true),
            new TopicField(ParamName.angleDeg, FieldType.Float, true),
            new TopicField(ParamName.angleDegTraveled, FieldType.Float, true),
            new TopicField(ParamName.numberOfRevolutions, FieldType.Float, true)
        };
    }
}

    