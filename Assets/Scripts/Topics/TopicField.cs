using System;
using System.Collections.Generic;
using UniRx;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class TopicField
{
    [SerializeField] private ParamName paramName;
    [SerializeField, NonSerialized] private FieldType type;
    [SerializeField, NonSerialized] private bool isReadOnly;
    //[SerializeField,ReadOnly] private string stringValue;
    private ReactiveProperty<object> property = new ReactiveProperty<object>();
    private static Dictionary<ParamName, FieldType> paramNameFieldTypes = new Dictionary<ParamName, FieldType>()
    {
        { ParamName.angleDeg,FieldType.Float } ,
        { ParamName.angleRad,FieldType.Float } ,
        { ParamName.additionalMass,FieldType.Bool } ,
        { ParamName.distance,FieldType.Float },
        { ParamName.friction,FieldType.Float } ,
        { ParamName.force,FieldType.Float } ,
        { ParamName.forceAcceleration,FieldType.Float },
        { ParamName.isMoving,FieldType.Bool } ,
        { ParamName.radius,FieldType.Float } ,
        { ParamName.refractiveIndex,FieldType.Float } ,
        { ParamName.position,FieldType.Vector3 } ,
        { ParamName.position2,FieldType.Vector3 } ,
        { ParamName.xPosition,FieldType.Float } ,
        { ParamName.velocity,FieldType.Vector3 } ,
        { ParamName.velocity2,FieldType.Vector3 } ,
        { ParamName.seed,FieldType.Int } ,
        { ParamName.unityPhycicsCalculation,FieldType.Bool } ,
        { ParamName.mass,FieldType.Float } ,
        { ParamName.mass2,FieldType.Float } ,
        {  ParamName.material1_Size,FieldType.Vector3},
        {  ParamName.material1_Position,FieldType.Vector3 },
        {  ParamName.material1_RefractiveIndex ,FieldType.Float },
        {  ParamName.material2_Size,FieldType.Vector3},
        {  ParamName.material2_Position,FieldType.Vector3 },
        {  ParamName.material2_RefractiveIndex ,FieldType.Float },
        {  ParamName.material3_Size,FieldType.Vector3},
        {  ParamName.material3_Position,FieldType.Vector3 },
        {  ParamName.material3_RefractiveIndex ,FieldType.Float },
    };

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
    private static Dictionary<FieldType, object> defaultValues = new Dictionary<FieldType, object>()
    {
        {FieldType.Float , 0f }
        ,{FieldType.Int , (int)1},
        {FieldType.Bool , false},
        {FieldType.Vector3, Vector3.zero}
    };
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
                    Debug.Log(new Vector3(x, y, z));
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

    public bool TrySetValue(string str,bool isUserChange = false)
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

    public bool TrySetValue(object value,bool notify = true)
    {
        Debug.Log(value);
        if (value == null)
        {
            Debug.LogAssertion("trying set null value");
            SetValue(value, false);
            return true;
        }
        Type valueType = value.GetType();
        if (FieldType == FieldType.Custom)
        {
            return true;
        }
        if (GetFieldValueType(FieldType) != valueType)
        {
            object convertedValue = TryConvertValue(value, out bool result);
            if (result)
            {
                value = convertedValue;
            }
            else
            {
                Debug.LogAssertion("Convertion (value) " + value + " of type " + valueType + " to " + ParamName + " of type  " + FieldType + " failed");
                return false;
                //Debug.LogAssertion("Cant set (value) " + value + " of type " + valueType + " to " + ParamName + " of type  " + FieldType);
            }
        }
        object clampedValue = ClampValue(value);
        SetValue(value, notify);
        return true;
    }
    private void SetValue(object value, bool notify = true )
    {
        if(notify)
            property.SetValueAndForceNotify(value);
        else
            property.Value = value;
    }
    private object TryConvertValue(object value, out bool result)
    {
        return GetValueFromString(value.ToString(), out  result);
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
    angleDegTraveled,
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
    helicalAngle
    helicalAngle,
    rotationalAxis
}
public enum FieldType {None, Float, Vector3, Int,Bool,
    Custom
}
public enum FieldType { Float, Vector3, Int,Bool }
