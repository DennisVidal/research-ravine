using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventData<T0>
{
    public T0 param0;

    public EventData(T0 param0)
    {
        this.param0 = param0;
    }
}

public class TurnSpeedBooster : MonoBehaviour
{
    Vehicle vehicle;

    public int boosterButton = 1;

    //public float multiplier = 2.0f;
    [System.NonSerialized] bool isBoosterEnabled = false;

    [System.NonSerialized] public float currentBoosterCharge = 0.0f;
    //public float maxBoosterCharge = 5.0f;
    public float boosterDrainingRate = 1.0f;
    //public float boosterRechargeRate = 0.5f;


    [System.NonSerialized] public bool updateBoosterCharge = true;

    void Start()
    {
        currentBoosterCharge = GetMaxCharge();
        SetVehicle(GetComponent<Vehicle>());

        InputManager.Instance.onButtonPressed += OnButtonPressed;
        InputManager.Instance.onButtonReleased += OnButtonReleased;

        GameManager.Instance.onGameplayPaused += OnGameplayPaused;
        GameManager.Instance.onGameplayResumed += OnGameplayResumed;
    }

    void OnDestroy()
    {
        SetVehicle(null);

        if (InputManager.Instance != null)
        {
            InputManager.Instance.onButtonPressed -= OnButtonPressed;
            InputManager.Instance.onButtonReleased -= OnButtonReleased;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameplayPaused -= OnGameplayPaused;
            GameManager.Instance.onGameplayResumed -= OnGameplayResumed;
        }
    }

    void Update()
    {
        float newBoosterCharge = currentBoosterCharge + GetRate();
        if(newBoosterCharge < 0.0f)
        {
            isBoosterEnabled = false;
        }
        currentBoosterCharge = Mathf.Clamp(newBoosterCharge, 0.0f, GetMaxCharge());
    }

    void OnGameplayPaused()
    {
        updateBoosterCharge = false;
    }
    void OnGameplayResumed()
    {
        updateBoosterCharge = true;
    }

    void OnUpdateTurnSpeedModifier(EventData<float> eventData)
    {
        eventData.param0 *= isBoosterEnabled ? GetTurnSpeedMultiplier() : 1.0f;
    }

    void OnButtonPressed(int button)
    {
        if(button != boosterButton)
        {
            return;
        }

        isBoosterEnabled = true;
    }
    void OnButtonReleased(int button, float duration)
    {
        if (button != boosterButton)
        {
            return;
        }

        isBoosterEnabled = false;
    }
    float GetRate()
    {
        if (!updateBoosterCharge)
        {
            return 0.0f;
        }

        return Time.deltaTime * (isBoosterEnabled ? -boosterDrainingRate : GetRechargeRate());
    }

    public void SetVehicle(Vehicle v)
    {
        if (vehicle != null)
        {
            vehicle.onUpdateTurnSpeedModifier -= OnUpdateTurnSpeedModifier;
        }

        vehicle = v;
        if (vehicle != null)
        {
            vehicle.onUpdateTurnSpeedModifier += OnUpdateTurnSpeedModifier;
        }
    }

    public float GetNormalizedCharge()
    {
        return Mathf.Clamp01(currentBoosterCharge / GetMaxCharge());
    }


    public float GetMaxCharge()
    {
        return GameManager.Instance.GetSelectedDifficultyData().turnSpeedBoosterMaxCharge;
    }
    public float GetRechargeRate()
    {
        return GameManager.Instance.GetSelectedDifficultyData().turnSpeedBoosterRechargeRate;
    }
    public float GetTurnSpeedMultiplier()
    {
        return GameManager.Instance.GetSelectedDifficultyData().turnSpeedBoosterMultiplier;
    }
}
