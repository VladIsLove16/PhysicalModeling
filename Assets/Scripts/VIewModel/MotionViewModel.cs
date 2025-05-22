using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MotionViewModel
{
    //public Action paramsChanged;
    private MotionModel CurrentModel;
    public Action CurrentModelChanged;
    public ReactiveProperty<SimulationState> simulationStateChanged = new ReactiveProperty<SimulationState>();
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
        simulationStateChanged.Value = SimulationState.stoped;
        GetFields(newModel);
        CurrentModel = newModel;
        //CurrentModel.paramsChanged += paramsChanged;
        CurrentModelChanged?.Invoke();
    }
    private List<TopicField> GetFields(MotionModel newModel, bool force = false)
    {
        var fields = newModel.GetTopicFields(force);
        return fields;
    }

    private void OnModelChanged(ParamName paramName, ReactiveProperty<object> property, object value)
    {
        if (CurrentModel == null || CurrentModel == null)
        { 
            return;
        }
        property.SetValueAndForceNotify(value);
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
        var topicField = CurrentModel.GetTopicField(paramName);
        result = topicField != null;
        if(!result)
        {
            Debug.LogAssertion("cant get param " + paramName + " from model");
        }
        return topicField.Value;
    }
    internal List<TopicField> GetFields(bool recreation = false)
    {
         return GetFields(CurrentModel, recreation);
    }
}