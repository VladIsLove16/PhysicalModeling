using System;
using UniRx;
using UnityEngine;

[Serializable]
public class TopicField
{
    [SerializeField] private ParamName label;
    [SerializeField] private FieldType type;
    [SerializeField] private bool isReadOnly;
    public ParamName ParamName => label;
    public FieldType Type => type;
    public bool IsReadOnly => isReadOnly;
    public TopicField() { }

    public TopicField(ParamName label, FieldType type, bool isReadonly = false)
    {
        this.label = label;
        this.type = type;
        this.isReadOnly = isReadonly;
    }
    public static Type GetFieldValueType(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.Float =>typeof( float),

            FieldType.Int => typeof(int),
            FieldType.Vector3 => typeof(Vector3),
            _ => typeof(float)
        };
    }
}
public enum ParamName
{
    velocity,
    distance,
    pathTraveled,
    time,
    position,
    acceleration,
    angularVelocity,
    angularAcceleration,
    angleRadTraveled,
    angleRad,
    period,
    radius,
    rotationFrequency,
    numberOfRevolutions

}
public enum FieldType { Float, Vector3, Int }
