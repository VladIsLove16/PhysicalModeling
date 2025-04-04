using UniRx;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    [SerializeField]   public MotionModel motionModel;
    public MotionViewModel ViewModel;
    [SerializeField] public MotionView View;
    public Transform MovingObject;
    private Vector3 currentPosition;

    void Start()
    {
        motionModel.InitializeParameters();
        ViewModel = new MotionViewModel(motionModel);
        View.Init(ViewModel);
        currentPosition = MovingObject.position;
        ViewModel.isSimulating.Subscribe(_=>OnIsSimulatingChanged());
    }
    private void Update()
    {
        if(ViewModel.isSimulating.Value)
        {
            currentPosition = ViewModel.Update(Time.deltaTime);
            MovingObject.position = currentPosition;
        }
    }
    private void OnIsSimulatingChanged()
    {
        if(!ViewModel.isSimulating.Value)
        {
            MovingObject.position = Vector3.zero;
        }
    }
}
