using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TransformView : MotionView
{
    [SerializeField] private Transform MovingObject;

    public override void OnDisabled()
    {
        
    }

    public override void OnEnabled()
    {
        base.OnEnabled();
    }

    protected override void ViewModel_OnPropertyChanged(InputFieldController inputFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(inputFieldController, newValue);
        if (inputFieldController.ParamName == ParamName.position)
        {
            MovingObject.position = (Vector3)newValue;
        }
    }
    protected override void ViewModel_OnSimulationStateChanged()
    {
        Debug.Log("TransformView + ViewModel_OnSimulationStateChanged");
    }
}
