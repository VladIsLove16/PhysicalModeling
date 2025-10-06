using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class TopicFieldController : MonoBehaviour
{
    public Action<string, TopicFieldController> UserChangeTopicFieldValue;
    public Action<string> TopicFieldEndEdited;
    public bool IsInvokeOnValueChanged;
    [SerializeField] TextMeshProUGUI label;
    public bool IsReadOnly;
    public ParamName ParamName;
    private int userChangeNotificationSuppressionDepth;

    private static readonly Dictionary<ParamName, string> Translations = new Dictionary<ParamName, string>
    {
        { ParamName.velocity, "скорость" },
        { ParamName.velocityMagnitude, "модуль скорости" },
        { ParamName.distance, "расстояние" },
        { ParamName.pathTraveled, "пройденный путь" },
        { ParamName.time, "время" },
        { ParamName.position, "позиция" },
        { ParamName.acceleration, "ускорение" },
        { ParamName.jerk, "рывок" },
        { ParamName.angularVelocity, "угловая скорость" },
        { ParamName.angleDeg, "угол (градусы)" },
        { ParamName.angleRad, "угол (радианы)" },
        { ParamName.angleRadTraveled, "пройденный угол (радианы)" },
        { ParamName.period, "период" },
        { ParamName.radius, "радиус" },
        { ParamName.rotationFrequency, "частота вращения" },
        { ParamName.rotationFrequencyAcceleration, "ускорение частоты вращения" },
        { ParamName.rotationFrequencyJerk, "рывок частоты вращения" },
        { ParamName.numberOfRevolutions, "число оборотов" },
        { ParamName.step, "шаг" },
        { ParamName.deltaPosition, "изменение позиции" },
        { ParamName.deltaPathTraveled, "приращение пути" },
        { ParamName.accelerationStartTime, "время начала ускорения" },
        { ParamName.flightTime, "время полета" },
        { ParamName.landingVelocity, "скорость посадки" },
        { ParamName.range, "дальность" },
        { ParamName.averageSpeed, "средняя скорость" },
        { ParamName.mass, "масса" },
        { ParamName.mass2, "масса 2" },
        { ParamName.obstaclesMass, "масса препятствий" },
        { ParamName.velocity2, "скорость 2" },
        { ParamName.pointAReached, "точка A достигнута" },
        { ParamName.seed, "зерно случайности" },
        { ParamName.respawnObstacles, "возрождение препятствий" },
        { ParamName.friction, "трение" },
        { ParamName.force, "сила" },
        { ParamName.forceAcceleration, "ускорение от силы" },
        { ParamName.refractiveIndex, "показатель преломления" },
        { ParamName.unityPhycicsCalculation, "расчет физики Unity" },
        { ParamName.xPosition, "позиция X" },
        { ParamName.rayAngle, "угол луча" },
        { ParamName.material1_Size, "размер материала 1" },
        { ParamName.material1_Position, "позиция материала 1" },
        { ParamName.material1_RefractiveIndex, "показатель преломления материала 1" },
        { ParamName.material2_Size, "размер материала 2" },
        { ParamName.material2_Position, "позиция материала 2" },
        { ParamName.material2_RefractiveIndex, "показатель преломления материала 2" },
        { ParamName.material3_Size, "размер материала 3" },
        { ParamName.material3_Position, "позиция материала 3" },
        { ParamName.material3_RefractiveIndex, "показатель преломления материала 3" },
        { ParamName.additionalMass, "дополнительная масса" },
        { ParamName.position2, "позиция 2" },
        { ParamName.mass2Acceleration, "ускорение массы 2" },
        { ParamName.isMoving, "движется" },
        { ParamName.gearCount, "число шестерен" },
        { ParamName.module, "модуль" },
        { ParamName.teethCount, "число зубьев" },
        { ParamName.gearBox, "коробка передач" },
        { ParamName.totalGearRatio, "общее передаточное число" },
        { ParamName.outputAngularVelocity, "выходная угловая скорость" },
        { ParamName.outputFrequency, "выходная частота" },
        { ParamName.inputAngularVelocity, "входная угловая скорость" },
        { ParamName.inputFrequency, "входная частота" },
        { ParamName.helicalAngle, "спиральный угол" },
        { ParamName.volume, "объем" },
        { ParamName.density, "плотность" },
        { ParamName.piston1Square, "площадь поршня 1" },
        { ParamName.piston2Square, "площадь поршня 2" },
        { ParamName.pistonHeightDelta, "изменение высоты поршня" },
        { ParamName.weight, "вес" },
        { ParamName.applyingForce, "приложенная сила" },
        { ParamName.startingPosition, "стартовая позиция" },
        { ParamName.Submarine, "подводная лодка" },
        { ParamName.densityCount, "счетчик плотности" },
        { ParamName.startTime, "время начала" },
        { ParamName.startTime1, "время начала 1" },
        { ParamName.density1, "плотность 1" },
        { ParamName.startTime2, "время начала 2" },
        { ParamName.density2, "плотность 2" },
        { ParamName.density3, "плотность 3" },
        { ParamName.startTime3, "время начала 3" }
    };

    protected bool IsUserChangeNotificationSuppressed => userChangeNotificationSuppressionDepth > 0;

    protected void SuppressUserChangeNotification(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        userChangeNotificationSuppressionDepth++;
        try
        {
            action();
        }
        finally
        {
            userChangeNotificationSuppressionDepth = Math.Max(0, userChangeNotificationSuppressionDepth - 1);
        }
    }

    protected void RaiseUserValueChanged(string newValue)
    {
        if (IsUserChangeNotificationSuppressed)
            return;

        UserChangeTopicFieldValue?.Invoke(newValue, this);
    }

    protected virtual void Start()
    {
        
    }

    private void SetLabel(string newText)
    {
        label.text = newText;
    }

    private static string GetTranslation(ParamName paramName)
    {
        return Translations.TryGetValue(paramName, out var translation) ? translation : paramName.ToString();
    }

    public void Setup(bool isReadOnly, ParamName paramName, string defaultValue = "enter property")
    {
        IsReadOnly = isReadOnly;
        ParamName = paramName;
        Debug.Log(" Setup  " + GetType() + paramName.ToString());
        SetReadOnly(isReadOnly);
        SetLabel(GetTranslation(paramName));
    }

    public abstract bool SetValue(object newValue);

    protected abstract void SetReadOnly(bool value);

    protected abstract string GetText();
}

