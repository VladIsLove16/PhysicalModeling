using System;
using UniRx;
using UnityEngine;

[Serializable]
public class TopicField
{
    [SerializeField] private ParamName label;
    [SerializeField] private FieldType type;

    public ParamName ParamName => label;
    public FieldType Type => type;

    public TopicField() { }

    public TopicField(ParamName label, FieldType type)
    {
        this.label = label;
        this.type = type;
    }
    public static FieldType GetFieldType(object value)
    {
        return value switch
        {
            Vector3 => FieldType.Vector3,
            float => FieldType.Float,
            int => FieldType.Int,
            _ => FieldType.Float
        };
    }
    public static Type GetFieldValueType(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.Float =>typeof( float),

            FieldType.Int => typeof(int),
            FieldType.Vector3 => typeof(Vector3),
            _ => typeof(float)
        };
    }
}
public enum ParamName
{
    velocity,
    distance,
    pathTraveled,
    time
}
public enum FieldType { Float, Vector3, Int }
