using System.Collections.Generic;
using System;
using UnityEngine;

public class RefractionView : MotionView
{
    [SerializeField] MultiMaterialRefraction multiMaterialRefraction;
    // Инициализация словаря
    Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    public override void OnEnabled()
    {
        base.OnEnabled();
        actions[ParamName.angle] = value =>
        {
            multiMaterialRefraction.SetAngle((float)value);
        };
        actions[ParamName.radius] = value =>
        {
            multiMaterialRefraction.SetRadius((float)value);
        };
        actions[ParamName.distance] = value =>
        {
            multiMaterialRefraction.SetLensDistance((float) value);
        };
        actions[ParamName.position] = value =>
        {
            multiMaterialRefraction.SetLensPosition((Vector3)value);
        };
        actions[ParamName.refractiveIndex] = value =>
        {
            multiMaterialRefraction.SetLensRefractiveIndex((float  )value);
        };
        actions[ParamName.unityPhycicsCalculation] = value =>
        {
            multiMaterialRefraction.calculationMode =(bool) value == true ? MultiMaterialRefraction.CalculationMode.physics : MultiMaterialRefraction.CalculationMode.mathematic;
        };
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        foreach(var action in actions)
        {
            actions[action.Key] = null;
        }
    }

    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);
        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
            action(newValue);
    }
    protected override void ViewModel_OnSimulationStateChanged()
    {
        //base.ViewModel_OnSimulationStateChanged();
        //switch (viewModel.simulationStateChanged.Value)
        //{
        //    case MotionViewModel.SimulationState.stoped:
        //        object param = viewModel.TryGetParam(ParamName.respawnObstacles, out bool result);
        //        if (result)
        //        {
        //            if ((bool)param)
        //                obstacleSpawner.SpawnObstacles();
        //        }
        //        break;
        //}
    }
}
