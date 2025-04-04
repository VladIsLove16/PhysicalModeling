using UnityEngine.UI;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;

public class MotionView : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button toggleSimulationButton;
    public TextMeshProUGUI toggleSimulationButtonText;
    public Transform inputFieldsContainer;
    public InputFieldController inputPrefab;
    private MotionViewModel viewModel;
    private Dictionary<ParamName, InputFieldController> inputFields = new();
    public void Init(MotionViewModel motionViewModel)
    {
        viewModel = motionViewModel;
        viewModel.CurrentModel.Subscribe(_ => RedrawUI());
        RedrawUI();
        titleText.text = viewModel.Title.Value;
        toggleSimulationButton.onClick.AddListener(OnToggleSimulationButtonClicked);
        viewModel.isSimulating.Subscribe(_ => OnSimulatingChanged());
        
    }

    private void OnToggleSimulationButtonClicked()
    {
        if (viewModel.isSimulating.Value)
        {
            viewModel.StopSimulation();
        }
        else
            viewModel.StartSimulation();
    }
    private void OnSimulatingChanged()
    {
        if (viewModel.isSimulating.Value)
            toggleSimulationButtonText.text = "Стоп";
        else
            toggleSimulationButtonText.text = "Старт";
    }
    void RedrawUI()
    {
        foreach (var obj in inputFields.Values)
            Destroy(obj.gameObject);
        inputFields.Clear();

        foreach (var pair in viewModel.Properties)
        {
            FieldType fieldType = viewModel.GetFieldType(pair.Key);
            ParamName paramName= pair.Key;
            ReactiveProperty<object> property = pair.Value;

            InputFieldController inputFieldController = Instantiate(inputPrefab, inputFieldsContainer);
            inputFields[paramName] = inputFieldController;
            inputFieldController.Setup(paramName, fieldType);
            inputFieldController.inputField.text = GetValue(property);
            //property.Subscribe(newValue => OnViewModelParamChanged(property, inputFieldController));
            inputFieldController.inputField.onValueChanged.AddListener(value => { Debug.Log("new value" + value); property.SetValueAndForceNotify (inputFieldController.GetValue()); });
        }
    }
    private void OnViewModelParamChanged(ReactiveProperty<object> reactiveProperty, InputFieldController inputFieldController)
    {
        inputFieldController.inputField.text = GetValue(reactiveProperty);
    }
    private string GetValue(ReactiveProperty<object> reactiveProperty)
    {
        string valueText = reactiveProperty.Value switch
        {
            float floatValue => floatValue.ToString(),
            int intValue => intValue.ToString(),
            Vector3 vectorValue => vectorValue.ToString("F2"),
            _ => reactiveProperty.Value.ToString()
        };
        return valueText;
    }
    public  FieldType GetFieldType(object value)
    {
        return viewModel.GetFieldType(value);
    }
    private void CreateUIForVector()
    {

    }
}