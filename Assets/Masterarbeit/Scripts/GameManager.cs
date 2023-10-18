using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Icaros.Desktop.Input;


public enum PlayerStatus
{
    NONE,
    ALIVE,
    RESCUED,
    CRASHED,
    CRUSHED,
    Count
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameplayPaused;

    public event Action onGameplayPaused;
    public event Action onGameplayResumed;

    public Mission currentMission;

    public event Action<SceneIndex> onSceneReady;

    public event Action<Mission> onMissionStart;
    public event Action<Mission> onMissionEnd;
    public event Action<Mission> onMissionAbort;


    public int currentScore;

    public event Action<int> onScoreChanged;

    public int scoreMultiplier = 1;
    Timer scoreMultiplierTimer;


    public AudioSource crushedSound;


    public Difficulty selectedDifficulty;

    public List<DifficultyData> difficulties;

    public int selectedColorMode;
    public List<ColorModeData> colorModes;

    public PlayerStatus playerStatus;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);


        SaveSystem.Init();
        SaveSystem.Load();

        currentMission.onSwitchingMissionState += OnSwitchingMissionState;
        currentMission.onMissionEnded += OnMissionEnded;

        scoreMultiplierTimer = gameObject.AddComponent<Timer>();
        scoreMultiplierTimer.onEndTimer += OnScoreMultiplierTimerEnd;


        AudioListener.volume = SaveSystem.SAVE_DATA.GetAudioVolumeFloat();
        selectedDifficulty = SaveSystem.SAVE_DATA.GetDifficulty();
        selectedColorMode = SaveSystem.SAVE_DATA.GetColorMode();
    }

    private void Start()
    {

        SceneLoader.Instance.onStartedLoading += OnSceneLoadingStarted;
        SceneLoader.Instance.onFinishedLoading += OnSceneLoadingFinished;
        SceneLoader.Instance.onFinishedSetup += OnSceneSetupFinished;

    }


    public void SetScoreMultiplier(int multiplier, int durationInSeconds)
    {
        scoreMultiplier = multiplier;
        scoreMultiplierTimer.ResetTimer();
        scoreMultiplierTimer.StartTimer(durationInSeconds);
    }

    public void OnScoreMultiplierTimerEnd()
    {
        scoreMultiplier = 1;
    }

    public DifficultyData GetSelectedDifficultyData()
    {
        return difficulties[(int)selectedDifficulty];
    }
    public ColorModeData GetSelectedColorModeData()
    {
        return colorModes[(int)selectedColorMode];
    }

    public bool AddDrillRayCharge(int amount)
    {
        DrillRay drillRay = DrillRay.GetActiveDrillRay();
        if (drillRay.CanAddCharge())
        {
            drillRay.AddCharge(amount);
            return true;
        }
        return false;
    }

    public void AddScore(int score)
    {
        currentScore += score * scoreMultiplier;
        if (onScoreChanged != null)
        {
            onScoreChanged.Invoke(currentScore);
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public int GetHighscore()
    {
        return SaveSystem.SAVE_DATA.GetHighscore();
    }
    public void SetHighscore(int highscore)
    {
        SaveSystem.SAVE_DATA.SetHighscore(highscore);
        SaveSystem.Save();
    }


    void OnSwitchingMissionState(MissionState oldState, MissionState newState)
    {
        if (newState == null || newState.GetMissionStateType() != MissionStateType.ENDED)
        {
            return;
        }

        if (currentScore > GetHighscore())
        {
            SetHighscore(currentScore);
        }
    }

    void OnMissionEnded()
    {
        SceneLoader.Instance.LoadMainMenu();
    }


    void OnSceneLoadingStarted(SceneIndex oldSceneIndex, SceneIndex newSceneIndex)
    {
        switch (newSceneIndex)
        {
            case SceneIndex.MAIN_MENU:
                if(isGameplayPaused)
                {
                    ResumeGameplay();
                }
                break;
        }
    }
    void OnSceneLoadingFinished(SceneIndex oldSceneIndex, SceneIndex newSceneIndex)
    {
        switch (newSceneIndex)
        {
            case SceneIndex.GAME_WORLD:
                ProceduralWorld.Instance.GenerateWorld();
                break;
        }
    }
    void OnSceneSetupFinished(SceneIndex oldSceneIndex, SceneIndex newSceneIndex)
    {
        AudioListener.volume = SaveSystem.SAVE_DATA.GetAudioVolumeFloat();
        if (newSceneIndex == SceneIndex.GAME_WORLD)
        {
            OnEnterGameWorld();
        }
    }

    void OnEnterGameWorld()
    {
        currentScore = 0;
        SetPlayerStatus(PlayerStatus.ALIVE);
        currentMission.StartMission();
    }

    public PlayerStatus GetPlayerStatus()
    {
        return playerStatus;
    }

    public void SetPlayerStatus(PlayerStatus status)
    {
        playerStatus = status;

        if(playerStatus == PlayerStatus.RESCUED)
        {
            currentScore *= SaveSystem.SAVE_DATA.GetMissionTimeMinutes() * GetSelectedDifficultyData().survivalMultiplier;
        }

        MissionState currentMissionState = currentMission.GetCurrentMissionState();
        if (currentMissionState == null || currentMissionState.GetMissionStateType() != MissionStateType.RUNNING)
        {
            return;
        }

        if (playerStatus == PlayerStatus.CRUSHED)
        {
            crushedSound.Play();
        }

        if (playerStatus == PlayerStatus.CRUSHED || playerStatus == PlayerStatus.CRASHED)
        {
            currentMissionState.ForceExit();
        }
    }



    public void PauseGameplay()
    {
        isGameplayPaused = true;
        if (onGameplayPaused != null)
        {
            onGameplayPaused.Invoke();
        }
    }

    public void ResumeGameplay()
    {
        isGameplayPaused = false;
        if (onGameplayResumed != null)
        {
            onGameplayResumed.Invoke();
        }
    }

    public bool TogglePauseGameplay()
    {
        if(isGameplayPaused)
        {
            ResumeGameplay();
        }
        else
        {
            PauseGameplay();
        }
        return isGameplayPaused;
    }
}
