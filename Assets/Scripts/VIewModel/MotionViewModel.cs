using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MotionViewModel
{
    public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("Движение");
    public Dictionary<ParamName, ReactiveProperty<object>> Properties { get; } = new();
    public MotionModel CurrentModel;
    public Action CurrentModelChanged;
    public ReactiveProperty<SimulationState> simulationState = new ReactiveProperty<SimulationState>();
    public enum SimulationState
    {
        paused,
        running,
        stoped
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
        simulationState.Value = SimulationState.stoped;
        InitProperies(newModel);
        CurrentModel = newModel;
        CurrentModelChanged?.Invoke();
        Debug.Log("newModel.Parameters " + newModel.Parameters.Count);
        Debug.Log("MotionViewModel.Properties " + Properties.Count);
    }

    private void InitProperies(MotionModel newModel)
    {
        Properties.Clear();
        foreach (var modelParam in newModel.Parameters)
        {
            ReactiveProperty<object> property = new ReactiveProperty<object>(modelParam.Value.Value);
            if (!Properties.ContainsKey(modelParam.Key))
            {
                Properties[modelParam.Key] = property;
                modelParam.Value.Subscribe(value => OnModelChanged(modelParam.Key, property, value));
            }
        }
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
        simulationState.Value = SimulationState.running;
    }
    public void StopSimulation()
    {
        CurrentModel.ResetParams();
        simulationState.Value = SimulationState.stoped;
    }

    public void PauseSimulation()
    {
        simulationState.Value = SimulationState.paused;
    }

    public Vector3 Update(float deltaTime)
    {
        return CurrentModel.UpdatePosition(deltaTime);
    }

    internal bool SetParam(ParamName paramName, string obj)
    {
        Debug.Log("Setting Param" + paramName + " " + obj);
        TryParse(paramName, obj, out bool result);
        return result;
    }
    private void TryParse(ParamName paramName, string obj, out bool result)
    {
        var newValue = GetValueFromString(paramName, obj, out result);
        if (result)
        {
            CommitChanges(paramName, newValue);

        }
        else
            Debug.Log("wrong");
    }

    private void CommitChanges(ParamName paramName, object newValue)
    {
        Debug.Log("Success");
        if (paramName == ParamName.time)
        {
            PauseSimulation();
            CurrentModel.CalculatePosition((float)newValue);
        }
        CurrentModel.SetParam(paramName, newValue);
    }

    private object GetValueFromString(ParamName paramName, string value, out bool result)
    {

        FieldType fieldType = CurrentModel.GetFieldType(paramName); 
        Debug.Log("param: " + paramName + " " + fieldType);
        switch (fieldType)
        {
            case FieldType.Float:
                result = float.TryParse(value, out float floatValue);
                if (result)
                    return floatValue;
                break;
            case FieldType.Int:
                result = int.TryParse(value, out int intValue);
                if (result)
                    return intValue;
                break;
            case FieldType.Vector3:
                string[] values = value.Split(';');
               
                if (values.Length == 3 &&
                    float.TryParse(values[0], out float x) &&
                    float.TryParse(values[1], out float y) &&
                    float.TryParse(values[2], out float z))
                {
                    result = true;
                    return new Vector3(x, y, z);
                }
                result = false;
                if (result)
                    return Vector3.zero;
                break;
            default:
                result = false;
                return null;
        }
        return false;
    }
    public object GetParam(ParamName paramName)
    {
        return Properties[paramName].Value;
    }
}