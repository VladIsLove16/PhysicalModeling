using System.Collections.Generic;
using System;
using UnityEngine;
public class WaterView : MotionView
{
    [SerializeField] private  WaterCollider WaterCollider;
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    public override void OnDisabled()
    {
        base.OnDisabled();
        foreach (var key in actions.Keys)
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
  