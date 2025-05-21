using System;
using System.Collections.Generic;
using UnityEngine;

public class RampBlockView : MotionView
{
    [SerializeField] RampMeshGenerator RampMeshGenerator;
    [SerializeField] GameObject MovingObject;
    [SerializeField] GameObject AdditionalMass;
    [SerializeField] LineRenderer YarnRenderer;
    [SerializeField] GameObject Block;
    private Dictionary<ParamName, Action<object>> actions = new Dictionary<ParamName, Action<object>>();
    [ContextMenu("move to zero")]
    public void Move()
    {
        RampMeshGenerator.Move(Vector3.zero);
    }
    public override void OnEnabled()
    {
        base.OnEnabled();
        Vector3 objOffset = GetMovingObjectSizeOffset();
        RampMeshGenerator.Move(Vector3.zero - objOffset);
        RampMeshGenerator.regenerated += () => OnRampMeshGenerator_regenerated(objOffset);
        Block.transform.position = RampMeshGenerator.GetUpperMiddle();
        actions[ParamName.angleDeg] = (value) =>
        {
            RampMeshGenerator.SetAngle((float)value, true);
            Block.transform.position = RampMeshGenerator.GetUpperMiddle() + RampMeshGenerator.transform.position + Vector3.up * Block.transform.localScale.y / 2;
            YarnRenderer.SetPositions(GetYarnPositions().ToArray());
            UpdateAdditionMassPos();
        };
        actions[ParamName.position] = (value) =>
        {
            MovingObject.transform.position = (Vector3)value;
            UpdateAdditionMassPos();
        };
    }

    private void UpdateAdditionMassPos()
    {
        Vector3 reachedDist = RampMeshGenerator.GetCenterOnIncline() + RampMeshGenerator.transform.position + MovingObject.transform.position + GetMovingObjectSizeOffset();
        AdditionalMass.transform.position = RampMeshGenerator.GetBackMiddle() + RampMeshGenerator.transform.position + Vector3.left / 2 + (reachedDist.y > 0 ? Vector3.down : Vector3.up) * reachedDist.magnitude;
    }

    private List<Vector3> GetYarnPositions()
    {
        List<Vector3> result = new List<Vector3>();
        Vector3 mass1Point = MovingObject.transform.position;
        Vector3 BlockPoint1 = Block.transform.position;
        Vector3 AdditionalMassPoint = AdditionalMass.transform.position;
        result.Add(mass1Point);
        result.Add((Vector3)BlockPoint1);
        result.Add((Vector3)AdditionalMassPoint);
        return result;
    }

    private void OnRampMeshGenerator_regenerated(Vector3 objOffset)
    {
        RampMeshGenerator.Move(Vector3.zero - objOffset);
    }

    private Vector3 GetMovingObjectSizeOffset()
    {
        float x = MovingObject.transform.localScale.x;
        float radius = Mathf.Sqrt(x * x / 8);
        return new Vector3(radius, radius, 0);
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        actions[ParamName.angleDeg] = null;
        actions[ParamName.position] = null;
    }

    protected override void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(topicFieldController, newValue);
        if (actions.TryGetValue(topicFieldController.ParamName, out var action))
        {
            if (action != null)
                action(newValue);
        }
    }
}