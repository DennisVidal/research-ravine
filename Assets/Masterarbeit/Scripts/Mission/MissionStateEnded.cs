using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStateEnded : MissionState
{
    public override MissionStateType GetMissionStateType()
    {
        return MissionStateType.ENDED;
    }
}
