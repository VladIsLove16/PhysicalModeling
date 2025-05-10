using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;
public class ObstaclesView : MotionView
{
    [SerializeField] ObstacleSpawner obstacleSpawner;
    [SerializeField] GameObject MovingDirectionArrow;
    [SerializeField] public GameObject MovingObject;
    [SerializeField] bool clearObstaclesOnGeneration = true;
    // Инициализация словаря
    Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    protected override void Start()
    {
        obstacleSpawner.clearObstaclesOnGeneration = clearObstaclesOnGeneration;
    }
    public override void OnEnabled()
    {
        base.OnEnabled();
        actions[ParamName.seed] = value =>
        {
            obstacleSpawner.SetSeed((int)value);
            obstacleSpawner.SpawnObstacles();
        };
        actions[ParamName.angle] = value =>
        {
            if (MovingDirectionArrow != null)
            {
                MovingDirectionArrow.transform.eulerAngles = new Vector3(0, -(float)value, 0);
                MovingDirectionArrow.transform.position = Vector3.zero + MovingDirectionArrow.transform.right;
            }
        };
        actions[ParamName.obstaclesMass] = value =>
        {
            obstacleSpawner.SetObstaclesMass((float)value);
        };
        obstacleSpawner.SpawnObstacles();
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        actions[ParamName.seed] = null;
        actions[ParamName.angle] = null;
        actions[ParamName.obstaclesMass] = null;
        obstacleSpawner.ClearObstacles();
    }

    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);
        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
            action(newValue);
    }
    protected override void ViewModel_OnSimulationStateChanged()
    {
        base.ViewModel_OnSimulationStateChanged();
        switch (viewModel.simulationStateChanged.Value)
        {
            case MotionViewModel.SimulationState.stoped:
                object param = viewModel.TryGetParam(ParamName.respawnObstacles, out bool result);
                if(result)
                {
                    if((bool)param)
                        obstacleSpawner.SpawnObstacles();
                }
                break;
        }
    }
}