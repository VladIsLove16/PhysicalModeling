using System.Collections.Generic;
using System;
using UnityEngine;

public class RefractionLensView : MotionView
{
    [SerializeField] MultiMaterialRefraction multiMaterialRefraction;
    // Maps view-model parameters to lens mutations
    Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();

    public override void OnEnabled()
    {
        base.OnEnabled();

        if (multiMaterialRefraction == null)
        {
            Debug.LogError("RefractionLensView requires MultiMaterialRefraction reference");
            actions.Clear();
            return;
        }

        actions.Clear();
        multiMaterialRefraction.OnEnabled();
        multiMaterialRefraction.SetTracerObject(MultiMaterialRefraction.RayTracerObject.lens);
        actions[ParamName.rayAngle] = value => multiMaterialRefraction.SetAngle((float)value);
        actions[ParamName.radius] = value => multiMaterialRefraction.SetLensRadius((float)value, true);
        actions[ParamName.distance] = value => multiMaterialRefraction.SetLensDistance((float)value, true);
        actions[ParamName.position] = value => multiMaterialRefraction.SetLensPosition((Vector3)value, true);
        actions[ParamName.xPosition] = value => multiMaterialRefraction.SetLensXPosition((float)value, true);
        actions[ParamName.refractiveIndex] = value => multiMaterialRefraction.SetLensRefractiveIndex((float)value);
        actions[ParamName.unityPhycicsCalculation] = value =>
        {
            bool usePhysics = value is bool flag && flag;
            multiMaterialRefraction.SetCalculationMode(usePhysics ? MultiMaterialRefraction.CalculationMode.physics : MultiMaterialRefraction.CalculationMode.mathematic);
        };
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        actions.Clear();

        if (multiMaterialRefraction != null)
            multiMaterialRefraction.OnDisabled();
    }

    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);

        if (multiMaterialRefraction == null)
            return;

        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
            action(newValue);
    }

    protected override void ViewModel_OnSimulationStateChanged()
    {
        // no-op for now
    }
}


