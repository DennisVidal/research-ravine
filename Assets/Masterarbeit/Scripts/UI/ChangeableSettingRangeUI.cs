using UnityEngine.UI;

public class ChangeableSettingRangeUI : ChangeableSettingUI
{
    public Button decrementButton;
    public Button incrementButton;

    public TMPro.TextMeshProUGUI valueText;
    public string valueUnit;

    protected override void Start()
    {
        decrementButton.onClick.AddListener(OnDecrementButtonClicked);
        incrementButton.onClick.AddListener(OnIncrementButtonClicked);
        base.Start();
    }
    protected override void OnDestroy()
    {
        if (decrementButton != null)
        {
            decrementButton.onClick.RemoveListener(OnDecrementButtonClicked);
        }
        if (incrementButton != null)
        {
            incrementButton.onClick.RemoveListener(OnIncrementButtonClicked);
        }
        base.OnDestroy();
    }

    protected virtual void OnDecrementButtonClicked()
    {
    }

    protected virtual void OnIncrementButtonClicked()
    {
    }

}
