using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSoundOnPause : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        GameManager.Instance.onGameplayPaused += OnGameplayPaused;
        GameManager.Instance.onGameplayResumed += OnGameplayResumed;
    }

    void OnDestroy()
    {
        GameManager.Instance.onGameplayPaused -= OnGameplayPaused;
        GameManager.Instance.onGameplayResumed -= OnGameplayResumed;
    }


    void OnGameplayPaused()
    {
        audioSource.Pause();
    }
    void OnGameplayResumed()
    {
        audioSource.UnPause();
    }
}
