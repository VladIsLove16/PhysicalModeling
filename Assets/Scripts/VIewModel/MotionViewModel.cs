using System;
using System.Collections.Generic;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MotionViewModel
{
    private MotionModel CurrentModel;
    public Action CurrentModelChanged;
    private Dictionary<ParamName,TopicField> properties = new();
    public ReactiveProperty<SimulationState> simulationStateChanged = new ReactiveProperty<SimulationState>();
    public enum SimulationState
    {
        paused,
        stoped,
        running,
    }
    public MotionViewModel(MotionModel motionModel)
    {
        Init(motionModel);
    }
    public void Init(MotionModel newModel)
    {
        if (newModel == null)
        {
            Debug.LogWarning("MotionModel is null");
            return;

        }
        else
            Debug.Log("Initing viewmodel with" + newModel.ToString());
        simulationStateChanged.Value = SimulationState.stoped;
        InitProperies(newModel);
        CurrentModel = newModel;
        CurrentModelChanged?.Invoke();
        Debug.Log("newModel.Params " + newModel.TopicFields.Count);
        Debug.Log("MotionViewModel.Properties " + properties.Count);
    }

    private void InitProperies(MotionModel newModel)
    {
        properties.Clear();
        properties = newModel.TopicFields;
        //foreach (var modelParam in newModel.TopicFields)
        //{
        //    ReactiveProperty<object> property = new ReactiveProperty<object>(modelParam.Value.Value);
        //    if (!properties.ContainsKey(modelParam.Key))
        //    {
        //        properties[modelParam.Key] = property;
        //        modelParam.Value.Subscribe(value => OnModelChanged(modelParam.Key, property, value));
        //    }
        //}
    }

    private void OnModelChanged(ParamName paramName, ReactiveProperty<object> property, object value)
    {
        if (CurrentModel == null || CurrentModel == null)
        { 
            return;
        }
        property.SetValueAndForceNotify(value);
    }

    public FieldType GetFieldType(ParamName value)
    {
        return CurrentModel.GetFieldType(value);
    }

    public void StartSimulation()
    {
        if(simulationStateChanged.Value == SimulationState.paused)
        {
            CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.continued);
        }
        else
            CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.started);
        simulationStateChanged.Value = SimulationState.running;
    }
    public void StopSimulation()
    {
        CurrentModel.ResetParams();
        simulationStateChanged.Value = SimulationState.stoped;
        CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.stoped);
    }

    public void PauseSimulation()
    {
        simulationStateChanged.Value = SimulationState.paused;
        CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.paused);
    }

    public Vector3 Update(float deltaTime)
    {
        return CurrentModel.UpdatePosition(deltaTime);
    }
    //private void CommitChanges(ParamName paramName, object newValue)
    //{
    //    Debug.Log("Success");
    //    if (paramName == ParamName.time)
    //    {
    //        PauseSimulation();
    //        CurrentModel.CalculatePosition((float)newValue);
    //    }
    //    CurrentModel.SetParam(paramName, newValue);
    //}

    
    public object TryGetParam(ParamName paramName, out bool result)
    {
        result = GetProperties().TryGetValue(paramName, out TopicField topicField);
        if(!result)
        {
            Debug.LogAssertion("cant get param " + paramName + " from model");
        }
        return topicField.Value;
    }
    public bool IsReadonly(ParamName paramName)
    {
        return CurrentModel.IsReadonly(paramName);
    }

    internal Dictionary<ParamName,TopicField> GetProperties()
    {
        if (properties == null
                 || properties.Count == 0)
        {
            InitProperies(CurrentModel);
        }
        return properties;
    }
}