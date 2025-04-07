using UniRx;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    [SerializeField]   public MotionModel motionModel;
    public MotionViewModel ViewModel;
    [SerializeField] public MotionView View;

    void Start()
    {
        motionModel.InitializeParameters();
        ViewModel = new MotionViewModel(motionModel);
        View.Init(ViewModel);
    }
}
