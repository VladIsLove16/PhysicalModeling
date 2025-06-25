using System;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class InputFieldTopicFieldController : TopicFieldController
{
    [SerializeField] private TMP_InputField inputField;
    private string previousValue;
    private bool isLocalUpdate;
    protected override void Start()
    {
        base.Start();
        
    }

    public void Setup(bool isReadOnly, ParamName paramName, FieldType fieldType,  string defaultValue = "enter property")
    {
        base.Setup(isReadOnly, paramName);
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
        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener(_ => On_TopicFieldEndEdit());
        inputField.onValueChanged.AddListener((str) => OnValueChanged());
        inputField.onSelect.AddListener(OnSelect);
        SetText(defaultValue);
    }

    protected virtual void On_TopicFieldEndEdit()
    {
        TopicFieldEndEdited?.Invoke(GetText());
    }

    protected override  string GetText()
    {
        return inputField.text;
    }

    public void SetText(string v)
    {
        isLocalUpdate = true;
        inputField.text = v;
    }

    protected override void SetReadOnly(bool value)
    {
        inputField.interactable = !value;
    }

    protected void OnSelect(string arg0)
    {
        previousValue = arg0;
    }
    protected string GetStringFromValue(object obj)
    {
        if (obj == null)
        {
            Debug.Log("why is null " + obj);
            return "null value";
        }
        string valueText = obj switch
        {
            float floatValue => floatValue.ToString("0.00"),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x.ToString("0.00")};{v.y.ToString("0.00")};{v.z.ToString("0.0000")}",
            string stringValue => stringValue,
            bool boolValue => boolValue == true ? true.ToString() : false.ToString(),
            _ => obj.ToString()
        };
        return valueText;
    }

    public override bool SetValue(object newValue)
    {
        isLocalUpdate = true;
        SetText(GetStringFromValue(newValue));
        return true;
    }

    protected void OnValueChanged()
    {
        Debug.Log("value changed");
        if(isLocalUpdate)
        {
            isLocalUpdate = false;
            Debug.Log("value changed locally");
            return;
        }
        UserChangeTopicFieldValue?.Invoke(GetText(),this);
    }
}