using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class RigidbodyView : MotionView
{
    [SerializeField] private Transform MovingObject;
    public Rigidbody MovingObjectrb
    {
        get
        {
            if (_movingObjectrb == null)
            {
                _movingObjectrb = MovingObject.AddComponent<Rigidbody>();
                _movingObjectrb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            return _movingObjectrb;
        }
    }

    public Rigidbody HittedObjectrb
    {
        get
        {
            if (_hittedObjectrb == null)
            {
                _hittedObjectrb = HittedObject.AddComponent<Rigidbody>();
            }
            return _hittedObjectrb;
        }
    }
    [HideInInspector] public Rigidbody _movingObjectrb;
    [HideInInspector] public Rigidbody _hittedObjectrb;
    [SerializeField] private Transform HittedObject;
    [SerializeField] private Collider Floor;
    private Vector3 MovingObjectStartedPosition;
    private Vector3 HittedObjectStartedPosition;
    private float MovingObjectMass;
    private float HittedObjectMass;
    private Vector3 MovingObjectVelocity;
    private Vector3 HittedObjectVelocity;
    protected override void Start()
    {
        MovingObjectStartedPosition = MovingObject.transform.position;
        HittedObjectStartedPosition = HittedObject.transform.position;
    }
    protected override void ViewModel_OnPropertyChanged(TopicFieldController inputFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(inputFieldController, newValue);
        if (inputFieldController.ParamName == ParamName.mass)
        {
            MovingObjectMass = (float)newValue;
        }
        if (inputFieldController.ParamName == ParamName.mass2)
        {
            HittedObjectMass = (float)newValue;
        }
        if (inputFieldController.ParamName == ParamName.velocity)
        {
            MovingObjectVelocity = (Vector3)newValue;
        }
        if (inputFieldController.ParamName == ParamName.velocity2)
        {
            HittedObjectVelocity = (Vector3)newValue;
        }
        if (inputFieldController.ParamName == ParamName.position)
        {
            MovingObject.position = (Vector3)newValue;
        }
    }
    protected override void ViewModel_OnSimulationStateChanged()
    {
        Debug.Log("Rb view + ViewModel_OnSimulationStateChanged");
        base.ViewModel_OnSimulationStateChanged();
        var state = viewModel.simulationStateChanged.Value;
        switch (state)
        {
           case MotionViewModel.SimulationState.paused:
                ResetVelocities();
                break;
           case MotionViewModel.SimulationState.stoped:
                ResetPositions();
                ResetVelocities();
                ResetMasses();
                break;
           case MotionViewModel.SimulationState.running:
                MovingObjectrb.linearVelocity = MovingObjectVelocity;
                HittedObjectrb.linearVelocity = HittedObjectVelocity;
                MovingObjectrb.mass = MovingObjectMass;
                HittedObjectrb.mass = HittedObjectMass;
                break;
        }
    }
    private void ResetMasses()
    {
        HittedObjectrb.mass = 0;
        MovingObjectrb.mass = 0;
    }

    private void ResetVelocities()
    {
        MovingObjectrb.linearVelocity = Vector3.zero;
        HittedObjectrb.linearVelocity = Vector3.zero;
        MovingObjectrb.angularVelocity = Vector3.zero;
        HittedObjectrb.angularVelocity = Vector3.zero;
    }

    private void ResetPositions()
    {
        MovingObject.transform.position = MovingObjectStartedPosition;
        HittedObject.transform.position = HittedObjectStartedPosition;
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        Destroy(MovingObjectrb);
        Destroy(HittedObjectrb);
    }


    public override void OnEnabled()
    {
        base.OnEnabled();
        ResetPositions();
        Debug.Log(MovingObjectrb.name);
    }

    
}
