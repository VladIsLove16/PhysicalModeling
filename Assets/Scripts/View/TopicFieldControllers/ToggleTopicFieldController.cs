using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ToggleTopicFieldController : TopicFieldController
{
    [SerializeField] Toggle Toggle;
    protected override void Start()
    {
        base.Start();
    }
    public override void Setup(TopicField topicField, string defaultValue = "enter property")
    {
        base.Setup(topicField, defaultValue);
        Toggle.onValueChanged.AddListener((boolean) => OnValueChanged());
    }
    protected override string GetText()
    {
        return Toggle.isOn ? true.ToString() : false.ToString();
    }

    protected override void SetDefaultValue()
    {
        Toggle.isOn = false;
    }

    protected override void SetReadOnly(bool value)
    {
        Toggle.interactable = !value;
    }

    public override bool SetValue(object newValue)
    {
        if (newValue is bool)
        {
            Toggle.isOn = (bool)newValue;
            return true;
        }
        return false;
    }

    protected void OnValueChanged()
    {
        topicField.TrySetValue(Toggle.isOn);
    }
}
