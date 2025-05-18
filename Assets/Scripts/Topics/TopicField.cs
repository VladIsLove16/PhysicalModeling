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
    [SerializeField, NonSerialized] private FieldType type;
    [SerializeField, NonSerialized] private bool isReadOnly;
    //[SerializeField,ReadOnly] private string stringValue;
    private ReactiveProperty<object> property = new ReactiveProperty<object>();
    private Dictionary<ParamName, FieldType> paramNameFieldTypes = new Dictionary<ParamName, FieldType>()
    {
        { ParamName.angle,FieldType.Float } ,
        { ParamName.angleRad,FieldType.Float } ,
        { ParamName.distance,FieldType.Float },
        { ParamName.friction,FieldType.Float } ,
        { ParamName.force,FieldType.Float } ,
        { ParamName.forceAcceleration,FieldType.Float } ,
        { ParamName.radius,FieldType.Float } ,
        { ParamName.refractiveIndex,FieldType.Float } ,
        { ParamName.unityPhycicsCalculation,FieldType.Bool } ,
        { ParamName.position,FieldType.Vector3 } ,
        { ParamName.velocity,FieldType.Vector3 } ,
        { ParamName.velocity2,FieldType.Vector3 } ,
        { ParamName.seed,FieldType.Int } ,
        { ParamName.mass,FieldType.Float } ,
    }; 
           

    public static Dictionary<ParamName, float> MaxValues = new Dictionary<ParamName, float>()
    {
        { ParamName.seed, 1000000f },
        { ParamName.angle, 360f },
    };
    internal float maxValue => MaxValues[paramName];

    public ParamName ParamName => paramName;
    public FieldType FieldType => type;
    public bool IsReadOnly => isReadOnly;
    public object Value => property.Value;
    public ReactiveProperty<object> Property => property;
    public TopicField() { }

    //[Obsolete("Используйте метод TopicField(ParamName paramName) вместо этого.")]
    public TopicField(ParamName paramName, FieldType type = FieldType.Float, bool isReadonly = false)
    {
        this.paramName = paramName;
        if (TryGetType(paramName, out FieldType resultType))
            this.type = resultType;
        else
            this.type = type;
        this.isReadOnly = isReadonly;
        //property.Subscribe(_ => OnPropertyChanged());
    }
    public void SetMaxValueForce( float value)
    {
        MaxValues[paramName] = value;
    }
    public TopicField(ParamName paramName, bool isReadonly = false, FieldType type = FieldType.Float)
    {
        this.paramName = paramName;
        if (TryGetType(paramName, out FieldType resultType))
            this.type = resultType;
        else
            this.type = type;
        this.isReadOnly = isReadonly;
        //property.Subscribe(_ => OnPropertyChanged());
    }
    public string GetStringValue()
    {
       return GetStringFromValue(Property.Value);
    }
    public void SetTypeForce(FieldType type)
    {
        this.type = type;
    }

    private bool TryGetType(ParamName paramName, out FieldType fieldType)
    {
        if(paramNameFieldTypes.TryGetValue(paramName, out fieldType))
            return true;
        else
            return false;
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
    public string GetStringFromValue(object obj)
    {
        if(obj == null)
            return "null value";
        string valueText = obj switch
        {
            float floatValue => floatValue.ToString("0.00"),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x.ToString("0.00")};{v.y.ToString("0.00")};{v.z.ToString("0.0000")}",
            string stringValue => stringValue,
            bool boolValue => boolValue == true ? true.ToString() : false.ToString(),
            _ => obj.ToString()
        };
        return valueText;
    }

    public object GetValueFromString(string value, out bool result)
    {
        switch (FieldType)
        {
            case FieldType.Float:
                result = float.TryParse(value, out float floatValue);
                if (result)
                    return floatValue;
                else
                    return 0f;
            case FieldType.Int:
                result = int.TryParse(value, out int intValue);
                if (result)
                    return intValue;
                else
                    return 0;
            case FieldType.Vector3:
                string[] values = value.Split(';');

                if (values.Length == 3 &&
                    float.TryParse(values[0], out float x) &&
                    float.TryParse(values[1], out float y) &&
                    float.TryParse(values[2], out float z))
                {
                    result = true;
                    return new Vector3(x, y, z);
                }
                else
                {
                    result = false;
                    return Vector3.zero;
                }
            case FieldType.Bool:
                if (value == false.ToString())
                {
                    result = true;
                    return false;
                }
                else if (value == true.ToString())
                {
                    result = true;
                    return true;
                }
                else
                {
                    result = false;
                    return false;
                }
            default:
                result = false;
                return null;
        }
    }

    public bool TrySetValue(string str)
    {
        object value = GetValueFromString(str, out bool result);
        if (result) 
        {
            TrySetValue(value);
            return true;
        }
        Debug.LogAssertion("Cant set (string) " + str + " to " +  ParamName  + " of type  " + FieldType);
        return false;
    }

    public bool TrySetValue(object value)
    {
        Type valueType = value.GetType();
        if (GetFieldValueType(FieldType) == valueType)
        { 
            property.SetValueAndForceNotify(value);
            return true;
        }
        Debug.LogAssertion("Cant set (value) " + value +  " of type " + valueType +  " to " + ParamName + " of type  " + FieldType);
        return true;
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
    angle,
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
    unityPhycicsCalculation
}
public enum FieldType { Float, Vector3, Int,Bool }
