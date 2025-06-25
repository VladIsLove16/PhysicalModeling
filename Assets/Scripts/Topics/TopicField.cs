using System;
using System.Collections.Generic;
using UniRx;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class TopicField
{
    [SerializeField] private ParamName paramName;
    [SerializeField, NonSerialized] private FieldType type = FieldType.None;
    [SerializeField, NonSerialized] private bool isReadOnly;
    //[SerializeField,ReadOnly] private string stringValue;
    private ReactiveProperty<object> property = new ReactiveProperty<object>();
  
    public object MaxValue;
    public object MinValue;
    private object maxValue;
    private object minValue;
    public ParamName ParamName => paramName;
    public FieldType FieldType => type;
    public bool IsReadOnly => isReadOnly;
    public object Value => property.Value;
    public ReactiveProperty<object> Property => property;
    public TopicField() { }

    //[Obsolete("Используйте метод TopicField(ParamName ParamName) вместо этого.")]
    public TopicField(ParamName paramName, FieldType type = FieldType.None, bool isReadonly = false)
    {
        this.paramName = paramName;
        this.type = type;
        this.isReadOnly = isReadonly;
        //property.Subscribe(_ => PropertyChanged());
    }

    public TopicField(ParamName paramName, bool isReadonly = false, FieldType type = FieldType.None)
    {
        this.paramName = paramName;
        this.type = type;
        this.isReadOnly = isReadonly;
        //property.Subscribe(_ => PropertyChanged());
    }

    public void SetType(FieldType type, bool isForce = false)
    {
        if(isForce || FieldType == FieldType.None)
            this.type = type;
    }

    public static Type GetFieldValueType(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.Float =>typeof( float),
            FieldType.Int => typeof(int),
            FieldType.Vector3 => typeof(Vector3),
            FieldType.Bool => typeof(bool),
            _ => typeof(float)
        };
    }

    public bool TrySetValue(object value, bool notify = true)
    {
        if (value == null)
        {
            Debug.LogAssertion("trying set null value");
            return true;
        }
        Type valueType = value.GetType();
        if (FieldType == FieldType.Custom)
        {
            SetValue(value, false);
            return true;
        }
        if (GetFieldValueType(FieldType) != valueType)
        {
            Debug.LogAssertion("not coincidental types" + value + " of type " + valueType + " to " + ParamName + " of type  " + FieldType);
            //object convertedValue = TryConvertValue(value, out bool result);
            //if (result)
            //{
            //    value = convertedValue;
            //}
            //else
            //{
            //    Debug.LogAssertion("Convertion (value) " + value + " of type " + valueType + " to " + ParamName + " of type  " + FieldType + " failed");
            //    return false;
            //}
        }
        object clampedValue = ClampValue(value);
        SetValue(value, notify);
        return true;
    }
    private void SetValue(object value, bool notify = true )
    {
        Debug.Log("new value " + paramName + " " + value.ToString());
        if(notify)
            property.SetValueAndForceNotify(value);
        else
            property.Value = value;
    }
    

    private object ClampValue(object value)
    {
        if (MaxValue == null || MinValue == null)
        {
            return value;
        } 
        if (value is float fValue)
        {
            fValue = Mathf.Clamp(fValue, (float)MinValue, (float)MaxValue);
            return fValue;
        }
        else if (value is int intValue)
        {
            intValue = Math.Clamp(intValue, (int)MinValue, (int)MaxValue);
            return intValue;
        }
        else
            return value;
    }

    internal void SetMaxValue(object v)
    {
        MaxValue = v;
    }

    internal void SetMinValue(object v)
    {
         MinValue = v;
    }
}
public enum ParamName
{
    velocity,
    velocityMagnitude,
    distance,
    pathTraveled,
    time,
    position,
    acceleration,
    jerk,
    angularVelocity,
    angleDeg,
    angleRad,
    angleRadTraveled,
    period,
    radius,
    rotationFrequency,
    rotationFrequencyAcceleration,
    rotationFrequencyJerk,
    numberOfRevolutions,
    step,
    deltaPosition,
    deltaPathTraveled,
    accelerationStartTime,
    flightTime,
    landingVelocity,
    range,
    averageSpeed,
    mass,
    mass2,
    obstaclesMass,
    velocity2,
    pointAReached,
    seed,
    respawnObstacles,
    friction,
    force,
    forceAcceleration,
    refractiveIndex,
    unityPhycicsCalculation,
    xPosition,
    rayAngle,
    material1_Size,
    material1_Position,
    material1_RefractiveIndex,
    material2_Size,
    material2_Position,
    material2_RefractiveIndex,
    material3_Size,
    material3_Position,
    material3_RefractiveIndex,
    additionalMass,
    position2,
    mass2Acceleration,
    isMoving,
    gearCount,
    module,
    teethCount,
    gearBox,
    totalGearRatio,
    outputAngularVelocity,
    outputFrequency,
    inputAngularVelocity,
    inputFrequency,
    helicalAngle,
    volume,
    density,
    piston1Square,
    piston2Square,
    pistonHeightDelta,
    weight,
    applyingForce,
    startingPosition,
    Submarine,
    densityCount,
    startTime,
    startTime1,
    density1,
    startTime2,
    density2,
    density3,
    startTime3
}
public enum FieldType {None, Float, Vector3, Int,Bool,
    Custom
}
