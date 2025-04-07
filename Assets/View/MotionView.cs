// MotionView.cs
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;

public class MotionView : MonoBehaviour
{
    [SerializeField] Transform MovingObject;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Button toggleSimulationButton;
    [SerializeField] Button stopSimulationButton;
    [SerializeField] TextMeshProUGUI toggleSimulationButtonText;
    [SerializeField] Transform inputFieldsContainer;
    [SerializeField] InputFieldController inputPrefab;

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
        if (viewModel == null) return;

        if (viewModel.simulationState.Value == MotionViewModel.SimulationState.running)
        {
            var currentPosition = viewModel.Update(Time.deltaTime);
            MovingObject.position = currentPosition;
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

    private void RebuildUI()
    {
        ClearUI();

        foreach (var pair in viewModel.Properties)
        {
            var paramName = pair.Key;
            var fieldType = viewModel.GetFieldType(paramName);
            var property = pair.Value;

            var input = Instantiate(inputPrefab, inputFieldsContainer);
            input.Setup(paramName, fieldType);
            input.BindProperty(property);

            if (viewModel.CurrentModel.Value.TopicFields.IsReadOnly(paramName))
                input.SetReadOnly(true);

            inputFields[paramName] = input;
        }
    }

    private void ClearUI()
    {
        foreach (var input in inputFields.Values)
            Destroy(input.gameObject);

        inputFields.Clear();
    }
}
