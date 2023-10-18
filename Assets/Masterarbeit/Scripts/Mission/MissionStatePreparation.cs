using UnityEngine;

public class MissionStatePreparation : MissionState
{
    protected bool isTimerRunning = false;
    protected float preparationTime = 3.0f;
    protected float remainingPreparationTime = 3.0f;

    public override MissionStateType GetMissionStateType()
    {
        return MissionStateType.PREPARATION;
    }
    public override MissionStateType GetNextMissionStateType()
    {
        return MissionStateType.RUNNING;
    }

    public override void OnEnter()
    {
        InputManager.Instance.onButtonPressed += OnButtonPressed;
    }
    public override void OnExit()
    {
        InputManager.Instance.onButtonPressed -= OnButtonPressed;
    }

    protected override void UpdateStateTime()
    {
        if (isTimerRunning)
        {
            base.UpdateStateTime();
        }
    }
    public override bool DidStateTimeEnd()
    {
        return isTimerRunning && base.DidStateTimeEnd();
    }

    protected void OnButtonPressed(int button)
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            remainingPreparationTime = preparationTime;
        }
    }
}
