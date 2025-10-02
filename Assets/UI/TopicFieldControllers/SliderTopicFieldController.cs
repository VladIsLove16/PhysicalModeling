using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTopicFieldController : InputFieldTopicFieldController
{
    [SerializeField] private Slider Slider;
    private FieldType sliderFieldType;
    private bool isSynchronizingSliderValue;

    protected override void Start()
    {
        base.Start();
    }

    public void Setup(bool isReadOnly, ParamName paramName, FieldType fieldType, object minValue, object maxValue, string defaultValue = "enter property")
    {
        Debug.Log("Slider setup");
        sliderFieldType = fieldType;
        base.Setup(isReadOnly, paramName, fieldType, defaultValue);
        Slider.onValueChanged.AddListener(_ => OnSliderValueChanged());
        Slider.wholeNumbers = fieldType != FieldType.Float;
        if (maxValue != null && minValue != null)
        {
            if (fieldType == FieldType.Float)
                ClampSlider(fieldType, (float)minValue, (float)maxValue);
            else
                ClampSlider(fieldType, (int)minValue, (int)maxValue);
        }
    }

    private void ClampSlider(FieldType fieldType, float min, float max)
    {
        Slider.maxValue = max;
        Slider.minValue = min;
    }

    private void ClampSlider(FieldType fieldType, int min, int max)
    {
        Slider.maxValue = max;
        Slider.minValue = min;
    }

    private void OnSliderValueChanged()
    {
        if (isSynchronizingSliderValue)
            return;

        Debug.Log("OnSliderValueChanged");

        object valueForText = Slider.value;
        if (sliderFieldType == FieldType.Int)
        {
            int rounded = Mathf.RoundToInt(Slider.value);
            if (!Mathf.Approximately(rounded, Slider.value))
            {
                isSynchronizingSliderValue = true;
                Slider.value = rounded;
                isSynchronizingSliderValue = false;
            }
            valueForText = rounded;
        }

        string valueString = GetStringFromValue(valueForText);
        SetText(valueString);
        RaiseUserValueChanged(valueString);
    }

    protected override void SetReadOnly(bool value)
    {
        base.SetReadOnly(value);
        Slider.interactable = !value;
    }

    public override bool SetValue(object newValue)
    {
        Debug.Log(" slider new value  " + newValue);
        base.SetValue(newValue);
        if (newValue is float floatValue)
        {
            SuppressUserChangeNotification(() => Slider.value = floatValue);
            return true;
        }
        if (newValue is int intValue)
        {
            SuppressUserChangeNotification(() => Slider.value = intValue);
            return true;
        }
        return false;
    }
}

