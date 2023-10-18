using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeableSettingIntUI : ChangeableSettingRangeUI
{
    public int currentValue;
    public int minValue;
    public int maxValue;
    public int valueIncrement;

    public UnityEvent<int> onValueChanged = new UnityEvent<int>();

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
        SetValue(SaveSystem.SAVE_DATA.GetSettingInt(setting));
    }

    public int GetValue()
    {
        return currentValue;
    }

    public void SetValue(int value)
    {
        currentValue = value;

        string unitStr = valueUnit.Length > 0 ? " " + valueUnit : "";
        valueText.text = currentValue + unitStr;

        onValueChanged.Invoke(currentValue);
    }
}
