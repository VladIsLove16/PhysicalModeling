using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;

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
            return defaultValues;
        }
    }
    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
    }; ;
        }
    }
    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
            { ParamName.module, 0f },
            { ParamName.teethCount, (int)0 },
    }; ;
        }
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float inputAngularVelocity = (float)GetParam(ParamName.inputAngularVelocity);
        float inputFrequency = (float)GetParam(ParamName.inputFrequency);
        //gearbox.inputAngularVelocity = inputAngularVelocity;
        //gearbox.inputAngularVelocity = inputFrequency;

        float outputAngularVelocity = gearbox.GetOutputAngularVelocity(inputAngularVelocity);
        float outputFrequency = gearbox.GetOutputFrequency(inputFrequency);
        Debug.Log(outputAngularVelocity);
        TrySetParam(ParamName.totalGearRatio, gearbox.GetTotalGearRatio());
        TrySetParam(ParamName.outputAngularVelocity, outputAngularVelocity);
        TrySetParam(ParamName.outputFrequency, outputFrequency);
        return Vector3.zero;
    }

    public override Vector3 CalculatePosition(float Time)
    {
        return Vector3.zero;
    }
    public override void InitializeParameters(bool isForce = false)
    {
        base.InitializeParameters(isForce);
        GenerateGearBox();
    }
    private void GenerateGearBox()
    {
        gearbox = new();
        Gear currentGear = null;
        TopicField moduleTopicField = null;
        TopicField gearBoxTopicField = GetTopicField(ParamName.gearBox);
        Gear nextGear = null;

        foreach (var field in topicFields)
        {
            if (field.ParamName == ParamName.module)
                moduleTopicField = field;
            
            if (field.ParamName == ParamName.teethCount)
            {
                if (currentGear == null)
                {
                    float module = (float)moduleTopicField.Value;
                    int teethCount = (int)field.Value;
                    currentGear = new Gear(module, teethCount);
                }
                else
                {
                    nextGear = new Gear((float)moduleTopicField.Value, (int)field.Value);
                    gearbox.AddStage(currentGear, nextGear);
                    currentGear = nextGear;
                }
                field.Property.Skip(1).Subscribe(OnTeethCountChanged(currentGear));
                moduleTopicField.Property.Skip(1).Subscribe(OnModuleChanged(currentGear));
            }
        }
        gearBoxTopicField.TrySetValue(gearbox);
        Debug.Log(" gearbox.StageCount " + gearbox.StageCount);
    }

    private Action<object> OnModuleChanged(Gear nextGear)
    {
        return value =>
        {
            nextGear.SetModule((float)value);
        };
    }
    public override void ResetParam(TopicField field)
    {
        if(field.ParamName == ParamName.gearBox)
            return;
        base.ResetParam(field);
    }
    private Action<object> OnTeethCountChanged(Gear nextGear)
    {
        return value =>
        {
            Debug.Log("new teeth count");
            nextGear.SetTeethCount((int)value);
        };
    }

    public override List<TopicField> GetRequiredParams()
    {
        List<TopicField> RequiredParams = new List<TopicField>()
        {
            new TopicField(ParamName.gearCount, false),
            new TopicField(ParamName.inputAngularVelocity, false),
            new TopicField(ParamName.inputFrequency, false),
            new TopicField(ParamName.outputAngularVelocity, true),
            new TopicField(ParamName.outputFrequency, true),
            new TopicField(ParamName.totalGearRatio, true),
            new TopicField(ParamName.gearBox, true),
        };
        var value =  GetTopicField(ParamName.gearCount);
        int gearCount;
        if (value != null && value.Value != null)
        { 
            gearCount = (int)value.Value;
            defaultValues[ParamName.gearCount] = gearCount;
        }
        else
            gearCount = (int)defaultValues[ParamName.gearCount];
        for (int i = 0; i < gearCount; i++)
        {
            RequiredParams.Add(new TopicField(ParamName.module, false));
            RequiredParams.Add(new TopicField(ParamName.teethCount, false));
        }
        return RequiredParams;
    }
    public override void ResetParam(ParamName paramName)
    {
        base.ResetParam(paramName);
        if (paramName == ParamName.gearBox)
            return;
    }
    public override void ResetParams()
    {
        return;
    }
}
public partial class Gearbox 
{
    private List<GearPair> stages = new List<GearPair>();
    internal Action<Gearbox, bool> changed;

    //public float inputAngularVelocity;
    //public float inputFrequency;
    public int StageCount => stages.Count;
    public void AddStage(Gear driver, Gear driven)
    {
        if(stages.Count == 0)
        {
            driver.ModuleProperty.Skip(1).Subscribe(_ => changed?.Invoke(this, true));
            driver.TeethCountProperty.Skip(1).Subscribe(_ => changed?.Invoke(this, true));
        }
        driven.ModuleProperty.Skip(1).Subscribe(_ => changed?.Invoke(this, true));
        driven.TeethCountProperty.Skip(1).Subscribe(_ => changed?.Invoke(this, true));
        stages.Add(new GearPair(driver, driven));
    }
    public void Clear()
    {
        stages.Clear();
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
    public ReactiveProperty<float> ModuleProperty = new();
    public ReactiveProperty<float> TeethCountProperty = new();
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
    public void SetModule(float module)
    {
        this.Module = module;
        ModuleProperty.SetValueAndForceNotify(module);
    }
    public void SetTeethCount(int teethCount)
    {
        this.TeethCount = teethCount;
        TeethCountProperty.SetValueAndForceNotify(teethCount);
    }
}
