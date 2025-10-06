using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileMotionModel", menuName = "MotionModelsDropdown/Projectile")]
public class ProjectileMotionModel : MotionModel
{
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
            };
        }
    }

    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
            };
        }
    }

    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
            };
        }
    }
    private readonly Vector3 gravity = new(0, -9.81f, 0);

    public override Vector3 UpdatePosition(float deltaTime)
    {
        float currentTime = (float)GetParam(ParamName.time);
        float newTime = currentTime + deltaTime;
        float accelerationTime = (float)GetParam(ParamName.accelerationStartTime); // момент, когда началось ускорение
        Vector3 acceleration = (Vector3)GetParam(ParamName.acceleration);
        Vector3 currentAcceleration;
        if (currentTime > accelerationTime)
            currentAcceleration = acceleration;
        else
            currentAcceleration = Vector3.zero;

        Vector3 initialVelocity = GetVelocity();
        Vector3 initialPosition = (Vector3)GetParam(ParamName.position);
        Vector3 totalAcceleration = currentAcceleration + gravity;

        Vector3 deltaPosition = initialVelocity * deltaTime + 0.5f * totalAcceleration * deltaTime * deltaTime;
        Vector3 newVelocity = initialVelocity + totalAcceleration * deltaTime;
        Vector3 newPosition = initialPosition + deltaPosition;
        if (newPosition.y < 0)
        {
            newPosition.y = 0;
            newVelocity = Vector3.zero;
            deltaPosition = initialPosition - newPosition;
            newTime = currentTime;
        }
        float pathTraveled = (float)GetParam(ParamName.pathTraveled) + deltaPosition.magnitude;

        TrySetParam(ParamName.time, newTime);
        TrySetParam(ParamName.velocity, newVelocity);
        TrySetParam(ParamName.position, newPosition);
        TrySetParam(ParamName.deltaPosition, deltaPosition);
        TrySetParam(ParamName.velocityMagnitude, newVelocity.magnitude);
        TrySetParam(ParamName.pathTraveled, pathTraveled);
        TrySetParam(ParamName.distance, newPosition.magnitude);

        return newPosition;
    }
    protected virtual Vector3 GetVelocity()
    {
        return (Vector3)GetParam(ParamName.velocity);
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


        TrySetParam(ParamName.time, time,false);
        TrySetParam(ParamName.position, newPosition, false);
        TrySetParam(ParamName.velocity, finalVelocity, false);
        TrySetParam(ParamName.velocityMagnitude, finalVelocity.magnitude, false);
        TrySetParam(ParamName.deltaPosition, deltaPosition, false);
        TrySetParam(ParamName.pathTraveled, path, false);
        TrySetParam(ParamName.distance, newPosition.magnitude, false);

        // Расчеты через вспомогательный класс:
        float flightTime = ProjectilePhysics.CalculateFlightTime(initialPosition, initialVelocity, acceleration);
        float range = ProjectilePhysics.CalculateRange(initialVelocity, flightTime);
        Vector3 landingVelocity = ProjectilePhysics.CalculateFinalVelocity(initialVelocity, acceleration, flightTime);
        float averageSpeed = ProjectilePhysics.CalculateAverageSpeed(path, flightTime);

        TrySetParam(ParamName.flightTime, flightTime, false);
        TrySetParam(ParamName.range, range, false);
        TrySetParam(ParamName.landingVelocity, landingVelocity, false);
        TrySetParam(ParamName.averageSpeed, averageSpeed, false);

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

            // Выходные
            new TopicField(ParamName.time, FieldType.Float, true), // текущее время
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
public class ProjectileMotionModelAngled : ProjectileMotionModel
{
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                {ParamName.angleDeg, 0f }
            };
        }
    }

    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                {ParamName.angleDeg, 89f }
            };
        }
    }

    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>
            {
                {ParamName.angleDeg, 0f }
            };
        }
    }
    private readonly Vector3 gravity = new(0, -9.81f, 0);
    protected override Vector3 GetVelocity()
    {
        float angle =(float) GetParam(ParamName.angleDeg);
        float time = (float) GetParam(ParamName.time);
        Vector3 velocity = (Vector3) GetParam(ParamName.velocity);
        if(time == 0f)
        {
            return new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle),  0);
        }
        else
        {
            angle = Mathf.Atan2(Mathf.Abs(velocity.y), Mathf.Abs(velocity.x)) * Mathf.Rad2Deg;
            TrySetParam(ParamName.angleDeg, angle);
            return velocity;
        }
    }
    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            // Входные
            new TopicField(ParamName.position, FieldType.Vector3, false), // начальная позиция (высота)
            new TopicField(ParamName.angleDeg, FieldType.Float, false), // начальная скорость
            new TopicField(ParamName.acceleration, FieldType.Vector3, false), // ускорение после t
            new TopicField(ParamName.accelerationStartTime, FieldType.Float, false), // время, когда начинается ускорение
            // Выходные
            new TopicField(ParamName.time, FieldType.Float, true), // текущее время
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
        float totalAy = acceleration.y + (-9.81f);

        float a = 0.5f * totalAy;
        float b = velocity.y;
        float c = initialPosition.y;

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return 0f;

        float sqrtD = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtD) / (2 * a);
        float t2 = (-b - sqrtD) / (2 * a);

        float result = Mathf.Max(t1, t2);
        return result > 0f ? result : 0f;
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
