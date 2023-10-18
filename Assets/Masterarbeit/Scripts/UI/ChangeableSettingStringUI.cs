using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeableSettingStringUI : ChangeableSettingRangeUI
{
    public int currentValue;
    public List<string> strings;

    public UnityEvent<int> onValueChanged = new UnityEvent<int>();

    protected override void OnDecrementButtonClicked()
    {
        SetValue(Mathf.Clamp(GetValue() - 1, 0, strings.Count - 1));
    }

    protected override void OnIncrementButtonClicked()
    {
        SetValue(Mathf.Clamp(GetValue() + 1, 0, strings.Count - 1));
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
        valueText.text = strings[currentValue];

        onValueChanged.Invoke(currentValue);
    }
}
