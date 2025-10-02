using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
public abstract class MotionView : MonoBehaviour
{
    [SerializeField] private Button toggleSimulationButton;
    [SerializeField] private Button stopSimulationButton;
    [SerializeField] private TextMeshProUGUI toggleSimulationButtonText;
    [SerializeField] private Transform inputFieldsContainer;
    [SerializeField] private InputFieldTopicFieldController inputFieldPrefab;
    [SerializeField] private SliderTopicFieldController sliderPrefab;
    [SerializeField] private ToggleTopicFieldController toggleprefab;
    [SerializeField] protected Camera _Camera;
    [SerializeField] protected Vector3 CameraNewPosition;
    [SerializeField] protected Vector3 CameraRotation;
    [SerializeField] private GameObject[] actionObjects;
    protected MotionViewModel viewModel;
    private Dictionary<ParamName, TopicFieldController> inputFields = new();
    public int TopicFieldsCount => viewModel.TopicFieldsCount;
    private CompositeDisposable disposables = new();
    private List<IDisposable> uiDisposables = new();

    private readonly Dictionary<MotionViewModel.SimulationState, string> simulationButtonTexts = new()
    {
        { MotionViewModel.SimulationState.running, "Пауза" },
        { MotionViewModel.SimulationState.paused, "Продолжить" },
        { MotionViewModel.SimulationState.stoped, "Старт" },
    };
    private readonly Dictionary<ParamName, ViewType> topicFieldsViewTypes = new()
    {
        { ParamName.angleDeg, ViewType.Slider },
        { ParamName.additionalMass, ViewType.Toggle },
        { ParamName.isMoving, ViewType.Toggle },
        { ParamName.density, ViewType.inputField },
        { ParamName.distance, ViewType.Slider },
        { ParamName.friction, ViewType.Slider },
        { ParamName.helicalAngle, ViewType.Slider },
        { ParamName.seed, ViewType.Slider },
        { ParamName.pointAReached, ViewType.Toggle },
        { ParamName.gearBox, ViewType.inputField },
        { ParamName.volume, ViewType.inputField },
        { ParamName.respawnObstacles, ViewType.Toggle },
        { ParamName.unityPhycicsCalculation, ViewType.Toggle },
        { ParamName.xPosition, ViewType.Slider },
        { ParamName.rayAngle, ViewType.Slider },
        { ParamName.refractiveIndex, ViewType.Slider },
        { ParamName.radius, ViewType.Slider },
        { ParamName.weight, ViewType.Slider },
        { ParamName.applyingForce, ViewType.Slider },

        {ParamName.material1_Size,ViewType.inputField },
        {ParamName.material1_Position,ViewType.inputField },
        {ParamName.material1_RefractiveIndex,ViewType.Slider },
        {ParamName.material2_Size,ViewType.inputField },
        {ParamName.material2_Position,ViewType.inputField },
        {ParamName.material2_RefractiveIndex,ViewType.Slider },
        {ParamName.material3_Size,ViewType.inputField },
        {ParamName.material3_Position,ViewType.inputField },
        {ParamName.material3_RefractiveIndex,ViewType.Slider },
    };

    protected virtual void Start()
    {

    }
   
    private void Update()
    {
        if (viewModel == null)
            return;
        if (viewModel.simulationState.Value == MotionViewModel.SimulationState.running)
        {
            var currentPosition = viewModel.Update(Time.deltaTime);
        }
    }

    public virtual void Init(MotionViewModel motionViewModel)
    {
        disposables.Clear();

        viewModel = motionViewModel;
        //viewModel.paramsChanged += RebuildUI;

        viewModel.simulationState.Subscribe(_ => ViewModel_OnSimulationStateChanged()).AddTo(disposables);

        toggleSimulationButton.onClick.RemoveAllListeners();
        toggleSimulationButton.onClick.AddListener(OnToggleSimulationButtonClicked);

        stopSimulationButton.onClick.RemoveAllListeners();
        stopSimulationButton.onClick.AddListener(OnStopSimulationButtonClicked);

        RebuildUI();
    }


    protected virtual void OnStopSimulationButtonClicked()
    {
        viewModel.StopSimulation();
        Debug.Log("OnToggleSimulationButtonClicked");
    }

    protected virtual void OnToggleSimulationButtonClicked()
    {
        Debug.Log("OnToggleSimulationButtonClicked");
        if (viewModel.simulationState.Value == MotionViewModel.SimulationState.running)
            viewModel.PauseSimulation();
        else
            viewModel.StartSimulation();
    }

    protected virtual void ViewModel_OnSimulationStateChanged()
    {
        var state = viewModel.simulationState.Value;
        toggleSimulationButtonText.text = simulationButtonTexts[state];
    }
    
    protected virtual void RebuildUI(bool recreation = false)
    {
        ClearUI();
        CreateTopicFields(recreation);
    }
  
