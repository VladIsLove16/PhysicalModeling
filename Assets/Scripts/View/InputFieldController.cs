using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TMP_InputField inputField;
    public Action<string> OnInputFieldTextChanged;
    public Action<string> OnInputFieldEndEdited;
    public bool IsInvokeOnTextChanged;
    private FieldType fieldType;
    public ParamName ParamName;
    private string previousValue;
    private void Start()
    {
        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener(_ => OnInputFieldEndEdit());
        inputField.onValueChanged.AddListener(OnTextChanged);
        inputField.onSelect.AddListener(OnSelect);
    }

    private void OnSelect(string arg0)
    {
        previousValue = arg0;
    }

    public void Setup(ParamName ParametrName, FieldType type,bool isReadonly = false, string defaultValue = "enter property")
    {
        ParamName = ParametrName;
        label.text = ParametrName.ToString();
        fieldType = type;
        SetReadOnly(isReadonly);
        SetText(defaultValue);
        switch (fieldType)
        {
            case FieldType.Float:
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                break;
            case FieldType.Int:
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                break;
            case FieldType.Vector3:
                inputField.contentType = TMP_InputField.ContentType.Standard;
                break;
        }
    }
    public void SetReadOnly(bool value)
    {
        inputField.interactable = !value;
    }

    
    internal void SetText(string v)
    {
        inputField.text = v;
    }

    private void OnTextChanged(string newValue)
    {
        if (!IsInvokeOnTextChanged)
            return;
        OnInputFieldTextChanged.Invoke(newValue);
    }

    private void OnInputFieldEndEdit()
    {
         OnInputFieldEndEdited.Invoke(GetText());

    }
    private string GetText()
    {
        return inputField.text;
    }


}