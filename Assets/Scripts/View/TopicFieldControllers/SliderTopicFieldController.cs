using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SliderTopicFieldController : InputFieldTopicFieldController
{
    [SerializeField] Slider Slider;
    protected override void Start()
    {
        base.Start();
    }
    public override void Setup(TopicField topicField, string defaultValue = "enter property")
    {
        base.Setup(topicField, defaultValue);
        Slider.onValueChanged.AddListener((str) => OnSliderValueChanged());
        Slider.maxValue = topicField.maxValue;
        Slider.wholeNumbers = topicField.FieldType == FieldType.Int;
    }

    private void OnSliderValueChanged()
    {
        if(topicField.TrySetValue(Slider.value))
        {
            SetText(Slider.value.ToString());
        }
        else
        {
            SetText("error");
        }
    }

    protected override void SetDefaultValue()
    {
        base.SetDefaultValue();
        Slider.value = 0.3f;
    }

    protected override void SetReadOnly(bool value)
    {
        base.SetReadOnly(value);
        Slider.interactable = !value;
    }

    public override bool SetValue(object newValue)
    {
        base.SetValue(newValue);
        if (newValue is float)
        {
            Slider.value = (float)newValue;
            return true;
        }
        else if(newValue is int)
        {
            Slider.value = (int)newValue;
            return true;
        }
        return false;
    }
}
