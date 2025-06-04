using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System;
using UniRx;

[CreateAssetMenu(fileName = "RampMotionModel", menuName = "MotionModelsDropdown/RampMotionModel")]
public class RampMotionModel : MotionModel
{
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 30f },
        { ParamName.friction, 1f },
    }; ;
        }
    }
    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 60f },
        { ParamName.friction, 1f },
    }; ;
        }
    }
    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 0f },
        { ParamName.friction, 0f },
    }; ;
        }
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        //var property = topicFields[ParamName.additionalMass];
        //property.Property.Subscribe(_ =>
        //{
        //});
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 pos =(Vector3) GetParam(ParamName.position);
        float force =  (float)GetParam(ParamName.force) ;
        (Vector3 moveVector, Vector3 newVelocity)   = RampPhysics.CheckInclined(
            (float)GetParam(ParamName.mass),
            (float)GetParam(ParamName.friction),
            (float)GetParam(ParamName.angleDeg),
            (Vector3)GetParam(ParamName.velocity),
            force,
            deltaTime);
        float newForce = force + (float)GetParam(ParamName.forceAcceleration) * deltaTime;
        TrySetParam(ParamName.force, newForce);
        Debug.Log("new force " + newForce);
        Debug.Log("moveVector.magnitude " + moveVector.magnitude);
        Vector3 newPos = pos + moveVector;
        TrySetParam(ParamName.position, newPos);
        TrySetParam(ParamName.velocity, newVelocity);
        TrySetParam(ParamName.time, (float)GetParam(ParamName.time) + deltaTime);
        TrySetParam(ParamName.isMoving, (bool)!Mathf.Approximately(moveVector.magnitude, 0));
        return pos + moveVector;
    }
    //public override bool TrySetParam(ParamName paramName, object value)
    //{
    //    Debug.Log("paramName " + paramName + " changed ");
    //    if (paramName == ParamName.additionalMass)
    //    {
    //        Debug.Log("paramName");
            
    //    }
    //    return base.TrySetParam(paramName, value);
    //}
    public override Vector3 CalculatePosition(float Time)
    {
        return Vector3.zero;
    }

    public override List<TopicField> GetRequiredParams()
    {
        List<TopicField> RequiredParams = new List<TopicField>();
            RequiredParams = new List<TopicField>()
            {
               new TopicField(ParamName.position, true),
               new TopicField(ParamName.velocity, true),
               new TopicField(ParamName.isMoving, true),
               new TopicField(ParamName.angleDeg, false),
               new TopicField(ParamName.mass, false),
               new TopicField(ParamName.friction, false),
               new TopicField(ParamName.force, false),
               new TopicField(ParamName.forceAcceleration, false),
               new TopicField(ParamName.time, true)
            };
        return RequiredParams;
    }
    private void OnadditionalMassParamChange()
    {

    }
    public override void ResetParam(ParamName paramName)
    {
        if (paramName == ParamName.angleDeg)
            return;
        if (paramName == ParamName.mass)
            return;
        if (paramName == ParamName.friction)
            return;
        if (paramName == ParamName.force)
            return;
        if (paramName == ParamName.forceAcceleration)
            return;
        base.ResetParam(paramName);
    }
}
public static class RampPhysics
{
    private const float g = 9.81f;

    public static (Vector3 moveVector, Vector3 newSpeed) CheckInclined(
    float mass,
    float frictionCoeff,
    float angleDeg,
    Vector3 currentSpeed,
    float appliedForce,
    float deltaTime)
    {
        float angleRadians = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRadians);
        float sin = Mathf.Sin(angleRadians);

        // Вектор наклонной плоскости: вниз по наклону, например, по диагонали X-Y
        // Например: наклон направлен вниз по X и Y (горка вправо-вниз)
        // Направление наклонной (2D вдоль XY, вниз по наклону)
        Vector3 inclineDir = new Vector3(Mathf.Cos(angleRadians), -Mathf.Sin(angleRadians), 0f).normalized;

        // Гравитационная сила вдоль наклонной
        float gravityForce = mass * g * sin;

        // Нормальная сила (перпендикуляр к наклонной)
        float normalForce = mass * g * cos;

        // Максимальная сила трения
        float maxFriction = frictionCoeff * normalForce;


        // Скорость вдоль наклонной
        float speedAlongIncline = Vector3.Dot(currentSpeed, inclineDir);

        // Сила, вызывающая движение
        float drivingForce = appliedForce + gravityForce;

        // Вычисляем силу трения
        float frictionForce;
        if (!Mathf.Approximately(speedAlongIncline, 0f))
        {
            // Кинетическое трение
            frictionForce = -Mathf.Sign(speedAlongIncline) * maxFriction;
        }
        else if (Mathf.Abs(drivingForce) > maxFriction)
        {
            // Преодолено статическое трение — начинается движение
            frictionForce = -Mathf.Sign(drivingForce) * maxFriction;
        }
        else
        {
            // Статическое трение полностью уравновешивает
            frictionForce = -drivingForce;
        }

        // Суммарная сила вдоль наклонной
        float netForce = drivingForce + frictionForce;

        // Ускорение вдоль наклонной
        float acceleration = netForce / mass;

        // Обновлённая скорость вдоль наклонной
        float newSpeedAlongIncline = speedAlongIncline + acceleration * deltaTime;

        // Проверка на разворот: если изменила знак → останавливаем
        if (Mathf.Sign(speedAlongIncline) != Mathf.Sign(newSpeedAlongIncline) &&
            !Mathf.Approximately(speedAlongIncline, 0f))
        {
            newSpeedAlongIncline = 0f;
        }

        // Средняя скорость
        float averageSpeed = 0.5f * (speedAlongIncline + newSpeedAlongIncline);
        float displacement = averageSpeed * deltaTime;

        Vector3 moveVector = inclineDir * displacement;
        Vector3 newSpeed = inclineDir * newSpeedAlongIncline;

        Debug.Log($"[Inclined3D] Applied: {appliedForce:F2}, Gravity: {gravityForce:F2}, Friction: {frictionForce:F2}, NetF: {netForce:F2}, Accel: {acceleration:F2}");

        return (moveVector, newSpeed);

    }
    internal static float GetForceForMass2(object mass2)
    {
        float mass = (float)mass2;
        return mass * g;
    }

}
