using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public abstract class TopicFieldController : MonoBehaviour
{
    public Action OnTopicFieldValueChanged;
    public bool IsInvokeOnValueChanged;
    public FieldType FieldType => topicField.FieldType;
    public ParamName ParamName => topicField.ParamName;
    [SerializeField] TextMeshProUGUI label;
    protected TopicField topicField;
    protected virtual void Start()
    {
        
    }

    private void SetLabel(string newText)
    {
        label.text = newText;
    }

    //public virtual void Setup(TopicField topicField, object MaxValue = null, string defaultValue = "enter property")
    //{
    //    this.topicField = topicField;
    //    Debug.Log(" Setup  " + GetType().ToString());
    //    SetReadOnly(topicField.IsReadOnly);
    //    SetDefaultValue();
    //    SetLabel(topicField.ParamName.ToString());
    //}
    public virtual void Setup(TopicField topicField, string defaultValue = "enter property")
    {
        this.topicField = topicField;
        Debug.Log(" Setup  " + GetType().ToString());
        SetReadOnly(topicField.IsReadOnly);
        SetLabel(topicField.ParamName.ToString());
    }

    public virtual bool SetValue(object newValue)
    {
        //if (maxValues.ContainsKey(ParamName))
        //{
        //    if ((float)maxValues[ParamName] < (float)newValue)
        //    {
        //        newValue = maxValues[ParamName];
        //    }
        //}
        //return topicField.TrySetValue(newValue);
        return true;
    }

    protected abstract void SetReadOnly(bool value);

    protected abstract string GetText();
}
