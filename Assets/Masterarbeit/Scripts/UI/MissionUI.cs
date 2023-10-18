using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUI : MonoBehaviour
{
    public static float SECONDS_TO_MINUTES = 1.0f / 60.0f;
    public TMPro.TextMeshProUGUI missionInfoDisplayText;

    Coroutine currentMissionInfoCoroutine;

    Mission currentMission;

    void Start()
    {
        currentMission = GameManager.Instance.currentMission;
        currentMission.onSwitchingMissionState += OnSwitchingMissionState;
    }
    void OnDestroy()
    {
        currentMission.onSwitchingMissionState -= OnSwitchingMissionState;
        StopAllCoroutines();
    }


    void OnSwitchingMissionState(MissionState oldState, MissionState newState)
    {
        if(newState == null)
        {
            return;
        }

        switch (newState.GetMissionStateType())
        {
            case MissionStateType.PREPARATION:
                currentMissionInfoCoroutine = StartCoroutine(UpdateMissionPreparationInfoCoroutine());
                break;
            case MissionStateType.RUNNING:
                currentMissionInfoCoroutine = StartCoroutine(UpdateMissionRunningInfoCoroutine());
                break;
        }
    }



    public void StopCurrentMissionInfoCoroutine()
    {
        if (currentMissionInfoCoroutine != null)
        {
            StopCoroutine(currentMissionInfoCoroutine);
        }
    }

    IEnumerator UpdateMissionPreparationInfoCoroutine()
    {
        while (currentMission.GetCurrentMissionStateType() == MissionStateType.PREPARATION)
        {
            missionInfoDisplayText.text = ((int)currentMission.GetCurrentMissionState().GetRemainingStateTime() + 1).ToString();
            yield return null;
        }
        missionInfoDisplayText.text = "";
    }
    IEnumerator UpdateMissionRunningInfoCoroutine()
    {
        while (currentMission.GetCurrentMissionStateType() == MissionStateType.RUNNING)
        {
            missionInfoDisplayText.text = GetFormatedTimeString(currentMission.GetCurrentMissionState().GetRemainingStateTime());
            yield return null;
        }
        missionInfoDisplayText.text = "";
    }

    protected string GetFormatedTimeString(float time)
    {
        float minutes = (int)(time * SECONDS_TO_MINUTES);
        float seconds = (int)time - minutes * 60;
        return (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
    }
}
