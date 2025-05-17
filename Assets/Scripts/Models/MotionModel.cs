using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    public enum SimulationState
    {
        continued,
        started,
        stoped,
        paused
    }
    [SerializeField] public string Title;
    //вторая идея: добавить метод, который будет пересчитывать другие параметры на основе измененного пользователем, как со временем. 
    protected Dictionary<ParamName, TopicField> topicFields = new();
    public Dictionary<ParamName, TopicField> TopicFields
    {
        get
        {
            if(topicFields==null || topicFields.Count == 0)
            {
                InitializeParameters(true);
            }
            return topicFields;
        }
    }

    //private Dictionary<ParamName, ReactiveProperty<object>> paramValues;
    //public Dictionary<ParamName, ReactiveProperty<object>> Params
    //{
    //    get
    //    {
    //        if (paramValues == null || paramValues.Count == 0)
    //            paramValues = TopicFields.ToDictionary(
    //        x => x.Key,
    //        x => x.Value.Property);
    //        return paramValues;
    //    }
    //}
    private bool isInitialized;
    protected ReactiveDictionary<FieldType, object> DefaultValues = new ReactiveDictionary<FieldType, object>()
    {
        { FieldType.Float, 0f },
        { FieldType.Vector3, Vector3.zero },
        { FieldType.Int, 0 },
        { FieldType.Bool, false },
    };
    [SerializeField] protected List<TopicField> TopicFieldsList;
    public virtual void InitializeParameters(bool isForce = false)
    {
        if (isInitialized && !isForce)
            return;
        TopicFieldsList = GetRequiredParams();
        foreach (var field in TopicFieldsList)
        {
            topicFields[field.ParamName] = field;
            field.TrySetValue(DefaultValues[field.FieldType]);
        }
        isInitialized= true;
    }
    public abstract Vector3 UpdatePosition(float deltaTime);
    public abstract Vector3 CalculatePosition(float Time);
    public abstract List<TopicField> GetRequiredParams();

    public virtual void ResetParams()
    {
        foreach (var pair in topicFields)
        {
            ResetParam(pair.Key);
        }
    }
    public FieldType GetFieldType(ParamName value)
    {
        return topicFields[value].FieldType;
    }

    public object GetDefaultValue(FieldType fieldType)
    {
        return DefaultValues[fieldType];
    }
    public object GetDefaultValue(ParamName paramName)
    {
        return DefaultValues[topicFields[paramName].FieldType];
    }
    public virtual void ResetParam(ParamName parametrName)
    {
        topicFields[parametrName].TrySetValue(GetDefaultValue(parametrName));
    }
    public object GetParam(ParamName paramName)
    {
        return topicFields[paramName].Value;
    }
    public virtual bool TrySetParam(ParamName paramName, object value)
    {
        if (!TopicFields.TryGetValue(paramName, out TopicField topicField))
        {
            Debug.LogWarning(paramName + " not found");
            return false;
        }
        if (topicField.TrySetValue(value))
        {
            return true;
        }
        return false;
    }
    public bool IsReadonly(ParamName paramName)
    {
        return topicFields[paramName].IsReadOnly;
    }
    public  virtual void OnDisabled()
    {
        
    }
    public  virtual void OnEnabled()
    {
        
    }

    public void GetParamStringValue(ParamName paramName)
    {
        topicFields[paramName].GetStringValue();
    }

    public virtual void OnSimulationStateChanged(SimulationState value)
    {
       
    }
}
