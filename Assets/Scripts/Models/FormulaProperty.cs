using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class FormulaProperty
{
    public ParamName ParamName;
    public FieldType FieldType;
    public string Value { get; set; }
    public FormulaProperty(ParamName paramName, FieldType fieldType, string defaultValue = "null")
    {
        ParamName = paramName;
        FieldType = fieldType;
        Value = defaultValue;
    }
    public string SetValue(object obj)
    {
        string valueText = obj switch
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
    public override string ToString()
    {
        return Value;
    }
    public object GetValue(out bool result)
    {
        switch (FieldType)
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
}