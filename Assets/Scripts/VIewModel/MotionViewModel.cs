using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;
using static UnityEngine.Rendering.DebugUI;

public class MotionViewModel
{
    public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("Движение");
    public Dictionary<ParamName, ReactiveProperty<object>> Properties { get; } = new();
    public ReactiveProperty<MotionModel> CurrentModel { get; } = new();
    public ReactiveProperty<SimulationState> simulationState = new ReactiveProperty<SimulationState>();
    public enum SimulationState
    {
        paused,
        running,
        stoped
    }
    public MotionViewModel(MotionModel model)
    {
        simulationState.Value = SimulationState.stoped;
        SetTheme(model);
    }

    public void SetTheme(MotionModel newModel)
    {
        Properties.Clear();
        CurrentModel.Value = newModel;
        foreach (var modelParam in newModel.Parameters)
        {
            if (!Properties.ContainsKey(modelParam.Key))
                Properties[modelParam.Key] = new ReactiveProperty<object>(modelParam.Value.Value);
            modelParam.Value.Subscribe(value => OnModelChanged(modelParam.Key, Properties[modelParam.Key], value));
        }
    }
    private void OnModelChanged(ParamName paramName, ReactiveProperty<object> property, object value)
    {
        if (paramName == ParamName.time)
        {
            if (simulationState.Value == SimulationState.stoped)
            {
                CurrentModel.Value.CalculatePosition((float)property.Value);
                return;
            }
        }
        property.SetValueAndForceNotify(value);
    }

    public FieldType GetFieldType(ParamName value)
    {
        return CurrentModel.Value.GetFieldType(value);
    }

    public void StartSimulation()
    {
        simulationState.Value = SimulationState.running;
    }
    public void StopSimulation()
    {
        CurrentModel.Value.ResetParams();
        simulationState.Value = SimulationState.stoped;
    }

    public void PauseSimulation()
    {
        simulationState.Value = SimulationState.paused;
    }

    public Vector3 Update(float deltaTime)
    {
        return CurrentModel.Value.UpdatePosition(deltaTime);
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
            CurrentModel.Value.CalculatePosition((float)newValue);
        }
        CurrentModel.Value.SetParam(paramName, newValue);
    }

    private object GetValueFromString(ParamName paramName, string value, out bool result)
    {

        FieldType fieldType = CurrentModel.Value.GetFieldType(paramName); 
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
    //public object SetParam(ParamName paramName)
    //{
    //    return Properties[paramName].Value;
    //}

   
}