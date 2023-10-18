using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Mission : MonoBehaviour
{
    public event Action onMissionStarted;
    public event Action onMissionEnded;
    public event Action<MissionState, MissionState> onSwitchingMissionState;

    protected MissionState currentMissionState;

    public float preparationTime = 3.0f;
    public float endedTime = 5.0f;

    protected Coroutine updateCoroutine;


    public void StartMission()
    {
        StartUpdateCoroutine();

        if (onMissionStarted != null)
        {
            onMissionStarted.Invoke();
        }
    }
    public void StopMission()
    {
        StopUpdateCoroutine();
    }

    protected void StartUpdateCoroutine()
    {
        StopUpdateCoroutine();
        updateCoroutine = StartCoroutine(UpdateCoroutine());
    }
    protected void StopUpdateCoroutine()
    {
        if(updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        currentMissionState = null;
    }
    protected IEnumerator UpdateCoroutine()
    {
        SwitchMissionState(MissionStateType.PREPARATION);

        while(currentMissionState != null)
        {
            currentMissionState.Update();
            yield return null;
        }

        if(onMissionEnded != null)
        {
            onMissionEnded.Invoke();
        }
    }

    protected void SwitchMissionState(MissionStateType newType)
    {
        if (currentMissionState != null)
        {
            currentMissionState.OnExit();
            currentMissionState.onShouldExit -= OnShouldExitMissionState;
        }
        MissionState oldState = currentMissionState;
        currentMissionState = CreateMissionState(newType);
        if (currentMissionState != null)
        {
            currentMissionState.OnEnter();
            currentMissionState.onShouldExit += OnShouldExitMissionState;
        }

        if (onSwitchingMissionState != null)
        {
            onSwitchingMissionState.Invoke(oldState, currentMissionState);
        }
    }
    protected MissionState CreateMissionState(MissionStateType type)
    {
        MissionState state = null;
        switch (type)
        {
            case MissionStateType.PREPARATION:
                MissionStatePreparation preparationState = new MissionStatePreparation();
                preparationState.SetStateTime(preparationTime);
                state = preparationState;
                break;

            case MissionStateType.RUNNING:
                MissionStateRunning runningState = new MissionStateRunning();
                runningState.SetStateTime(GetMissionTime());
                state = runningState;
                break;

            case MissionStateType.ENDED:
                MissionStateEnded endedState = new MissionStateEnded();
                endedState.SetStateTime(GetEndedTime());
                state = endedState;
                break;
        }
        return state;
    }
    protected void OnShouldExitMissionState()
    {
        SwitchMissionState(currentMissionState.GetNextMissionStateType());
    }


    public float GetPreparationTime()
    {
        return preparationTime;
    }
    public float GetMissionTime()
    {
        return SaveSystem.SAVE_DATA.missionTime * 60.0f;
    }
   
    public float GetEndedTime()
    {
        return endedTime;
    }
    public MissionState GetCurrentMissionState()
    {
        return currentMissionState;
    }
    public MissionStateType GetCurrentMissionStateType()
    {
        return currentMissionState != null ? currentMissionState.GetMissionStateType() : MissionStateType.NONE;
    }

}





//public enum MissionState
//{
//    NONE,
//    PREPARATION,
//    RUNNING,
//    ENDING,
//    FAILING,
//    FAILED,
//    PAUSED,
//    DONE
//}

//public enum MissionType
//{
//    NONE,
//    DEFAULT
//}

//public enum FailingReason
//{
//    CRASHED,
//    GOT_CRUSHED
//}

//public class Mission : MonoBehaviour
//{
//    public event Action onStartMission;
//    public event Action onEndMission;
//    public event Action onAbortMission;
//    public event Action<MissionState, MissionState> onLeavingState;
//    public event Action<MissionState, MissionState> onEnteringState;

//    public MissionState lastMissionState;
//    public MissionState missionState;


//    public Timer preparationTimer;
//    public Timer missionTimer;
//    public Timer failingTimer;
//    public Timer failedTimer;
//    public Timer winningTimer;
//    public Timer wonTimer;

//    int initialFadeTime;

//    public FailingReason failingReason;

//    void Start()
//    {
//        preparationTimer.onEndTimer += OnPreparartionTimerEnded;
//        missionTimer.onEndTimer += OnMissionTimerEnded;
//        failingTimer.onEndTimer += OnFailingTimerEnded;
//        failedTimer.onEndTimer += OnFailedTimerEnded;
//        winningTimer.onEndTimer += OnWinningTimerEnded;
//        wonTimer.onEndTimer += OnWonTimerEnded;

//        GameManager.Instance.onGameplayPaused += OnGameplayPaused;
//        GameManager.Instance.onGameplayResumed += OnGameplayResumed;

//        initialFadeTime = failingTimer.initialTime;
//    }


//    public int GetScore()
//    {
//        return 1110;
//    }

//    void OnGameplayPaused()
//    {
//        //if (missionState == MissionState.PREPARATION || missionState == MissionState.RUNNING)
//        if (missionState == MissionState.RUNNING)
//        {
//            SwitchMissionState(MissionState.PAUSED);
//        }
//    }
//    void OnGameplayResumed()
//    {
//        SwitchMissionState(lastMissionState);
//    }

//    public void ResetMission()
//    {
//        lastMissionState = MissionState.NONE;
//        missionState = MissionState.NONE;
//        preparationTimer.ResetTimer();
//        missionTimer.ResetTimer();
//        failingTimer.ResetTimer();
//        failedTimer.ResetTimer();
//        winningTimer.ResetTimer();
//        wonTimer.ResetTimer();
//    }

//    void OnPreparartionTimerEnded()
//    {
//        SwitchMissionState(MissionState.RUNNING);
//    }
//    void OnMissionTimerEnded()
//    {
//        if(missionState == MissionState.RUNNING)
//        {
//            SwitchMissionState(MissionState.ENDING);
//        }
//    }
//    void OnFailingTimerEnded()
//    {
//        SwitchMissionState(MissionState.FAILED);
//    }
//    void OnFailedTimerEnded()
//    {
//        EndMission();
//    }
//    void OnWinningTimerEnded()
//    {
//        SwitchMissionState(MissionState.DONE);
//    }
//    void OnWonTimerEnded()
//    {
//        EndMission();
//    }

//    public MissionType GetMissionType()
//    {
//        return MissionType.NONE;
//    }
//    public MissionState GetMissionState()
//    {
//        return missionState;
//    }


//    public void StartMission()
//    {
//        SwitchMissionState(MissionState.PREPARATION);

//        if (onStartMission != null)
//        {
//            onStartMission.Invoke();
//        }
//    }

//    public void EndMission()
//    {
//        if (onEndMission != null)
//        {
//            onEndMission.Invoke();
//        }
//        ResetMission();
//    }

//    public void AbortMission()
//    {
//        if (onAbortMission != null)
//        {
//            onAbortMission.Invoke();
//        }
//        ResetMission();
//    }

//    public void EndMissionPreparation()
//    {
//        if (preparationTimer.IsPaused())
//        {
//            preparationTimer.UnpauseTimer();
//            return;
//        }
//        preparationTimer.StartTimer();
//    }

//    public void SetFailingReason(FailingReason reason)
//    {
//        failingReason = reason;
//    }


//    public void SwitchMissionState(MissionState newState)
//    {
//        lastMissionState = missionState;
//        missionState = newState;
//        OnLeavingMissionState();
//        OnEnteringMissionState();
//    }

//    protected void OnLeavingMissionState()
//    {
//        switch (lastMissionState)
//        {
//            case MissionState.PREPARATION:
//                OnLeavingPreparationState();
//                break;
//            case MissionState.RUNNING:
//                OnLeavingRunningState();
//                break;
//            case MissionState.ENDING:
//                OnLeavingEndingState();
//                break;
//            case MissionState.DONE:
//                OnLeavingDoneState();
//                break;
//            case MissionState.PAUSED:
//                OnLeavingPausedState();
//                break;
//            case MissionState.FAILING:
//                OnLeavingFailingState();
//                break;
//            case MissionState.FAILED:
//                OnLeavingFailedState();
//                break;
//        }
//        if(onLeavingState != null)
//        {
//            onLeavingState.Invoke(lastMissionState, missionState);
//        }
//    }
//    protected void OnEnteringMissionState()
//    {
//        switch (missionState)
//        {
//            case MissionState.PREPARATION:
//                OnEnteringPreparationState();
//                break;
//            case MissionState.RUNNING:
//                OnEnteringRunningState();
//                break;
//            case MissionState.ENDING:
//                OnEnteringEndingState();
//                break;
//            case MissionState.DONE:
//                OnEnteringDoneState();
//                break;
//            case MissionState.PAUSED:
//                OnEnteringPausedState();
//                break;
//            case MissionState.FAILING:
//                OnEnteringFailingState();
//                break;
//            case MissionState.FAILED:
//                OnEnteringFailedState();
//                break;
//        }
//        if (onEnteringState != null)
//        {
//            onEnteringState.Invoke(lastMissionState, missionState);
//        }
//    }


//    protected void OnLeavingPreparationState()
//    {
//        if (preparationTimer.IsRunning())
//        {
//            preparationTimer.PauseTimer();
//        }
//    }
//    protected void OnEnteringPreparationState()
//    {
//        //if(preparationTimer.IsPaused())
//        //{
//        //    preparationTimer.UnpauseTimer();
//        //    return;
//        //}
//        //preparationTimer.StartTimer();
//    }
//    protected void OnLeavingRunningState()
//    {
//        if (missionTimer.IsRunning())
//        {
//            missionTimer.PauseTimer();
//        }
//    }
//    protected void OnEnteringRunningState()
//    {
//        if (missionTimer.IsPaused())
//        {
//            missionTimer.UnpauseTimer();
//            return;
//        }
//        missionTimer.StartTimer(SaveSystem.SAVE_DATA.missionTime*60);
//    }

//    protected void OnLeavingEndingState()
//    {
//    }
//    protected void OnEnteringEndingState()
//    {
//        winningTimer.StartTimer();
//    }
//    protected void OnLeavingDoneState()
//    {
//    }
//    protected void OnEnteringDoneState()
//    {
//        wonTimer.StartTimer();
//    }
//    protected void OnLeavingPausedState()
//    {
//    }
//    protected void OnEnteringPausedState()
//    {
//    }
//    protected void OnLeavingFailingState()
//    {
//    }
//    protected void OnEnteringFailingState()
//    {

//        failingTimer.StartTimer(SaveSystem.SAVE_DATA.instantCrashFade ? 0 : initialFadeTime);
//    }
//    protected void OnLeavingFailedState()
//    {
//    }
//    protected void OnEnteringFailedState()
//    {
//        failedTimer.StartTimer();
//    }
//}
