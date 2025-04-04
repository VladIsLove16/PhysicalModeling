using System;
using System.Linq;
using UniRx;
using UnityEngine;

public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    public ReactiveDictionary<ParamName, ReactiveProperty<object>> Parameters { get; } = new();
    public ReactiveDictionary<FieldType, object> DefaultValues = new ReactiveDictionary<FieldType, object>()
    {
        { FieldType.Float, 0f },
        { FieldType.Vector3, Vector3.forward },
        { FieldType.Int, 0 },
    };
    [SerializeField] public TopicFields TopicFields;
    public virtual void InitializeParameters()
    {
        foreach (var field in TopicFields.Fields)
        {
            Parameters[field.ParamName] = CreateReactiveProperty(field.Type);
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


    public abstract Vector3 CalculatePosition(float deltaTime);

    public void ResetParams()
    {
        foreach (var pair in Parameters)
        {
            ResetParam(pair.Key);

        }
    }

    public FieldType GetFieldType(object value)
    {
        return TopicField.GetFieldType(value);
    }

    public void ResetParam(ParamName parametrName)
    {
        Parameters[parametrName].Value = DefaultValues[TopicFields.Fields.First(x => x.ParamName == parametrName).Type];
    }
}