using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UniRx;
using System.Linq;

public class GearView : MotionView
{
   [SerializeField] private GearSystemVisualizer gearSystemVisualizer;
    Gearbox gearbox = new();
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    public override void Init(MotionViewModel motionViewModel)
    {
        base.Init(motionViewModel);
        Debug.Log(TopicFieldsCount);
        var param = motionViewModel.TryGetParam(ParamName.gearBox, out bool result);
        if (result)
            gearbox = (Gearbox)param;
        
        gearSystemVisualizer.GenerateSystem(gearbox);
        actions[ParamName.gearCount] = OnGearCountChanged;
        actions[ParamName.gearBox] = OnGearBoxChanged;
        actions[ParamName.inputAngularVelocity] = (value) => gearSystemVisualizer.SetAngularVelocity((float)value);
    }
    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);
        bool result = actions.TryGetValue(topicFieldController.ParamName, out var action);
        if (result)
            action(newValue);
    }
    private void OnGearCountChanged(object value)
    {
        RebuildUI(true);
        gearbox = (Gearbox)viewModel.TryGetParam(ParamName.gearBox, out bool result);
        gearSystemVisualizer.GenerateSystem(gearbox);
        var AngularVelocity = viewModel.TryGetParam(ParamName.inputAngularVelocity, out bool result2);
        if (result2)
            gearSystemVisualizer.SetAngularVelocity((float)AngularVelocity);
    }
    private void OnGearBoxChanged(object value)
    {
        if(value ==null)
        {
            Debug.LogAssertion("new Gearbox is null");
            return;
        }
        if(gearbox == null)
        {
            Debug.LogAssertion("Current Gearbox is null");
            return;
        }
        gearbox = (Gearbox)value;
        gearSystemVisualizer.GenerateSystem(gearbox);
    }
}
