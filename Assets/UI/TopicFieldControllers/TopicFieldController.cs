using System;
using TMPro;
using UnityEngine;

public abstract class TopicFieldController : MonoBehaviour
{
    public Action<string, TopicFieldController> UserChangeTopicFieldValue;
    public Action<string> TopicFieldEndEdited;
    public bool IsInvokeOnValueChanged;
    [SerializeField] TextMeshProUGUI label;
    public bool IsReadOnly;
    public ParamName ParamName;
    private int userChangeNotificationSuppressionDepth;

    protected bool IsUserChangeNotificationSuppressed => userChangeNotificationSuppressionDepth > 0;

    protected void SuppressUserChangeNotification(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        userChangeNotificationSuppressionDepth++;
        try
        {
            action();
        }
        finally
        {
            userChangeNotificationSuppressionDepth = Math.Max(0, userChangeNotificationSuppressionDepth - 1);
        }
    }

    protected void RaiseUserValueChanged(string newValue)
    {
        if (IsUserChangeNotificationSuppressed)
            return;

        UserChangeTopicFieldValue?.Invoke(newValue, this);
    }

    protected virtual void Start()
    {
        
    }
    private void SetLabel(string newText)
    {
        label.text = newText;
    }

    public void Setup(bool isReadOnly, ParamName paramName, string defaultValue = "enter property")
    {
        this.IsReadOnly = isReadOnly;
        this.ParamName = paramName;
        Debug.Log(" Setup  " + GetType().ToString() + paramName.ToString());
        SetReadOnly(isReadOnly);
        SetLabel(paramName.ToString());
    }

    public abstract bool SetValue(object newValue);

    protected abstract void SetReadOnly(bool value);

    protected abstract string GetText();
}
