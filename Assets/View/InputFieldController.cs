using System;
using TMPro;
using UniRx;
using UnityEngine;

public class InputFieldController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public TMP_InputField inputField;
    public ReactiveProperty<object> valueChanged;
    private FieldType fieldType;
    private void Awake()
    {
        inputField.onValueChanged.AddListener((x) => valueChanged.Value = x);
    }
    public void Setup(ParamName ParametrName, FieldType type)
    {
        Debug.Log("field type: " + type);
        label.text = ParametrName.ToString();
        fieldType = type;

        switch (fieldType)
        {
            case FieldType.Float:
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                break;
            case FieldType.Int:
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                break;
            case FieldType.Vector3:
                inputField.contentType = TMP_InputField.ContentType.Standard;
                inputField.text = "0,0,0";
                break;
        }
    }

    public object GetValue()
    {
        switch (fieldType)
        {
            case FieldType.Float:
                return float.TryParse(inputField.text, out float floatValue) ? floatValue : 0f;
            case FieldType.Int:
                return int.TryParse(inputField.text, out int intValue) ? intValue : 0;
            case FieldType.Vector3:
                string[] values = inputField.text.Split(',');
                if (values.Length == 3 &&
                    float.TryParse(values[0], out float x) &&
                    float.TryParse(values[1], out float y) &&
                    float.TryParse(values[2], out float z))
                {
                    return new Vector3(x, y, z);
                }
                return Vector3.zero;
            default:
                return null;
        }
    }
}
