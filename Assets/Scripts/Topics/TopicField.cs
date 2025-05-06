using System;
using UniRx;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class TopicField
{
    [SerializeField] private ParamName label;
    [SerializeField, NonSerialized] private FieldType type;
    [SerializeField, NonSerialized] private bool isReadOnly;
    [SerializeField,ReadOnly] private string stringValue;
    private ReactiveProperty<object> property = new ReactiveProperty<object>();
    public ViewType viewType = ViewType.inputField;//относится ко виду, а не модели и портит MVVM паттерн, но эту лучше, чем делать новый класс для создания соотвествия переменной и вида отображения(не требуется)
    public ParamName ParamName => label;
    public FieldType Type => type;
    public bool IsReadOnly => isReadOnly;
    public object Value => property.Value;
    public TopicField() { }


    public TopicField(ParamName label, FieldType type, bool isReadonly = false, ViewType viewType = ViewType.inputField)
    {
        this.label = label;
        this.type = type;
        this.isReadOnly = isReadonly;
        property.Subscribe(_ => OnPropertyChanged());
        this.viewType = viewType;
    }

    private void OnPropertyChanged()
    {
        stringValue = GetStringValue();
    }

    public ReactiveProperty<object> Property => property;
    public static Type GetFieldValueType(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.Float =>typeof( float),
            FieldType.Int => typeof(int),
            FieldType.Vector3 => typeof(Vector3),
            FieldType.Bool => typeof(bool),
            _ => typeof(float)
        };
    }
    private string GetStringValue()
    {
        var obj = property.Value;
        if(obj == null)
            return "null value";
        string valueText = obj switch
        {
            float floatValue => floatValue.ToString("0.00"),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x.ToString("0.00")};{v.y.ToString("0.00")};{v.z.ToString("0.0000")}",
            string stringValue => stringValue,
            bool boolValue => boolValue == true? "true" : "false",
            _ => obj.ToString()
        };
        return valueText;
    }
    public void SetValue(object value)
    {
        property.SetValueAndForceNotify(value);
    }
}
public enum ParamName
{
    velocity,
    velocityMagnitude,
    distance,
    pathTraveled,
    time,
    position,
    acceleration,
    jerk,
    angularVelocity,
    angle,
    angleRad,
    angleRadTraveled,
    period,
    radius,
    rotationFrequency,
    rotationFrequencyAcceleration,
    rotationFrequencyJerk,
    numberOfRevolutions,
    step,
    deltaPosition,
    deltaPathTraveled,
    accelerationStartTime,
    flightTime,
    landingVelocity,
    range,
    averageSpeed,
    mass,
    mass2,
    obstaclesMass,
    velocity2,
    pointAReached,
    seed
}
public enum FieldType { Float, Vector3, Int,Bool }
