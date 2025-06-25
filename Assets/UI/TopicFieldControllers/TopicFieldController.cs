using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public abstract class TopicFieldController : MonoBehaviour
{
    public Action<string, TopicFieldController> UserChangeTopicFieldValue;
    public Action<string> TopicFieldEndEdited;
    public bool IsInvokeOnValueChanged;
    [SerializeField] TextMeshProUGUI label;
    public bool IsReadOnly;
    public ParamName ParamName;
    protected virtual void Start()
    {
        
    }
    private void SetLabel(string newText)
    {
        label.text = newText;
    }

    public void Setup(bool isReadOnly, ParamName paramName, string defaultValue = "enter property")
    {
        this.IsReadOnly = isReadOnly;
        this.ParamName = paramName;
        Debug.Log(" Setup  " + GetType().ToString() + paramName.ToString());
        SetReadOnly(isReadOnly);
        SetLabel(paramName.ToString());
    }

    public abstract bool SetValue(object newValue);

    protected abstract void SetReadOnly(bool value);

    protected abstract string GetText();
}
