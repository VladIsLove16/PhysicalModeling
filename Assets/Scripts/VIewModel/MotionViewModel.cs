using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class MotionViewModel
{
    public ReactiveProperty<string> Title { get; } = new("Движение");
    public Dictionary<ParamName, ReactiveProperty<object>> Properties { get; } = new();
    public ReactiveProperty<MotionModel> CurrentModel { get; } = new();
    public ReactiveProperty<SimulationState> simulationState = new(SimulationState.stoped);

    public enum SimulationState
    {
        paused,
        running,
        stoped
    }

    public MotionViewModel(MotionModel model)
    {
        SetModel(model);
    }

    public void SetModel(MotionModel model)
    {
        CurrentModel.Value = model;
        model.InitializeParameters();
        Properties.Clear();

        var fieldList = model.Parameters.ToList();  // Создаём копию коллекции для безопасной итерации
        foreach (var kvp in fieldList)
        {
            ParamName paramName = kvp.Key;
            string modelValue = kvp.Value.Value;

            var property = new ReactiveProperty<object>(modelValue);
            property.Subscribe(newValue => OnPropertyChanged(paramName,modelValue));
            Properties[paramName] = property;
        }

        SyncFromModel();
    }

    private void OnPropertyChanged(ParamName paramName,object newValue   )
    {
        if (paramName == ParamName.time && simulationState.Value == SimulationState.stoped)
        {
            CurrentModel.Value.SetParameter(paramName, newValue);
            CurrentModel.Value.CalculatePosition((float)newValue);
            return;
        }
        CurrentModel.Value.SetParameter(paramName, newValue);

    }

    public void SyncFromModel()
    {
        var model = CurrentModel.Value;
        var keys = model.Parameters.Keys.ToList();
        foreach (var key in keys)
        {
            if (Properties.ContainsKey(key))
                Properties[key].SetValueAndForceNotify(model.Parameters[key]);
        }
    }

    public FieldType GetFieldType(object value) => CurrentModel.Value.GetFieldType(value);

    public FieldType GetFieldType(ParamName paramName) =>
        CurrentModel.Value.GetFieldType(paramName);

    public void StartSimulation() => simulationState.Value = SimulationState.running;

    public void StopSimulation()
    {
        CurrentModel.Value.ResetParams();
        simulationState.Value = SimulationState.stoped;
        SyncFromModel();
    }

    public void PauseSimulation() => simulationState.Value = SimulationState.paused;

    public Vector3 Update(float deltaTime)
    {
        var newPosition = CurrentModel.Value.UpdatePosition(deltaTime);
        SyncFromModel();
        return newPosition;
    }
}
