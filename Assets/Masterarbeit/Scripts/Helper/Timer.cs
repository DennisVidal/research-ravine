using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public static float SECONDS_TO_MINUTES = 1.0f / 60.0f;


    public int initialTime = 900;

    [NonSerialized] public float remainingTime = 0.0f;
    [NonSerialized] public bool isRunning = false;


    public event Action onStartTimer;
    public event Action onEndTimer;
    public event Action onPauseTimer;
    public event Action onUnpauseTimer;

    public void AbortTimer()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    public void ResetTimer()
    {
        remainingTime = -1.0f;
        isRunning = false;
        StopAllCoroutines();
    }
    public bool IsRunning()
    {
        return isRunning;
    }
    public bool IsPaused()
    {
        return !isRunning && remainingTime > 0.0f;
    }

    public void StartTimer()
    {
        if(!isRunning)
        {
            StartCoroutine(TimerCoroutine());
        }
    }
    public void StartTimer(int seconds)
    {
        initialTime = seconds;
        StartTimer();
    }

    public void PauseTimer()
    {
        if(!isRunning)
        {
            return;
        }

        isRunning = false;
        if (onPauseTimer != null)
        {
            onPauseTimer.Invoke();
        }
    }

    public void UnpauseTimer()
    {
        if (isRunning)
        {
            return;
        }
        isRunning = true;
        if (onUnpauseTimer != null)
        {
            onUnpauseTimer.Invoke();
        }
    }

    public void EndTimer()
    {
        StopAllCoroutines();
        isRunning = false;
        if (onEndTimer != null)
        {     
            onEndTimer.Invoke();
        }
    }

    void OnTimerEnded()
    {
        remainingTime = 0.0f;
        EndTimer();
    }

    IEnumerator TimerCoroutine()
    {
        isRunning = true;
        remainingTime = initialTime;

        if (onStartTimer != null)
        {
            onStartTimer.Invoke();
        }

        while (remainingTime > 0.0f)
        {
            if(isRunning)
            {
                remainingTime -= Time.deltaTime;
            }
            yield return null;
        }
        OnTimerEnded();
    }    

    public float GetRemainingSeconds()
    {
        return remainingTime;
    }
    public float GetRemainingMinutes()
    {
        return remainingTime * SECONDS_TO_MINUTES;
    }
    public void GetRemainingTime(out int minutes, out int seconds)
    {
        minutes = (int)(remainingTime * SECONDS_TO_MINUTES);
        seconds = (int)remainingTime - minutes * 60;
    }

    public void GetRemainingTime(out int minutes, out int seconds, out int milliseconds)
    {
        GetRemainingTime(out minutes, out seconds);
        milliseconds = (int)((remainingTime - (int)remainingTime) * 1000.0f);
    }

    public string GetRemainingTimeString()
    {
        GetRemainingTime(out int minutes, out int seconds);
        string minutesPadding = minutes < 10 ? "0" : "";
        string secondsPadding = seconds < 10 ? "0" : "";
        return minutesPadding + minutes + ":" + secondsPadding + seconds;
    }
}
