using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    [SerializeField] public string Title;
    //вторая идея: добавить метод, который будет пересчитывать другие параметры на основе измененного пользователем, как со временем. 
    protected Dictionary<ParamName, TopicField> topicFields = new();
    protected Dictionary<ParamName, TopicField> TopicFields
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

    private Dictionary<ParamName, ReactiveProperty<object>> paramValues;
    public Dictionary<ParamName, ReactiveProperty<object>> Params
    {
        get
        {
            if (paramValues == null || paramValues.Count == 0)
                paramValues = TopicFields.ToDictionary(
            x => x.Key,
            x => x.Value.Property);
            return paramValues;
        }
    }
    private bool isInitialized;
    private ReactiveDictionary<FieldType, object> DefaultValues = new ReactiveDictionary<FieldType, object>()
    {
        { FieldType.Float, 0f },
        { FieldType.Vector3, Vector3.zero },
        { FieldType.Int, 0 },
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
            field.SetValue(DefaultValues[field.Type]);
        }
        isInitialized= true;
    }


    public abstract Vector3 UpdatePosition(float deltaTime);
    public abstract Vector3 CalculatePosition(float Time);
    public abstract List<TopicField> GetRequiredParams();

    public void ResetParams()
    {
        foreach (var pair in topicFields)
        {
            ResetParam(pair.Key);

        }
    }
    public FieldType GetFieldType(ParamName value)
    {
        return topicFields[value].Type;
    }

    
    public object GetDefaultValue(FieldType fieldType)
    {
        return DefaultValues[fieldType];
    }
    public object GetDefaultValue(ParamName paramName)
    {
        return DefaultValues[topicFields[paramName].Type];
    }
    public void ResetParam(ParamName parametrName)
    {
        paramValues[parametrName].Value = GetDefaultValue(parametrName);
    }
    public object GetParam(ParamName paramName)
    {
        return topicFields[paramName].Value;
    }
    public void SetParam(ParamName paramName, object value)
    {
        paramValues[paramName].SetValueAndForceNotify(value);
    }
    public bool IsReadonly(ParamName paramName)
    {
        return topicFields[paramName].IsReadOnly;
    }

}