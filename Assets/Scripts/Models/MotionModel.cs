using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
public abstract class MotionModel : ScriptableObject, IMovementStrategy
{
    [SerializeField] public string Title;
    //идея фикс: поставить реактивное свойство в TopicField, а TopicField в модель. Тогда методы получения значения(разных типов) можно переопределить в самом классе TopicField а не здесь, в модели.
    //вторая идея: добавить метод, который будет пересчитывать другие параметры на основе измененного пользователем, как со временем. 
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
        parameters.Clear();
        foreach (var field in TopicFields.Fields)
        {
            parameters[field.ParamName] = CreateReactiveProperty(field.Type);
        }
    }

    protected ReactiveProperty<object> CreateReactiveProperty(FieldType fieldType)
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
    public abstract List<TopicField> GetRequiredParams();

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