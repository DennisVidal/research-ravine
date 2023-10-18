using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMissionMenu : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.currentMission.onSwitchingMissionState += OnSwitchingMissionState;
        InputManager.Instance.onButtonPressed += OnButtonPressed;
    }

    void OnDestroy()
    {
        GameManager.Instance.currentMission.onSwitchingMissionState -= OnSwitchingMissionState;
        InputManager.Instance.onButtonPressed -= OnButtonPressed;
    }

    public void OnButtonPressed(int button)
    {
        gameObject.SetActive(false);
    }
    void OnSwitchingMissionState(MissionState oldState, MissionState newState)
    {
        if (newState != null && newState.GetMissionStateType() == MissionStateType.PREPARATION)
        {
            gameObject.SetActive(true);
        }
    }
}
