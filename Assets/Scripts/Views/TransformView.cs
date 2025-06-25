using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TransformView : MotionView
{
    [SerializeField] private Transform MovingObject;
    protected override void ViewModel_OnPropertyChanged(TopicFieldController inputFieldController, object newValue)
    {
        base.ViewModel_OnPropertyChanged(inputFieldController, newValue);
        if (inputFieldController.ParamName == ParamName.position)
        {
            MovingObject.position = (Vector3)newValue;
        }
    }
}
