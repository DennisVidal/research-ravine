using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeableSettingBoolUI : ChangeableSettingUI
{
    public bool currentValue;
    public Toggle toggleButton;

    public UnityEvent<bool> onValueChanged = new UnityEvent<bool>();


    protected override void Start()
    {
        toggleButton.onValueChanged.AddListener(OnToggle);
        base.Start();
    }

    protected override void OnDestroy()
    {
        if(toggleButton != null)
        {
            toggleButton.onValueChanged.RemoveListener(OnToggle);
        }
        base.OnDestroy();
    }

    protected void OnToggle(bool enabled)
    {
        SetValue(!GetValue());
    }

    public override void Default()
    {
        SetValue(SaveSystem.SAVE_DATA.GetSettingBool(setting));
    }

    public bool GetValue()
    {
        return currentValue;
    }

    public void SetValue(bool value)
    {
        currentValue = value;
        toggleButton.isOn = value;

        onValueChanged.Invoke(currentValue);
    }
}

