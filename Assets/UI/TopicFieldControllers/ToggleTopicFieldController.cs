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
    public new void Setup(bool isReadOnly, ParamName paramName, string defaultValue = "enter property")
    {
        base.Setup(isReadOnly, paramName, defaultValue);
        Toggle.onValueChanged.AddListener((boolean) => OnValueChanged());
    }

    protected override string GetText()
    {
        return Toggle.isOn ? true.ToString() : false.ToString();
    }

    protected override void SetReadOnly(bool value)
    {
        Toggle.interactable = !value;
    }

    public override bool SetValue(object newValue)
    {
        if (newValue is bool)
        {
            SuppressUserChangeNotification(() => Toggle.isOn = (bool)newValue);
            return true;
        }
        return false;
    }

    protected void OnValueChanged()
    {
        RaiseUserValueChanged(GetText());
    }
}
