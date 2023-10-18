using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeableSettingFloatUI : ChangeableSettingRangeUI
{
    public float currentValue;
    public float minValue;
    public float maxValue;
    public float valueIncrement;

    public UnityEvent<float> onValueChanged = new UnityEvent<float>();

    protected override void OnDecrementButtonClicked()
    {
        SetValue(Mathf.Clamp(GetValue() - valueIncrement, minValue, maxValue));
    }

    protected override void OnIncrementButtonClicked()
    {
        SetValue(Mathf.Clamp(GetValue() + valueIncrement, minValue, maxValue));
    }

    public override void Default()
    {
        SetValue(SaveSystem.SAVE_DATA.GetSettingFloat(setting));
    }

    public float GetValue()
    {
        return currentValue;
    }

    public void SetValue(float value)
    {
        currentValue = value;
        string unitStr = valueUnit.Length > 0 ? " " + valueUnit : "";
        valueText.text = currentValue.ToString("n2") + unitStr;

        onValueChanged.Invoke(currentValue);
    }
}
