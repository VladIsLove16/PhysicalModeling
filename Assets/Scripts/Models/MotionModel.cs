using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    protected ReactiveDictionary<ParamName, ReactiveProperty<object>> parameters = new();
    public ReactiveDictionary<ParamName, ReactiveProperty<object>> Parameters => parameters;

    private ReactiveDictionary<FieldType, object> DefaultValues = new ReactiveDictionary<FieldType, object>()
    {
        { FieldType.Float, 0f },
        { FieldType.Vector3, Vector3.zero },
        { FieldType.Int, 0 },
    };
    [SerializeField] public TopicFields TopicFields;
    public virtual void InitializeParameters()
    {
        foreach (var field in TopicFields.Fields)
        {
            parameters[field.ParamName] = CreateReactiveProperty(field.Type);
        }
    }

    private ReactiveProperty<object> CreateReactiveProperty(FieldType fieldType)
    {
        switch (fieldType)
        {
            case FieldType.Float:
                return new ReactiveProperty<object>(DefaultValues[FieldType.Float]);
            case FieldType.Vector3:
                return new ReactiveProperty<object>(DefaultValues[FieldType.Vector3]);
            case FieldType.Int:
                return new ReactiveProperty<object>(DefaultValues[FieldType.Int]);
            default:
                return new ReactiveProperty<object>(DefaultValues[FieldType.Float]);
        }
    }


    public abstract Vector3 UpdatePosition(float deltaTime);
    public abstract Vector3 CalculatePosition(float Time);

    public void ResetParams()
    {
        foreach (var pair in parameters)
        {
            ResetParam(pair.Key);

        }
    }
    public FieldType GetFieldType(ParamName value)
    {
        return TopicFields.GetFieldType(value);
    }

    
    public object GetDefaultValue(FieldType fieldType)
    {
        return DefaultValues[fieldType];
    }
    public object GetDefaultValue(ParamName paramName)
    {
        return DefaultValues[TopicFields.Fields.First(x => x.ParamName == paramName).Type];
    }
    public void ResetParam(ParamName parametrName)
    {
        parameters[parametrName].Value = GetDefaultValue(parametrName);
    }
    public object GetParam(ParamName paramName)
    {
        return parameters[paramName].Value;
    }
    public void SetParam(ParamName paramName, object value)
    {
        parameters[paramName].SetValueAndForceNotify(value);
    }

}