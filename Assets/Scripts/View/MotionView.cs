using UnityEngine.UI;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public abstract class MotionView : MonoBehaviour
{
    [SerializeField] private Button toggleSimulationButton;
    [SerializeField] private Button stopSimulationButton;
    [SerializeField] private TextMeshProUGUI toggleSimulationButtonText;
    [SerializeField] private Transform inputFieldsContainer;
    [SerializeField] private InputFieldController inputPrefab;
    [SerializeField] private Slider sliderPrefab;
    [SerializeField] private Toggle toggleprefab;
    [SerializeField] protected Camera _Camera;
    [SerializeField] protected Vector3 CameraNewPosition;
    [SerializeField] protected Vector3 CameraRotation;
    protected MotionViewModel viewModel;
    private Dictionary<ParamName, InputFieldController> inputFields = new();

    private CompositeDisposable disposables = new();
    private List<IDisposable> uiDisposables = new();

    private readonly Dictionary<MotionViewModel.SimulationState, string> simulationButtonTexts = new()
    {
        { MotionViewModel.SimulationState.running, "Пауза" },
        { MotionViewModel.SimulationState.paused, "Продолжить" },
        { MotionViewModel.SimulationState.stoped, "Старт" },
    };
    protected virtual void Start()
    {

    }
   
    private void Update()
    {
        if (viewModel == null)
            return;
        if (viewModel.simulationStateChanged.Value == MotionViewModel.SimulationState.running)
        {
            var currentPosition = viewModel.Update(Time.deltaTime);
        }
    }

    public virtual void Init(MotionViewModel motionViewModel)
    {
        disposables.Clear();

        viewModel = motionViewModel;

        viewModel.simulationStateChanged.Subscribe(_ => ViewModel_OnSimulationStateChanged()).AddTo(disposables);

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
        if (viewModel.simulationStateChanged.Value == MotionViewModel.SimulationState.running)
            viewModel.PauseSimulation();
        else
            viewModel.StartSimulation();
    }

    protected virtual void ViewModel_OnSimulationStateChanged()
    {
        var state = viewModel.simulationStateChanged.Value;
        toggleSimulationButtonText.text = simulationButtonTexts[state];
        Debug.Log("MotionView +  Simulation state Setted " + state);
    }
    private string GetStringValue(ParamName paramName)
    {
        var obj = viewModel.GetParam(paramName);
        string valueText = obj switch
        {
            float floatValue => floatValue.ToString("0.00"),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x.ToString("0.00")};{v.y.ToString("0.00")};{v.z.ToString("0.000")}",
            string stringValue => stringValue,
            _ => "not avaialable type"
        };
        return valueText;
    }
    
    protected virtual void RebuildUI()
    {
        ClearUI();
        CreateInputFields();
    }

    private void CreateInputFields()
    {
        foreach (var pair in viewModel.GetProperties())
        {
             CreateInputField(pair);
        }
    }

    private void  CreateInputField(KeyValuePair<ParamName, ReactiveProperty<object>> pair)
    {
        var paramName = pair.Key;
        var fieldType = viewModel.GetFieldType(paramName);
        var property = pair.Value;

        Debug.Log("Instantiate");
        var inputFieldController = Instantiate(inputPrefab, inputFieldsContainer);
        inputFieldController.Setup(paramName, fieldType, viewModel.IsReadonly(paramName));
        var subscription = property.Subscribe(value => ViewModel_OnPropertyChanged(inputFieldController, value));
        uiDisposables.Add(subscription);
        inputFields[paramName] = inputFieldController;
        inputFieldController.OnInputFieldTextChanged += value => InputFieldController_OnInputFieldTextChanged(inputFieldController, value);
        inputFieldController.OnInputFieldEndEdited += value => InputFieldController_OnInputFieldEndEdited(inputFieldController, value);
    }

    protected virtual void ViewModel_OnPropertyChanged(InputFieldController inputFieldController, object newValue)
    {
        inputFieldController.SetText(GetStringValue(inputFieldController.ParamName));
        
    }

    private void InputFieldController_OnInputFieldTextChanged(InputFieldController controller, string obj)
    {
        bool res = viewModel.SetParam(controller.ParamName, obj);
    }

    private void InputFieldController_OnInputFieldEndEdited(InputFieldController controller, string obj)
    {
        bool res = viewModel.SetParam(controller.ParamName, obj);
        if (!res)
        {
            controller.SetText(GetStringValue(controller.ParamName));
        }
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
         disposables.Clear();
    }

    public virtual void OnEnabled()
    {
        MoveCamera(CameraNewPosition, CameraRotation);
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
            yield return null;
        }

        _Camera.transform.position = targetPos;
        _Camera.transform.eulerAngles = targetRot;
    }
}
