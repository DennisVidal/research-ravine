using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStateRunning : MissionState
{
    public override void OnExit()
    {
        base.OnExit();

        if(GameManager.Instance.GetPlayerStatus() == PlayerStatus.ALIVE)
        {
            GameManager.Instance.SetPlayerStatus(PlayerStatus.RESCUED);
        }
    }

    public override MissionStateType GetMissionStateType()
    {
        return MissionStateType.RUNNING;
    }
    public override MissionStateType GetNextMissionStateType()
    {
        return MissionStateType.ENDED;
    }

    protected override void UpdateStateTime()
    {
        if(!GameManager.Instance.isGameplayPaused)
        {
            base.UpdateStateTime();
        }
    }
}
