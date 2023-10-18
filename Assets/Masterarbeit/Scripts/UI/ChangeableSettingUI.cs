using UnityEngine;

public class ChangeableSettingUI : MonoBehaviour
{
    public SettingType setting;

    protected virtual void Start()
    {
        SaveSystem.onSettingsReset += OnSettingsReset;
        Default();
    }
    protected virtual void OnDestroy()
    {
        SaveSystem.onSettingsReset -= OnSettingsReset;
    }

    protected void OnSettingsReset()
    {
        Default();
    }

    public virtual void Default()
    {
    }

    protected virtual void OnDecrementButtonClicked()
    {
    }

    protected virtual void OnIncrementButtonClicked()
    {
    }

}