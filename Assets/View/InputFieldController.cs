using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TMP_InputField inputField;
    private ReactiveProperty<object> viewModelProperty;
    private FieldType fieldType;
    private ParamName parametrName;
    private void Start()
    {
        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener(_=>OnInputFieldEndEdit());
    }
    public void Setup(ParamName ParametrName, FieldType type)
    {
        Debug.Log("field type: " + type);
        parametrName = ParametrName;
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
                break;
        }
    }
    internal void BindProperty(ReactiveProperty<object> property)
    {
        viewModelProperty = property;
        property.Subscribe(value => OnPropertyChanged(value));
        inputField.onValueChanged.AddListener(value => OnTextChanged(value));
        SetText(GetString(property.Value));
    }
    public object GetValue(out bool result)
    {
        switch (fieldType)
        {
            case FieldType.Float:
                result = float.TryParse(inputField.text, out float floatValue);
                return floatValue;
            case FieldType.Int:
                result = int.TryParse(inputField.text, out int intValue);
                return intValue;
            case FieldType.Vector3:
                string[] values = inputField.text.Split(',');
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

    public void SetReadOnly(bool value)
    {
        inputField.interactable = !value;
    }

    private string GetString(ReactiveProperty<object> reactiveProperty)
    {
        return GetString(reactiveProperty.Value);
    }
    private string GetString(object obj)
    {
        string valueText = obj switch
        {
            float floatValue => floatValue.ToString(),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x},{v.y},{v.z}",
            string stringValue => stringValue,
            ReactiveProperty<object> property => GetString(property.Value),
            _ => obj.ToString()
        };
        return valueText;
    }
    internal void SetText(string v)
    {
        inputField.text = v;
    }

    private void OnTextChanged(string newValue)
    {
        if (parametrName == ParamName.time)
            return;

        bool result = false;
        object value = GetValue(out result);
        if (result)
        {
            viewModelProperty.Value = value; // без ForceNotify
        }
    }
    public void OnPropertyChanged(object newValue)
    {
        if (inputField.isFocused)
            return;
        SetText(GetString(newValue));
    }

    private void OnInputFieldEndEdit()
    {
        var value = GetValue(out bool result);
        if (result)
        {
            Debug.Log(parametrName + " setted " + value);
            viewModelProperty.SetValueAndForceNotify(value);
        }
        else
            SetText(GetString(viewModelProperty));

    }

    
}
