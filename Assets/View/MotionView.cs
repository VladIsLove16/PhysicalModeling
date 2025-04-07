using UnityEngine.UI;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;

public class MotionView : MonoBehaviour
{
    private const int DECIMALCOUNT = 4;
    [SerializeField] private Transform MovingObject;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button toggleSimulationButton;
    [SerializeField] private Button stopSimulationButton;
    [SerializeField] private TextMeshProUGUI toggleSimulationButtonText;
    [SerializeField] private Transform inputFieldsContainer;
    [SerializeField] private InputFieldController inputPrefab;


    private MotionViewModel viewModel;
    private Dictionary<ParamName, InputFieldController> inputFields = new();

    private readonly Dictionary<MotionViewModel.SimulationState, string> simButtonTexts = new()
    {
        { MotionViewModel.SimulationState.running, "Пауза" },
        { MotionViewModel.SimulationState.paused, "Продолжить" },
        { MotionViewModel.SimulationState.stoped, "Старт" },
    };

    private void Update()
    {
        if (viewModel == null)
            return;
        if (viewModel.simulationState.Value == MotionViewModel.SimulationState.running)
        {
            var currentPosition = viewModel.Update(Time.deltaTime);
        }
    }

    public void Init(MotionViewModel motionViewModel)
    {
        viewModel = motionViewModel;
        viewModel.CurrentModel.Subscribe(_ => RebuildUI()).AddTo(this);
        viewModel.simulationState.Subscribe(_ => UpdateSimulationState()).AddTo(this);

        toggleSimulationButton.onClick.AddListener(OnToggleSimulationButtonClicked);
        stopSimulationButton.onClick.AddListener(OnStopSimulationButtonClicked);

        titleText.text = viewModel.Title.Value;
        RebuildUI();
    }

    private void OnStopSimulationButtonClicked() => viewModel.StopSimulation();

    private void OnToggleSimulationButtonClicked()
    {
        if (viewModel.simulationState.Value == MotionViewModel.SimulationState.running)
            viewModel.PauseSimulation();
        else
            viewModel.StartSimulation();
    }

    private void UpdateSimulationState()
    {
        var state = viewModel.simulationState.Value;
        toggleSimulationButtonText.text = simButtonTexts[state];
    }
    private string GetStringValue(ParamName paramName)
    {
        var obj = viewModel.GetParam(paramName);
        string valueText = obj switch
        {
            float floatValue => floatValue.ToString("0.00"),
            int intValue => intValue.ToString(),
            Vector3 v => $"{v.x.ToString("0.00")};{v.y.ToString("0.00")};{v.z.ToString("0.0000")}",
            string stringValue => stringValue,
            _ => obj.ToString()
        };
        return valueText;
    }

    
    private void RebuildUI()
    {
        ClearUI();

        foreach (var pair in viewModel.Properties)
        {
            var paramName = pair.Key;
            var fieldType = viewModel.GetFieldType(paramName);
            var property = pair.Value;

            var inputFieldController = Instantiate(inputPrefab, inputFieldsContainer);
            inputFieldController.Setup(paramName, fieldType,GetStringValue(paramName));
            if (viewModel.CurrentModel.Value.TopicFields.IsReadOnly(paramName))
                inputFieldController.SetReadOnly(true);
            property.Subscribe(value =>OnViewModelPositionChanged(inputFieldController, value));
            inputFields[paramName] = inputFieldController;
            inputFieldController.OnInputFieldTextChanged += value => InputFieldController_OnInputFieldTextChanged(inputFieldController, value);
            inputFieldController.OnInputFieldEndEdited += value => InputFieldController_OnInputFieldEndEdited(inputFieldController, value);
        }
    }

    private void OnViewModelPositionChanged(InputFieldController inputFieldController, object newValue)
    {
        inputFieldController.SetText(GetStringValue(inputFieldController.ParamName));
        if(inputFieldController.ParamName == ParamName.position)
        {
            MovingObject.position =(Vector3) newValue;
        }
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
        foreach (var input in inputFields.Values)
            Destroy(input.gameObject);

        inputFields.Clear();
    }
}