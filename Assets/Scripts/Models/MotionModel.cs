using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Android.Gradle;
using UnityEngine;

public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    public Dictionary<ParamName, FormulaProperty> Parameters { get; private set; } = new();
    public Dictionary<FieldType, object> DefaultValues = new()
    {
        { FieldType.Float, 0f },
        { FieldType.Vector3, "0,0,0" },
        { FieldType.Int, 0 },
    };

    [SerializeField] public TopicFields TopicFields;

    public virtual void InitializeParameters()
    {
        Parameters.Clear();
        foreach (var field in TopicFields.Fields)
        {
            Parameters[field.ParamName] = CreateFormulaProperty(field.Type, field.ParamName);
        }
    }

    private FormulaProperty CreateFormulaProperty(FieldType fieldType, ParamName paramName)
    {
        return new FormulaProperty(paramName, fieldType, DefaultValues[fieldType].ToString());
        //switch (fieldType)
        //{ 
        //     case FieldType.Float:
        //     return new FormulaProperty(paramName, fieldType,DefaultValues[fieldType].ToString());
        //case FieldType.Vector3:
        //    return new ReactiveProperty<object>(DefaultValues[FieldType.Vector3]);
        //case FieldType.Int:
        //    return new ReactiveProperty<object>(DefaultValues[FieldType.Int]);
        //default:
        //    return new ReactiveProperty<object>(DefaultValues[FieldType.Float]);
        //}
    }
    private object GetDefaultValue(FieldType fieldType)
    {
        return DefaultValues.TryGetValue(fieldType, out var defaultValue)
            ? defaultValue
            : 0f;
    }

    public void SetParameter(ParamName paramName, object value)
    {
        if (Parameters.ContainsKey(paramName))
        {
            Debug.Log(paramName + " " + value);
            Parameters[paramName].SetValue(value);
        }
    }

    public T GetParameter<T>(ParamName paramName)
    {
        if (Parameters.TryGetValue(paramName, out var value) && value.Value is T casted)
        {
            Debug.Log("casted " + value);
            return casted;
        }
        return default;
    }

    public void ResetParams()
    {
        foreach (var field in TopicFields.Fields)
        {
            ResetParam(field.ParamName);
        }
    }
    public void ResetParam(ParamName paramName)
    {
        FieldType fieldType =  TopicFields.Fields.First(x => x.ParamName == paramName).Type;
        Parameters[paramName].SetValue(GetDefaultValue(fieldType));
    }

    public FieldType GetFieldType(object value)
    {
        return TopicField.GetFieldType(value);
    }

    public FieldType GetFieldType(ParamName paramName)
    {
        var field = TopicFields.Fields.FirstOrDefault(f => f.ParamName == paramName);
        return field.Type;
    }

    public abstract Vector3 UpdatePosition(float deltaTime);
    public abstract Vector3 CalculatePosition(float time);
}
