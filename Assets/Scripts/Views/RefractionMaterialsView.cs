using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class RefractionMaterialsView : MotionView
{
    [SerializeField] MultiMaterialRefraction multiMaterialRefraction;
    // Инициализация словаря
    Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();

    public override void OnEnabled()
    {
        base.OnEnabled();
        multiMaterialRefraction.OnEnabled();
        multiMaterialRefraction.SetTracerObject(MultiMaterialRefraction.RayTracerObject.materials);
        actions[ParamName.rayAngle] = value =>
        {
            multiMaterialRefraction.SetAngle((float)value);
        };
        actions[ParamName.material1_Size] = value =>
        {
            multiMaterialRefraction.SetMaterialSize(0,value);
        };
        actions[ParamName.material1_Position] = value =>
        {
            multiMaterialRefraction.SetMaterialPosition(0, value);
        };
        actions[ParamName.material1_RefractiveIndex] = value =>
        {
            multiMaterialRefraction.SetMaterialRefractiveIndex(0, value);
        };
        actions[ParamName.material2_Size] = value =>
        {
            multiMaterialRefraction.SetMaterialSize(1,value);
        };
        actions[ParamName.material2_Position] = value =>
        {
            multiMaterialRefraction.SetMaterialPosition(1,value);
        };
        actions[ParamName.material2_RefractiveIndex] = value =>
        {
            multiMaterialRefraction.SetMaterialRefractiveIndex(1, value);
        };
        actions[ParamName.material3_Size] = value =>
        {
            multiMaterialRefraction.SetMaterialSize(2,value);
        };
        actions[ParamName.material3_Position] = value =>
        {
            multiMaterialRefraction.SetMaterialPosition(2, value);
        };
        actions[ParamName.material3_RefractiveIndex] = value =>
        {
            multiMaterialRefraction.SetMaterialRefractiveIndex(2, value);
        };
        
        //actions[ParamName.unityPhycicsCalculation] = value =>
        //{
        //    multiMaterialRefraction.SetCalculationMode((bool)value == true ? MultiMaterialRefraction.CalculationMode.physics : MultiMaterialRefraction.CalculationMode.mathematic);
        //};
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        var keys = actions.Keys.ToArray();
        foreach (var key in keys)
        {
            actions[key] = null;
        }
        multiMaterialRefraction.OnDisabled();
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
