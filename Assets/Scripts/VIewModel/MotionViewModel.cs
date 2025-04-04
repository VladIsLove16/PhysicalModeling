using System;
using UniRx;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class MotionViewModel
{
    public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("Движение");
    public ReactiveDictionary<ParamName, ReactiveProperty<object>> Properties { get; } = new();
    public ReactiveProperty<MotionModel> CurrentModel { get; } = new();
    public ReactiveProperty<bool> isSimulating = new ReactiveProperty<bool>();
    public MotionViewModel(MotionModel model)
    {
        Properties = model.Parameters
           .ToReactiveDictionary(
               kvp => kvp.Key,
               kvp => kvp.Value.ToReadOnlyReactiveProperty() as IReadOnlyReactiveProperty<object>);
        CurrentModel.Value = motionModel;
        isSimulating.Value = false;
        SetTheme(motionModel);
    }

    public void SetTheme(MotionModel newModel)
    {
        Properties.Clear();

        foreach (var param in newModel.Parameters)
        {
            if (!Properties.ContainsKey(param.Key))
                Properties[param.Key] = new ReactiveProperty<object>(param.Value.Value);

            // Только подписка из Model -> ViewModel
            param.Value.Subscribe(value =>
            {
                if (!object.Equals(Properties[param.Key].Value, value))
                    Properties[param.Key].Value = value;
            });
        }
    }

    public FieldType GetFieldType(object value)
    {
        return CurrentModel.Value.GetFieldType(value);
    }
    public FieldType GetFieldType(ParamName paramName)
    {
        if (CurrentModel.Value.Parameters.ContainsKey(paramName))
        {
            return CurrentModel.Value.GetFieldType(CurrentModel.Value.Parameters[paramName].Value);
        }

        return FieldType.Float;
    }

    public void StartSimulation()
    {
        isSimulating.Value = true;
    }
    public void StopSimulation()
    {
        CurrentModel.Value.ResetParam(ParamName.time);
        isSimulating.Value = false;
    }

    public Vector3 Update(float deltaTime)
    {
       return CurrentModel.Value.CalculatePosition(deltaTime);
    }
}

