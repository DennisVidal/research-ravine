using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class GeneralInputs : MonoBehaviour
{
    public int recenterButton;
    public float recenterPressedDuration;

    public int pauseButton;

    XRInputSubsystem xrInputSubsystem;

    bool[] buttonInputHandled;

    void Awake()
    {
        buttonInputHandled = new bool[4];
    }

    void Start()
    {
        InputManager.Instance.onButtonKeepPressed += OnButtonKeepPressed;
        InputManager.Instance.onButtonReleased += OnButtonReleased;

        xrInputSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
    }
    void OnDestroy()
    {
        InputManager.Instance.onButtonKeepPressed -= OnButtonKeepPressed;
        InputManager.Instance.onButtonReleased -= OnButtonReleased;
    }

    void OnButtonKeepPressed(int button, float pressedDuration)
    {
        if(buttonInputHandled[button])
        {
            return;
        }

        //if(CheckRecenter(button, pressedDuration))
        //{
        //    return;
        //}
        CheckRecenter(button, pressedDuration);
    }

    void OnButtonReleased(int button, float pressedDuration)
    {
        if(!buttonInputHandled[button])
        {
            CheckPause(button, pressedDuration);
        }
        buttonInputHandled[button] = false;
    }

    bool CheckRecenter(int button, float pressedDuration)
    {
        if (button != recenterButton || pressedDuration < recenterPressedDuration)
        {
            return false;
        }
        buttonInputHandled[button] = true;

        if (xrInputSubsystem == null || xrInputSubsystem.TryRecenter())
        {
            InputTracking.Recenter();
        }
        return true;
    }
    bool CheckPause(int button, float pressedDuration)
    {
        if (button != pauseButton || !CanTogglePause())
        {
            return false;
        }
        buttonInputHandled[button] = true;

        GameManager.Instance.TogglePauseGameplay();
        return true;
    }


    bool IsGameWorld()
    {
        return SceneLoader.Instance.currentSceneIndex == SceneIndex.GAME_WORLD;
    }

    bool CanTogglePause()
    {
        return IsGameWorld() && GameManager.Instance.currentMission.GetCurrentMissionStateType() == MissionStateType.RUNNING;
    }
}
