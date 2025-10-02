using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MotionViewModel
{
    //public Action paramsChanged;
    public MotionModel CurrentModel;
    public Action CurrentModelChanged;
    public ReactiveProperty<SimulationState> simulationState = new ReactiveProperty<SimulationState>();
    public int TopicFieldsCount => CurrentModel.TopicFieldsCount;

    public Action<ParamName,object> PropertyChanged { get; internal set; }

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
        List<TopicField>fields = GetFields(newModel);
        foreach (TopicField field in fields)
        {
            field.Property.Skip(1).Subscribe((newValue) => PropertyChanged.Invoke(field.ParamName, newValue));
        }
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
    //private void CommitChanges(ParamName ParamName, object newValue)
    //{
    //    Debug.Log("Success");
    //    if (ParamName == ParamName.time)
    //    {
    //        PauseSimulation();
    //        CurrentModel.CalculatePosition((float)newValue);
    //    }
    //    CurrentModel.SetParam(ParamName, newValue);
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

    internal bool IsReadOnly(ParamName paramName)
    {
       return CurrentModel.GetTopicField(paramName).IsReadOnly;
    }

    internal object GetMinValue(ParamName paramName)
    {
        return CurrentModel.GetTopicField(paramName).MinValue;
    }

    internal object GetMaxValue(ParamName paramName)
    {
       return CurrentModel.GetTopicField(paramName).MaxValue;
    }

    internal FieldType GetFieldType(ParamName paramName)
    {
     return   CurrentModel.GetTopicField(paramName).FieldType;
    }
    private object GetValueFromString(string value, FieldType FieldType,out bool result)
    {
        switch (FieldType)
        {
            case FieldType.Float:
                result = float.TryParse(value, out float floatValue);
                if (result)
                    return floatValue;
                else
                    return 0f;
            case FieldType.Int:
                result = int.TryParse(value, out int intValue);
                if (result)
                    return intValue;
                else
                    return 0;
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
                else
                {
                    result = false;
                    return Vector3.zero;
                }
            case FieldType.Bool:
                if (value == false.ToString())
                {
                    result = true;
                    return false;
                }
                else if (value == true.ToString())
                {
                    result = true;
                    return true;
                }
                else
                {
                    result = false;
                    return false;
                }
            default:
                result = false;
                return null;
        }
    }
    internal bool OnUserChangeParam(ParamName paramName, string str)
    {
        FieldType fieldType = GetFieldType(paramName); 
        object value = GetValueFromString(str, fieldType, out bool result);
        Debug.Log("OnUserChangeParam" + result);
        if (!result)
        {
            Debug.LogWarning("GetValueFromString failed " + paramName + " " + str);
            return false;
        }
        return CurrentModel.TrySetParam(paramName, value, true);
    }

    internal void TrySetParam(ParamName density1, float density2)
    {
        CurrentModel.TrySetParam(density1, density2, true);
    }
}

