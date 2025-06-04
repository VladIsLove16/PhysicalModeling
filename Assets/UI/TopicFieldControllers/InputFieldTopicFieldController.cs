using System;
using TMPro;
using UnityEngine;

public class InputFieldTopicFieldController : TopicFieldController
{
    public Action OnTopicFieldEndEdited;
    [SerializeField] private TMP_InputField inputField;
    private string previousValue;
    private bool isLocalUpdate;
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
        Debug.Log("Setup  :  InputFieldTopicFieldController " +  topicField.ParamName +  "  with value " + topicField.Value);
        if(topicField.Value==null)
        {
            Debug.Log("topicField.Value==null" + topicField.ParamName);
            return;
        }
        SetText(topicField.GetStringFromValue(topicField.Value));
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

    public override bool SetValue(object newValue)
    {
        SetText(topicField.GetStringFromValue(newValue));
        return true;
    }

    protected void OnValueChanged()
    {
        if(isLocalUpdate)
        {
            isLocalUpdate = false;
            return;
        }
        topicField.TrySetValue(GetText());
    }
}