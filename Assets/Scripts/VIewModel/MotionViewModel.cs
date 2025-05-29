using System;
using System.Collections.Generic;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MotionViewModel
{
    //public Action paramsChanged;
    public MotionModel CurrentModel;
    public Action CurrentModelChanged;
    public ReactiveProperty<SimulationState> simulationState = new ReactiveProperty<SimulationState>();
    public int TopicFieldsCount => CurrentModel.TopicFieldsCount;

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
        simulationState.Value = SimulationState.stoped;
        GetFields(newModel);
        CurrentModel = newModel;
        CurrentModel.paramsChanged += paramsChanged;
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

    public void StartSimulation()
    {
        if(simulationState.Value == SimulationState.paused)
        {
            CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.continued);
        }
        else
            CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.started);
        simulationState.Value = SimulationState.running;
    }
    public void StopSimulation()
    {
        CurrentModel.ResetParams();
        simulationState.Value = SimulationState.stoped;
        CurrentModel.OnSimulationStateChanged(MotionModel.SimulationState.stoped);
    }

    public void PauseSimulation()
    {
        simulationState.Value = SimulationState.paused;
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

    internal void CalculatePosition()
    {
      float time = (float)  TryGetParam(ParamName.time, out bool result);
        CurrentModel.CalculatePosition(time);
    }
}