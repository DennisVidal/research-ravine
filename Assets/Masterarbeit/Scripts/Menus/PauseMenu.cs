using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
        GameManager.Instance.onGameplayResumed += OnGameplayResumed;
        GameManager.Instance.onGameplayPaused += OnGameplayPaused;
    }
    void OnDestroy()
    {
        GameManager.Instance.onGameplayResumed -= OnGameplayResumed;
        GameManager.Instance.onGameplayPaused -= OnGameplayPaused;
    }

    void OnGameplayResumed()
    {
        gameObject.SetActive(false);
    }
    void OnGameplayPaused()
    {
        gameObject.SetActive(true);
    }

    public void ResumeGameplay()
    {
        GameManager.Instance.ResumeGameplay();
    }
    public void ReturnToMainMenu()
    {
        GameManager.Instance.currentMission.StopMission();
        SceneLoader.Instance.LoadMainMenu();
    }
    public void QuitGame()
    {
        SaveSystem.Save();
        Application.Quit();
    }
}
