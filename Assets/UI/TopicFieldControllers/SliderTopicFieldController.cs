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
        ClampSlider(topicField);
        Slider.wholeNumbers = topicField.FieldType == FieldType.Int;
    }

    private void ClampSlider(TopicField topicField)
    {
        if (topicField.MaxValue == null || topicField.MinValue == null)
        {
            Slider.maxValue = 0.99f;
            Slider.minValue = -0.99f;
            return;
        }
        if (FieldType == FieldType.Float)
        {
            Slider.maxValue = (float)topicField.MaxValue;
            Slider.minValue = (float)topicField.MinValue;
        }
        else if (FieldType == FieldType.Int)
        {
            Slider.maxValue = (int)topicField.MaxValue;
            Slider.minValue = (int)topicField.MinValue;
        }
    }

    private void OnSliderValueChanged()
    {
        if(topicField.TrySetValue(Slider.value,true))
        {
            SetText(Slider.value.ToString());
        }
        else
        {
            SetText("error");
        }
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
