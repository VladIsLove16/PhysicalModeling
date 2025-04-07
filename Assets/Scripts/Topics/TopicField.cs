using System;
using UniRx;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class TopicField
{
    [SerializeField] private ParamName label;
    [SerializeField] private FieldType type;
    [SerializeField] private bool isReadOnly;
    private string value;
    private object Value
    {
        get { return value; }
    }
    public ParamName ParamName => label;
    public FieldType Type => type;
    public bool IsReadOnly => isReadOnly;

    public TopicField(ParamName label, FieldType type, bool isReadOnly = false)
    {
        this.label = label;
        this.type = type;
        this.isReadOnly = isReadOnly;
    }
    public bool SetValue<T>(T obj)
    {
        switch (obj)
        {
            float floatValue => floatValue.ToString(),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x},{v.y},{v.z}",
            string stringValue => stringValue,
            _ => Value
        };
        Value = valueText;
        return valueText;
    }
    private object GetValue(out bool result)
    {
        switch (type)
        {
            case FieldType.Float:
                result = float.TryParse(Value, out float floatValue);
                return floatValue;
            case FieldType.Int:
                result = int.TryParse(Value, out int intValue);
                return intValue;
            case FieldType.Vector3:
                string[] values = Value.Split(',');
                if (values.Length == 3 &&
                    float.TryParse(values[0], out float x) &&
                    float.TryParse(values[1], out float y) &&
                    float.TryParse(values[2], out float z))
                {
                    result = true;
                    return new Vector3(x, y, z);
                }
                result = false;
                return Vector3.zero;
            default:
                result = false;
                return null;
        }
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
            FieldType.Float => typeof(float),

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
    time,
    position
}
public enum FieldType { Float, Vector3, Int }
