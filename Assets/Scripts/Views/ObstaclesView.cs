using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesView : MotionView
{
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] public GameObject MovingObject;
    [SerializeField] private GameObject MovingDirectionArrow;
    [SerializeField] private bool clearObstaclesOnGeneration = true;

    private readonly Dictionary<ParamName, Action<object>> actions = new();
    private MotionViewModel.SimulationState lastSimulationState = MotionViewModel.SimulationState.stoped;

    protected override void Start()
    {
        base.Start();
        obstacleSpawner.clearObstaclesOnGeneration = clearObstaclesOnGeneration;
    }

    public override void OnEnabled()
    {
        base.OnEnabled();

        actions.Clear();
        obstacleSpawner.clearObstaclesOnGeneration = clearObstaclesOnGeneration;

        lastSimulationState = viewModel != null ? viewModel.simulationState.Value : MotionViewModel.SimulationState.stoped;

        actions[ParamName.seed] = value =>
        {
            obstacleSpawner.SetSeed((int)value);
            obstacleSpawner.ReSpawnObstacles();
        };

        actions[ParamName.angleDeg] = value =>
        {
            if (MovingDirectionArrow == null)
                return;

            float angle = (float)value;
            MovingDirectionArrow.transform.eulerAngles = new Vector3(0, -angle, 0);
            MovingDirectionArrow.transform.position = Vector3.zero + MovingDirectionArrow.transform.right;
        };

        actions[ParamName.obstaclesMass] = value => obstacleSpawner.SetObstaclesMass((float)value);

        obstacleSpawner.ReSpawnObstacles();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        actions.Clear();
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

        var state = viewModel != null ? viewModel.simulationState.Value : MotionViewModel.SimulationState.stoped;

        switch (state)
        {
            case MotionViewModel.SimulationState.stoped:
                object param = viewModel.TryGetParam(ParamName.respawnObstacles, out bool result);
                if (result && param is bool shouldRespawn && shouldRespawn)
                    obstacleSpawner.ReSpawnObstacles();
                break;
        }

        lastSimulationState = state;
    }
}