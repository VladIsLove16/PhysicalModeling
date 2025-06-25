using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SliderTopicFieldController : InputFieldTopicFieldController
{
    [SerializeField] Slider Slider;
    private bool isLocalSliderUpdate;
    protected override void Start()
    {
        base.Start();
    }
    public void Setup(bool isReadOnly, ParamName paramName, FieldType fieldType,object minValue, object maxValue, string defaultValue = "enter property")
    {
        Debug.Log("Slider setup"); 
        base.Setup(isReadOnly, paramName,fieldType, defaultValue);
        Slider.onValueChanged.AddListener((str) => OnSliderValueChanged());
        Slider.wholeNumbers = fieldType != FieldType.Float;
        if (maxValue != null && minValue != null)
        {
            if (fieldType == FieldType.Float)
            {
                ClampSlider(fieldType, (float)minValue, (float)maxValue);
            }
            else
                ClampSlider(fieldType, (int)minValue, (int)maxValue);
        }
    }
    private void ClampSlider(FieldType fieldType, float min, float max)
    {
            Slider.maxValue = (float)max;
            Slider.minValue = (float)min;
    }
    private void ClampSlider(FieldType fieldType, int min,int max)
    {
        Slider.maxValue = (int)max;
        Slider.minValue = (int)min;
    }

    private void OnSliderValueChanged()
    {
        Debug.Log("OnSliderValueChanged");
        SetText(Slider.value.ToString());
        if(isLocalSliderUpdate)
        {
            isLocalSliderUpdate = false;
            return;
        }
        Debug.Log("UserChangeTopicFieldValue OnSliderValueChanged");
        UserChangeTopicFieldValue?.Invoke(Slider.value.ToString(),this);
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
        if (newValue is float)
        {
            isLocalSliderUpdate = true;
            Slider.value = (float)newValue;
            return true;
        }
        else if(newValue is int)
        {
            isLocalSliderUpdate = true;
            Slider.value = (int)newValue;
            return true;
        }
        return false;
    }
}
