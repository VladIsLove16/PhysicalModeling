using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "GearMotionModel", menuName = "MotionModelsDropdown/GearMotionModel")]
public class GearMotionModel : MotionModel
{
    Gearbox gearbox = new Gearbox();    
    private Dictionary<ParamName, object> defaultValues = new Dictionary<ParamName, object>()
        {
            { ParamName.gearCount, (int)4 },
            { ParamName.inputAngularVelocity, 1f},
            { ParamName.inputFrequency, 1f},
            { ParamName.module, 2.0f },
            { ParamName.teethCount, (int)16 }
    };
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 30f },
        { ParamName.additionalMass, false },
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
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 pos = (Vector3)GetParam(ParamName.position);
        float force = (float)RampPhysics.GetForceForMass2(GetParam(ParamName.mass2));
        (Vector3 moveVector, Vector3 newVelocity) = RampPhysics.CheckInclined(
            (float)GetParam(ParamName.mass),
            (float)GetParam(ParamName.friction),
            (float)GetParam(ParamName.angleDeg),
            (Vector3)GetParam(ParamName.velocity),
            -force,
            deltaTime);

        float newMass = (float)GetParam(ParamName.mass2) + (float)GetParam(ParamName.mass2Acceleration) * deltaTime;
        TrySetParam(ParamName.mass2, newMass);
        Vector3 newPos = pos + moveVector;
        TrySetParam(ParamName.position, newPos);
        TrySetParam(ParamName.velocity, newVelocity);
        TrySetParam(ParamName.time, (float)GetParam(ParamName.time) + deltaTime);
        TrySetParam(ParamName.isMoving, (bool)!Mathf.Approximately(moveVector.magnitude, 0));
        return pos + moveVector;
    }
    public override bool TrySetParam(ParamName paramName, object value)
    {
        Debug.Log("paramName " + paramName + " changed ");
        if (paramName == ParamName.additionalMass)
        {
            Debug.Log("paramName");

        }
        return base.TrySetParam(paramName, value);
    }
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
               new TopicField(ParamName.position2, true),
               new TopicField(ParamName.isMoving, true),
               new TopicField(ParamName.velocity, true),
               new TopicField(ParamName.angleDeg, false),
               new TopicField(ParamName.mass, false),
               new TopicField(ParamName.mass2, false),
               new TopicField(ParamName.mass2Acceleration, false),
               new TopicField(ParamName.friction, false),
               new TopicField(ParamName.time, true)
            };
        return RequiredParams;
    }
    public override void ResetParam(ParamName paramName)
    {
        base.ResetParam(paramName);
    }
}
public partial class Gearbox 
{
    private List<GearPair> stages = new List<GearPair>();

    public void AddStage(Gear driver, Gear driven)
    {
        stages.Add(new GearPair(driver, driven));
    }

    public float GetTotalGearRatio()
    {
        float ratio = 1f;
        foreach (var pair in stages)
        {
            ratio *= pair.GearRatio;
        }
        return ratio;
    }

    // Выходная угловая скорость
    public float GetOutputAngularVelocity(float inputAngularVelocity)
    {
        return inputAngularVelocity / GetTotalGearRatio();
    }

    // Частота вращения выходного вала (об/с)
    public float GetOutputFrequency(float inputFrequency)
    {
        return inputFrequency / GetTotalGearRatio();
    }
}
public partial  class GearPair
{
    public Gear Driver { get; private set; } // Ведущее
    public Gear Driven { get; private set; } // Ведомое

    public float GearRatio => (float)Driven.TeethCount / Driver.TeethCount; // u = Z2 / Z1

    public GearPair(Gear driver, Gear driven)
    {
        if (driver.Module != driven.Module)
            throw new ArgumentException("Gears must have the same module.");

        Driver = driver;
        Driven = driven;
    }

    // Получение угловой скорости ведомого колеса (рад/с)
    public float GetDrivenAngularVelocity(float driverAngularVelocity)
    {
        return driverAngularVelocity / GearRatio;
    }

    // Получение частоты вращения ведомого колеса (об/с)
    public float GetDrivenFrequency(float driverFrequency)
    {
        return driverFrequency / GearRatio;
    }
}

public partial class Gear
{
    public float Module { get; private set; } // m
    public int TeethCount { get; private set; } // Z
    public float PitchDiameter => Module * TeethCount; // d

    public Gear(float module, int teethCount)
    {
        if (module <= 0 || teethCount <= 0)
            throw new ArgumentException("Module and teeth count must be positive.");

        Module = module;
        TeethCount = teethCount;
    }
}
