using System;
using TMPro;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class InputFieldTopicFieldController : TopicFieldController
{
    public Action OnTopicFieldEndEdited;
    [SerializeField] private TMP_InputField inputField;
    private string previousValue;
    protected override void Start()
    {
        base.Start();
        
    }

    public override void Setup(TopicField topicField, string defaultValue = "enter property")
    {
        base.Setup(topicField, defaultValue);
        switch (FieldType)
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
        inputField.onEndEdit.AddListener(_ => OnTopicFieldEndEdit());
        inputField.onValueChanged.AddListener((str) => OnValueChanged());
        inputField.onSelect.AddListener(OnSelect);
        Debug.Log("Setup  :  InputFieldTopicFieldController");
    }
    protected virtual void OnTopicFieldEndEdit()
    {
        bool res = topicField.TrySetValue(GetText());
        if (!res)
        {
            SetText(previousValue);
        }
    }
    protected override  string GetText()
    {
        return inputField.text;
    }
    public void SetText(string v)
    {
        inputField.text = v;
    }
    protected override void SetReadOnly(bool value)
    {
        inputField.interactable = !value;
    }
    protected void OnSelect(string arg0)
    {
        Debug.Log("inputfieldSelected");
        previousValue = arg0;
    }

    protected override void SetDefaultValue()
    {
        SetValue(DefaultValues[FieldType]);
    }

    public override bool SetValue(object newValue)
    {
        SetText(topicField.GetStringFromValue(newValue));
        return true;
    }

    protected void OnValueChanged()
    {
        Debug.Log("new value " + inputField.text);
        topicField.TrySetValue(GetText());
    }
}