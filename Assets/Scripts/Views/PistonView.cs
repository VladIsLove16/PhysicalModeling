using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class PistonView : MotionView
{
    [SerializeField] private PistonVisualController pistonVisualController;
    [SerializeField] private GameObject mass;
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    public override void OnEnabled()
    {
        base.OnEnabled();
        actions[ParamName.piston1Square] = OnPiston1SquareChanged;
        actions[ParamName.piston2Square] = OnPiston2SquareChanged;
        actions[ParamName.pistonHeightDelta] = OnPistonHeightDeltaChanged;
    }

    private void OnPistonHeightDeltaChanged(object value)
    {
        float heightDelta = (float)value;
        pistonVisualController.Move(heightDelta);
    }
    protected override void ViewModel_OnSimulationStateChanged()
    {
        base.ViewModel_OnSimulationStateChanged();
        if(viewModel.simulationState.Value == MotionViewModel.SimulationState.stoped)
        {
            pistonVisualController.ResetSizes();
        }
    }

    private void OnPiston1SquareChanged(object value)
    {
        pistonVisualController.SetSquare(0,(float)value);
    }
    private void OnPiston2SquareChanged(object value)
    {
        pistonVisualController.SetSquare(1, (float)value);
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
