using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    //public Action paramsChanged;
    public enum SimulationState
    {
        continued,
        started,
        stoped,
        paused
    }
    [SerializeField] public string Title;
    //вторая идея: добавить метод, который будет пересчитывать другие параметры на основе измененного пользователем, как со временем. 
    protected List<TopicField> m_topicFields= new List<TopicField>();
    protected List<TopicField> topicFields
    {
        get 
        {
            return m_topicFields;
        }
    }
    public int TopicFieldsCount => topicFields.Count;
    public TopicField GetTopicField(ParamName paramName, out bool result)
    {
        TopicField topicField = GetTopicField( paramName);
        result = topicField != null || topicField != default;
        return topicField;
    }
    public TopicField GetTopicField(ParamName paramName)
    {
        return topicFields.FirstOrDefault(x => x.ParamName == paramName);
    }

    //private Dictionary<ParamName, ReactiveProperty<object>> paramValues;
    //public Dictionary<ParamName, ReactiveProperty<object>> Params
    //{
    //    get
    //    {
    //        if (paramValues == null || paramValues.Count == 0)
    //            paramValues = topicFields.ToDictionary(
    //        x => x.Key,
    //        x => x.Value.Property);
    //        return paramValues;
    //    }
    //}
    protected bool isInitialized = false;
    protected ReactiveDictionary<FieldType, object> DefaultFieldTypeValues = new ReactiveDictionary<FieldType, object>()
    {
        { FieldType.Float, 0f },
        { FieldType.Vector3, Vector3.zero },
        { FieldType.Int, 0 },
        { FieldType.Bool, false },
    };
    protected static Dictionary<ParamName, FieldType> paramNameFieldTypes = new Dictionary<ParamName, FieldType>()
    {
        { ParamName.angleDeg,FieldType.Float } ,
        { ParamName.angleRad,FieldType.Float } ,
        { ParamName.acceleration,FieldType.Vector3 } ,
        { ParamName.additionalMass,FieldType.Bool } ,
        { ParamName.distance,FieldType.Float },
        { ParamName.deltaPosition,FieldType.Vector3 },
        { ParamName.friction,FieldType.Float } ,
        { ParamName.force,FieldType.Float } ,
        { ParamName.forceAcceleration,FieldType.Float },
        { ParamName.gearBox,FieldType.Custom },
        { ParamName.gearCount,FieldType.Int },
        { ParamName.isMoving,FieldType.Bool } ,
        { ParamName.inputAngularVelocity,FieldType.Float } ,
        { ParamName.inputFrequency,FieldType.Float } ,
        { ParamName.jerk,FieldType.Vector3 },
        { ParamName.mass,FieldType.Float } ,
        { ParamName.mass2,FieldType.Float } ,
        { ParamName.module,FieldType.Float },
        { ParamName.radius,FieldType.Float } ,
        { ParamName.rayAngle,FieldType.Float } ,
        { ParamName.refractiveIndex,FieldType.Float } ,
        { ParamName.outputAngularVelocity,FieldType.Float } ,
        { ParamName.outputFrequency,FieldType.Float } ,
        { ParamName.position,FieldType.Vector3 } ,
        { ParamName.position2,FieldType.Vector3 } ,
        { ParamName.totalGearRatio,FieldType.Float } ,
        { ParamName.teethCount,FieldType.Int } ,
        { ParamName.xPosition,FieldType.Float } ,
        { ParamName.velocity,FieldType.Vector3 } ,
        { ParamName.velocity2,FieldType.Vector3 } ,
        { ParamName.seed,FieldType.Int } ,
        { ParamName.unityPhycicsCalculation,FieldType.Bool } ,

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
    protected virtual Dictionary< ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
            {

            };
        }
    }
    protected virtual Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new();
        }
    }
    protected virtual Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new();
        }
    }
    public virtual void InitializeParameters(bool isForce = false)
    {
        //if (isInitialized && !isForce)
        //    return;
        Debug.Log("InitializeParameters");
        isInitialized = true;
        Debug.Log(topicFields.Count);
        var TopicFieldsList = GetRequiredParams();
        ClearTopicFields();
        foreach (var field in TopicFieldsList)
        {
            if(field.FieldType == FieldType.None)
            {
                var type = paramNameFieldTypes[field.ParamName];
                field.SetType(type, true);
            }
            if (MaxValues.TryGetValue(field.ParamName, out object maxValue))
                field.SetMaxValue(maxValue);
            if(MinValues.TryGetValue(field.ParamName, out object minValue))
                field.SetMinValue(minValue);
            object defaultValue = GetDefaultValue(field);
            field.TrySetValue(defaultValue);
            AddTopicField(field);
        }
        Debug.Log(topicFields.Count);
    }
    private void ClearTopicFields()
    {
        topicFields.Clear();
    }
    private void AddTopicField(TopicField topicField)
    {
        topicFields.Add(topicField);
    }
    public abstract Vector3 UpdatePosition(float deltaTime);
    public abstract Vector3 CalculatePosition(float Time);
    public abstract List<TopicField> GetRequiredParams();

    public virtual void ResetParams()
    {
        foreach (var pair in topicFields)
        {
            ResetParam(pair.ParamName);
        }
    }

    public object GetDefaultValue(ParamName paramName)
    {
       return GetDefaultValue(GetTopicField(paramName));
    }
    public object GetDefaultValue(TopicField topicField)
    {
        if(DefaultValues.ContainsKey(topicField.ParamName))
            return DefaultValues[topicField.ParamName];
        if (DefaultFieldTypeValues.ContainsKey(topicField.FieldType))
            return DefaultFieldTypeValues[topicField.FieldType];
        return null;
    }

    public virtual void ResetParam(ParamName paramName)
    {
        var defaultValue = GetDefaultValue(paramName);
        if (defaultValue != null)
            GetTopicField(paramName).TrySetValue(defaultValue);
        else
            Debug.Log("Default value not found, setted null " + paramName + " of type " + GetTopicField(paramName).FieldType);
    }

    public virtual void ResetParam(TopicField field)
    {
        field.TrySetValue(GetDefaultValue(field.ParamName));
    }

    public object GetParam(ParamName paramName)
    {
        TopicField topicField = GetTopicField(paramName);
        if (topicField != null)
            return topicField.Value;
        else
            return null;
    }

    //public object GetParam(ParamName paramName,out bool res)
    //{
    //    TopicField topicField = GetTopicField(paramName);
    //    res = topicField != null;
    //    if(res)
    //        return topicField.Value;
    //    else
    //        return null;
    //}
    public virtual bool TrySetParam(ParamName paramName, object value, bool notify = true)
    {
        TopicField topicField = GetTopicField(paramName,out bool result);
        if (!result)
        {
            Debug.LogWarning(paramName + " not found");
            return false;
        }
        if (topicField.TrySetValue(value, notify))
        {
            return true;
        }
        return false;
    }
    public  virtual void OnDisabled()
    {
        
    }
    public  virtual void OnEnabled()
    {
        
    }

    public virtual void OnSimulationStateChanged(SimulationState value)
    {
       
    }

    internal List< TopicField> GetTopicFields(bool recreation)
    {
        if (recreation)
            InitializeParameters(true);
        return topicFields;
    }
}
