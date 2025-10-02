using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterView : MotionView
{
    [SerializeField] private WaterCollider WaterCollider;
    private readonly Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();

    public override void OnEnabled()
    {
        base.OnEnabled();

        if (WaterCollider == null)
        {
            Debug.LogError("WaterView requires WaterCollider reference");
            actions.Clear();
            return;
        }

        actions.Clear();
        actions[ParamName.volume] = OnVolumeChanged;
        actions[ParamName.density] = OnDensityChanged;
        actions[ParamName.velocityMagnitude] = OnDensityChanged;
    }

    private void OnVolumeChanged(object value)
    {
        if (WaterCollider == null)
            return;

        float volume = value is float f ? f : Convert.ToSingle(value);
        WaterCollider.SetVolume(0, volume);
    }

    private void OnDensityChanged(object value)
    {
        if (WaterCollider == null)
            return;

        float density = value is float f ? f : Convert.ToSingle(value);
        WaterCollider.SetDensity(0, density);
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        actions.Clear();
    }

    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);

        if (WaterCollider == null)
            return;

        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
            action(newValue);
    }
}

