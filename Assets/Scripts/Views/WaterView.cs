using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
public class WaterView : MotionView
{
    [SerializeField] private  WaterCollider WaterCollider;
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    public override void OnEnabled()
    {
        base.OnEnabled();
        actions[ParamName.volume] = OnVolumeChanged;
        actions[ParamName.density] = OnDensityChanged;
        actions[ParamName.velocityMagnitude] = OnDensityChanged;
    }
    private void OnVolumeChanged(object value)
    {
        float volume = (float)value;
        WaterCollider.SetVolume(0, volume);
    }
    private void OnDensityChanged(object value)
    {
        float density = (float)value;
        WaterCollider.SetDensity(0, density);
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        var keys = actions.Keys.ToArray();
        foreach (var key in keys)
        {
            actions[key] = null;
        }
    }
    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);
        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
            action(newValue);
    }
}
