using System;
using System.Collections.Generic;
using UnityEngine;

public class RampView: MotionView
{

    [SerializeField] RampMeshGenerator RampMeshGenerator;
    [SerializeField] GameObject MovingObject;
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    [ContextMenu("move to zero")]
    public void Move()
    {
        RampMeshGenerator.Move(Vector3.zero);
    }
    public override void OnEnabled()
    {
        base.OnEnabled();
        Vector3 objOffset = GetObjOffset();
        RampMeshGenerator.Move(Vector3.zero + objOffset);
        RampMeshGenerator.Move(Vector3.zero - objOffset);
        RampMeshGenerator.regenerated += () => OnRampMeshGenerator_regenerated(objOffset);
        actions[ParamName.angle] = (value) => RampMeshGenerator.SetAngle((float)value, true);
        actions[ParamName.position] = (value) => MovingObject.transform.position = (Vector3)value;
    }

    private void OnRampMeshGenerator_regenerated(Vector3 objOffset)
    {
        RampMeshGenerator.Move(Vector3.zero - objOffset);
    }

    private Vector3 GetObjOffset()
    {
        float x = MovingObject.transform.localScale.x;
        float radius =Mathf.Sqrt(x * x/8);
        return new Vector3(radius, radius, 0);
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        actions[ParamName.angle] = null;
        actions[ParamName.position] = null;
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
        //switch (viewModel.simulationStateChanged.Value)
        //{
        //    case MotionViewModel.SimulationState.stoped:
        //        object param = viewModel.TryGetParam(ParamName.respawnObstacles, out bool result);
        //        if (result)
        //        {
        //            if ((bool)param)
        //                obstacleSpawner.SpawnObstacles();
        //        }
        //        break;
        //}
    }
}
