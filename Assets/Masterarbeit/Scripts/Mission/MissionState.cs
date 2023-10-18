using UnityEngine;

public enum MissionStateType
{
    NONE,
    PREPARATION,
    RUNNING,
    ENDING,
    ENDED,
    Count
}
public class MissionState
{
    public event System.Action onShouldExit;

    protected bool shouldForceExit;

    protected float stateTime = -1.0f;
    protected float remainingStateTime = -1.0f;

    public float GetStateTime()
    {
        return stateTime;
    }
    public void SetStateTime(float time)
    {
        stateTime = time;
        remainingStateTime = time;
    }

    public virtual bool DidStateTimeEnd()
    {
        return remainingStateTime < 0.0f;
    }

    public float GetRemainingStateTime()
    {
        return remainingStateTime;
    }

    public virtual MissionStateType GetMissionStateType()
    {
        return MissionStateType.NONE;
    }
    public virtual MissionStateType GetNextMissionStateType()
    {
        return MissionStateType.NONE;
    }

    public virtual void OnEnter()
    {

    }
    public virtual void OnExit()
    {

    }
    public virtual bool ShouldExit()
    {
        return shouldForceExit || DidStateTimeEnd();
    }

    public virtual void Update()
    {
        UpdateStateTime();
        if(ShouldExit() && onShouldExit != null)
        {
            onShouldExit.Invoke();
        }
    }
    protected virtual void UpdateStateTime()
    {
        remainingStateTime -= Time.deltaTime;
    }

    public void ForceExit()
    {
        shouldForceExit = true;
    }
}
