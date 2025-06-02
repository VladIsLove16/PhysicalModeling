using System;
using System.Collections.Generic;
using System.Linq;
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
        //CurrentModel.paramsChanged += paramsChanged;
        CurrentModelChanged?.Invoke();
    }
    private List<TopicField> GetFields(MotionModel newModel, bool force = false)
    {
        var fields = newModel.GetTopicFields(force);
        return fields;
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
        var topicField = CurrentModel.GetTopicField(paramName);
        result = topicField != null;
        if(!result)
        {
            Debug.Log("cant get param " + paramName + " from model");
            return 0f;
        }
        return topicField.Value;
    }
    internal List<TopicField> GetFields(bool recreation = false)
    {
         return GetFields(CurrentModel, recreation);
    }

    internal void CalculatePosition()
    {
        float time = (float)  TryGetParam(ParamName.time, out bool result);
        if (!result)
        {
            time = 0f;
        }
        CurrentModel.CalculatePosition(time);
    }
}