using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionOverUI : MonoBehaviour
{
    public Image image;
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI scoreText;


    void Start()
    {
        GameManager.Instance.currentMission.onSwitchingMissionState += OnSwitchingMissionState;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        GameManager.Instance.currentMission.onSwitchingMissionState -= OnSwitchingMissionState;
    }

    void OnSwitchingMissionState(MissionState oldState, MissionState newState)
    {
        if(newState == null || newState.GetMissionStateType() != MissionStateType.ENDED)
        {
            return;
        }

        gameObject.SetActive(true);

        PlayerStatus status = GameManager.Instance.GetPlayerStatus();
        text.text = GetStatusText(status);
        text.color = GetStatusColor(status);
        scoreText.text = "Score: " + GameManager.Instance.GetScore();

    }
    string GetStatusText(PlayerStatus status)
    {
        string t = "YOU\n";
        switch (status)
        {
            case PlayerStatus.RESCUED:
                t += "GOT RESCUED";
                break;
            case PlayerStatus.CRASHED:
                t += "CRASHED";
                break;
            case PlayerStatus.CRUSHED:
                t += "GOT CRUSHED";
                break;
            default:
                t += "DID SOMETHING";
                break;
        }
        return t;
    }
    Color GetStatusColor(PlayerStatus status)
    {
        if (status == PlayerStatus.RESCUED)
        {
            return Color.green;
        }
        return Color.red;
    }
}
