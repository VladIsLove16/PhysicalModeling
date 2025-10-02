using System.Collections.Generic;
using System;
using UnityEngine;

public class SubmarineView : MotionView
{
    [SerializeField] private WaterCollider WaterCollider;
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    Submarine submarine = null;
    float time;
    int currentIndex = 0;
    private void Update()
    {
        if (viewModel == null || WaterCollider == null || submarine == null)
            return;
        if (viewModel.simulationState.Value == MotionViewModel.SimulationState.running)
        {
            time += Time.deltaTime;
            bool a = submarine.Count > currentIndex;
            float nextTime = a ? submarine.GetTime(currentIndex) : Mathf.Infinity;
            Debug.Log("currentIndex  " + currentIndex + " current time" + time + "  submarine.Count" + submarine.Count + " " + a + " newx time +" + nextTime + " submarine times " + submarine.Times()) ;
            if (submarine != null && a && time > nextTime)
            {
                float density = submarine.GetDensity(currentIndex);
                Debug.Log("current density " + density);
                WaterCollider?.SetDensity(density);
                viewModel.TrySetParam(ParamName.density, density);
                currentIndex++;
            }
            WaterCollider?.CustomUpdate();
        }
        else
        {
            time = 0f;
            WaterCollider?.ResetSimulation();
            currentIndex = 0;
        }
    }
    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);
        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
            action(newValue);
    }
    public override void Init(MotionViewModel motionViewModel)
    {
        base.Init(motionViewModel);

        if (WaterCollider == null)
        {
            Debug.LogError("SubmarineView requires WaterCollider reference");
            actions.Clear();
            submarine = null;
            return;
        }

        actions.Clear();
        submarine = new();
        Add();
        actions[ParamName.volume] = OnVolumeChanged;
        actions[ParamName.velocityMagnitude] = OnvelocityMagnitudeChanged;
        actions[ParamName.density] = (value) => submarine.SetDensity(0, value);
        actions[ParamName.density1] = (value) => submarine.SetDensity(1, value);
        actions[ParamName.startTime1] = (value) => submarine.SetTime(1, value);
        actions[ParamName.density2] = (value) => submarine.SetDensity(2, value);
        actions[ParamName.startTime2] = (value) => submarine.SetTime(2, value);
        actions[ParamName.density3] = (value) => submarine.SetDensity(3, value);
        actions[ParamName.startTime3] = (value) => submarine.SetTime(3, value);

        var initialVolume = viewModel.TryGetParam(ParamName.volume, out _);
        OnVolumeChanged(initialVolume);

        var initialVelocity = viewModel.TryGetParam(ParamName.velocityMagnitude, out _);
        OnvelocityMagnitudeChanged(initialVelocity);

        var initialDensity = viewModel.TryGetParam(ParamName.density, out _);
        WaterCollider?.SetDensity(ToFloat(initialDensity));
        WaterCollider?.ResetSimulation();
        currentIndex = 0;
        time = 0f;
    }

    private void Add()
    {
        object val = viewModel.TryGetParam(ParamName.density, out bool result);
        float density = ToFloat(val);
        float density1 = ToFloat(viewModel.TryGetParam(ParamName.density1, out bool result1));
        float density2 = ToFloat(viewModel.TryGetParam(ParamName.density2, out bool result2));
        float density3 = ToFloat(viewModel.TryGetParam(ParamName.density3, out bool result3));
        float time1 = ToFloat(viewModel.TryGetParam(ParamName.startTime1, out bool result11));
        float time2 = ToFloat(viewModel.TryGetParam(ParamName.startTime2, out bool result22));
        float time3 = ToFloat(viewModel.TryGetParam(ParamName.startTime3, out bool result33));
        submarine.Add(new Submarine.SubmarineInfo(density, 0f));
        submarine.Add(new Submarine.SubmarineInfo(density1, time1));
        submarine.Add(new Submarine.SubmarineInfo(density2, time2));
        submarine.Add(new Submarine.SubmarineInfo(density3, time3));


    }
    private void SubmarineChanged()
    {
        time = 0f;
        Debug.LogWarning("SubmarineChanged");
        WaterCollider?.ResetSimulation();
        currentIndex = 0;
    }

    private void OnvelocityMagnitudeChanged(object obj)
    {
        float velocity = ToFloat(obj);
        Debug.LogWarning("new vel " + velocity);
        //currentIndex  = 0;
        //WaterCollider.ResetSimulation();
        WaterCollider?.SetVelocity(velocity);
    }


    private void OnVolumeChanged(object value)
    {
        float volume = ToFloat(value);
        WaterCollider?.SetVolume(0, volume);
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        actions.Clear();
        submarine = null;
    }

    private static float ToFloat(object value)
    {
        if (value == null)
            return 0f;

        return float.TryParse(value.ToString(), out var result) ? result : 0f;
    }
}
public class Submarine
{
    public class SubmarineInfo
    {
        public Action<object> OnChanged;
        public SubmarineInfo(
         float density,
         float time)
        {
            this.time = time;
            this.density = density;
        }
        public float density;
        public float time;

        internal void SetDensity(float value)
        {
            density = value;
            OnChanged?.Invoke(this);
        }

        internal void SetTime(float value)
        {
            time = value;
            OnChanged?.Invoke(this);
        }
    }
    public int Count => Volumes.Count;
    private List<SubmarineInfo> Volumes = new List<SubmarineInfo>();

    public Action OnChanged;

    public void Add(SubmarineInfo info)
    {
        info.OnChanged += (value) => OnChanged?.Invoke();
        Volumes.Add(info);
    }

    internal float GetTime(int v)
    {
        if (v >= Volumes.Count)
            throw new ArgumentException();
        return Volumes[v].time;
    }

    internal float GetDensity(int index)
    {
        if (index >= Volumes.Count)
            throw new ArgumentException();

        return Volumes[index].density;
    }

    internal string Times()
    {
        string str = "";
        foreach (SubmarineInfo info in Volumes)
        {
            str += " " + info.time;
        }
        return str;
    }

    internal void SetDensity(int index, object value)
    {
        if (index < 0 || index >= Volumes.Count)
            return;

        float floatValue = value is float f ? f : Convert.ToSingle(value);
        if (Volumes[index] == null)
        {
            Volumes[index] = new SubmarineInfo(floatValue, 0f);
        }
        else
        {
            Volumes[index].SetDensity(floatValue);
        }
    }

    internal void SetTime(int index, object value)
    {
        if (index < 0 || index >= Volumes.Count)
            return;

        float floatValue = value is float f ? f : Convert.ToSingle(value);
        if (Volumes[index] == null)
        {
            Volumes[index] = new SubmarineInfo(0f, floatValue);
        }
        else
        {
            Volumes[index].SetTime(floatValue);
        }
    }
}