    protected virtual void CreateTopicFields(bool recreation = false)
    {
        var topicFields = viewModel.GetFields(recreation);
        foreach (var field in topicFields)
        {

            CreateTopicField(field.ParamName, field.Value);
        }
        viewModel.PropertyChanged += OnViewModel_PropertyChanged; 
    }

    protected virtual void OnViewModel_PropertyChanged(ParamName paramName, object newValue)
    {
        if (inputFields.TryGetValue(paramName, out var field))
        {
            field.SetValue(newValue);
            ViewModel_OnPropertyChanged(field, newValue);
        }
    }

    protected void CreateTopicField(ParamName paramName,object defaultValue)
    {
        TopicFieldController prefab;
        if (GetViewType(paramName, out ViewType viewType))
            switch (viewType)
            {
                case ViewType.inputField:
                default:
                    prefab = inputFieldPrefab;
                    break;
                case ViewType.Slider:
                    prefab = sliderPrefab;
                    break;
                case ViewType.Toggle:
                    prefab = toggleprefab;
                    break;
            }
        else
            prefab = inputFieldPrefab;

        var instance = Instantiate(prefab, inputFieldsContainer);
        switch (viewType)
        {
            case ViewType.inputField:
            default:
                InputFieldTopicFieldController prefabController = (InputFieldTopicFieldController)instance;
                prefabController.Setup(viewModel.IsReadOnly(paramName), paramName, viewModel.GetFieldType(paramName));
                break;
            case ViewType.Slider:
                SliderTopicFieldController sliderTopicFieldController = instance as SliderTopicFieldController;
                sliderTopicFieldController.Setup(viewModel.IsReadOnly(paramName), paramName, viewModel.GetFieldType(paramName), viewModel.GetMinValue(paramName), viewModel.GetMaxValue(paramName));
                break;
            case ViewType.Toggle:
                ToggleTopicFieldController toggleTopicFieldController = (ToggleTopicFieldController)instance;
                toggleTopicFieldController.Setup(viewModel.IsReadOnly(paramName), paramName);
                break;
        }
        instance.SetValue(defaultValue);
        instance.UserChangeTopicFieldValue += OnTopicFieldContoller_UserChangeTopicFieldValue;
        //uiDisposables.Add(subscription);
        inputFields[paramName] = instance;
    }

    protected virtual void OnTopicFieldContoller_UserChangeTopicFieldValue(string arg1, TopicFieldController controller)
    {
        Debug.Log(controller.ParamName + " changed ");
        if (!viewModel.OnUserChangeParam(controller.ParamName, arg1))
            return;

        if (ShouldRecalculateAfterUserChange(controller))
            viewModel.CalculatePosition();
    }

    protected virtual bool ShouldRecalculateAfterUserChange(TopicFieldController controller)
    {
        return viewModel.simulationState.Value == MotionViewModel.SimulationState.stoped && !controller.IsReadOnly;
    }
    private bool GetViewType(ParamName paramName, out ViewType viewType)
    {
        if(!topicFieldsViewTypes.TryGetValue(paramName, out viewType))
        {
            viewType = ViewType.inputField;
        }
        return true;
    }
    protected virtual void ViewModel_OnPropertyChanged(TopicFieldController topicFieldController, object newValue)
    {
        if (!topicFieldController.SetValue(newValue))
            Debug.Log("ViewModel_OnPropertyChanged went wrong");
    }
    private void ClearUI()
    {
        foreach (var d in uiDisposables)
            d.Dispose();
        uiDisposables.Clear();

        for (int i = 0; i < inputFieldsContainer.childCount; i++)
            Destroy(inputFieldsContainer.GetChild(i).gameObject);

        inputFields.Clear();
    }

    public virtual void OnDisabled()
    {
        DisableActionObjects();
        disposables.Clear();
    }

    public virtual void OnEnabled()
    {
        ActivateActionObjects();
        MoveCamera(CameraNewPosition, CameraRotation);
    }

    private void ActivateActionObjects()
    {
        foreach (GameObject obj in actionObjects)
        {
            obj.SetActive(true);
        }
    }
    private void DisableActionObjects()
    {
        foreach (GameObject obj in actionObjects)
        {
            obj.SetActive(false);
        }
    }
    public void MoveCamera(Vector3 mewPos, Vector3 newRot)
    {
        StartCoroutine(MoveToPositionAndRotate(mewPos, newRot, 1f));
    }

    private IEnumerator MoveToPositionAndRotate(Vector3 targetPos, Vector3 targetRot, float duration)
    {
        Vector3 startPos = _Camera.transform.position;
        Vector3 startRot = _Camera.transform.eulerAngles;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _Camera.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            _Camera.transform.eulerAngles = Vector3.Lerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            if(Time.deltaTime == 0)
            {
                elapsed = duration;
            }
            yield return null;
        }

        _Camera.transform.position = targetPos;
        _Camera.transform.eulerAngles = targetRot;
    }
}
