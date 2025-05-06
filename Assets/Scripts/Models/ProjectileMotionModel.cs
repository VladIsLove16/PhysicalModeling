using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileMotionModel", menuName = "MotionModelsDropdown/Projectile")]
public class ProjectileMotionModel : MotionModel
{
    private readonly Vector3 gravity = new(0, -9.81f, 0);

    public override Vector3 UpdatePosition(float deltaTime)
    {
        float currentTime = (float)GetParam(ParamName.time);
        float accelerationTime = (float)GetParam(ParamName.accelerationStartTime); // момент, когда началось ускорение
        Vector3 acceleration = (Vector3)GetParam(ParamName.acceleration);
        Vector3 currentAcceleration;
        if (currentTime > accelerationTime)
            currentAcceleration = acceleration;
        else
            currentAcceleration = Vector3.zero;


        SetParam(ParamName.time, currentTime + deltaTime);

        Vector3 velocity = (Vector3)GetParam(ParamName.velocity);
        Vector3 position = (Vector3)GetParam(ParamName.position);
        Vector3 totalAcceleration = currentAcceleration + gravity;

        Vector3 deltaPosition = velocity * deltaTime + 0.5f * totalAcceleration * deltaTime * deltaTime;
        Vector3 newVelocity = velocity + totalAcceleration * deltaTime;
        Vector3 newPosition = position + deltaPosition;

        float pathTraveled = (float)GetParam(ParamName.pathTraveled) + deltaPosition.magnitude;

        SetParam(ParamName.velocity, newVelocity);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.deltaPosition, deltaPosition);
        SetParam(ParamName.velocityMagnitude, newVelocity.magnitude);
        SetParam(ParamName.pathTraveled, pathTraveled);
        SetParam(ParamName.distance, newPosition.magnitude);

        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 initialPosition = (Vector3)GetParam(ParamName.position);
        Vector3 initialVelocity = (Vector3)GetParam(ParamName.velocity);
        Vector3 acceleration = (Vector3)GetParam(ParamName.acceleration);
        float accelerationTime = (float)GetParam(ParamName.accelerationStartTime);

        Vector3 totalAcceleration = (time >= accelerationTime) ? acceleration + gravity : gravity;

        Vector3 deltaPosition = initialVelocity * time + 0.5f * totalAcceleration * time * time;
        Vector3 finalVelocity = initialVelocity + totalAcceleration * time;
        Vector3 newPosition = initialPosition + deltaPosition;

        float path = deltaPosition.magnitude;

        SetParam(ParamName.time, time);
        SetParam(ParamName.position, newPosition);
        SetParam(ParamName.velocity, finalVelocity);
        SetParam(ParamName.velocityMagnitude, finalVelocity.magnitude);
        SetParam(ParamName.deltaPosition, deltaPosition);
        SetParam(ParamName.pathTraveled, path);
        SetParam(ParamName.distance, newPosition.magnitude);

        // Расчеты через вспомогательный класс:
        float flightTime = ProjectilePhysics.CalculateFlightTime(initialPosition, initialVelocity, acceleration);
        float range = ProjectilePhysics.CalculateRange(initialVelocity, flightTime);
        Vector3 landingVelocity = ProjectilePhysics.CalculateFinalVelocity(initialVelocity, acceleration, flightTime);
        float averageSpeed = ProjectilePhysics.CalculateAverageSpeed(path, flightTime);

        SetParam(ParamName.flightTime, flightTime);
        SetParam(ParamName.range, range);
        SetParam(ParamName.landingVelocity, landingVelocity);
        SetParam(ParamName.averageSpeed, averageSpeed);

        return newPosition;
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            // Входные
            new TopicField(ParamName.position, FieldType.Vector3, false), // начальная позиция (высота)
            new TopicField(ParamName.velocity, FieldType.Vector3, false), // начальная скорость
            new TopicField(ParamName.acceleration, FieldType.Vector3, false), // ускорение после t
            new TopicField(ParamName.accelerationStartTime, FieldType.Float, false), // время, когда начинается ускорение
            new TopicField(ParamName.time, FieldType.Float, false), // текущее время

            // Выходные
            new TopicField(ParamName.velocityMagnitude, FieldType.Float, true), // модуль скорости в текущий момент
            new TopicField(ParamName.deltaPosition, FieldType.Vector3, true), // изменение позиции за шаг
            new TopicField(ParamName.pathTraveled, FieldType.Float, true), // полный путь
            new TopicField(ParamName.distance, FieldType.Float, true), // расстояние от начала до текущей точки
            new TopicField(ParamName.flightTime, FieldType.Float, true), // полное время полёта
            new TopicField(ParamName.range, FieldType.Float, true), // дальность полета (ось X)
            new TopicField(ParamName.landingVelocity, FieldType.Vector3, true), // скорость при приземлении
            new TopicField(ParamName.averageSpeed, FieldType.Float, true), // средняя скорость полета
        };
    }
}
public static class ProjectilePhysics
{
    public static float CalculateFlightTime(Vector3 initialPosition, Vector3 velocity, Vector3 acceleration)
    {
        // Решаем уравнение по Y: y = y0 + vy * t + 0.5 * ay * t^2 = 0 (когда приземлится)
        float a = 0.5f * (acceleration.y - 9.81f);
        float b = velocity.y;
        float c = initialPosition.y;

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return 0;

        float sqrtD = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtD) / (2 * a);
        float t2 = (-b - sqrtD) / (2 * a);
        return Mathf.Max(t1, t2);
    }

    public static float CalculateRange(Vector3 velocity, float time)
    {
        return velocity.x * time;
    }

    public static Vector3 CalculateFinalVelocity(Vector3 velocity, Vector3 acceleration, float time)
    {
        return velocity + (acceleration + new Vector3(0, -9.81f, 0)) * time;
    }

    public static float CalculateAverageSpeed(float totalPath, float totalTime)
    {
        return totalPath / totalTime;
    }
}
