using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Vehicle : MonoBehaviour
{
    public Rigidbody rigidBody;

    public Transform playerAnchor;
    public Transform vehicle;

    public float currentMovementSpeed = 4.0f;
    public Vector2 minMaxSpeed = new Vector2(0.0f, 30.0f);
    public float acceleration = 5.0f;

    public Vector3 rotationMultipliers = new Vector3(1.5f, 0.0f, 2.0f);

    public bool controlsEnabled = false;

    public bool crashed = false;


    public event Action<EventData<float>> onUpdateTurnSpeedModifier;

    protected DrillRay drillRay;

    void Start()
    {
        if(rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        GameManager.Instance.onGameplayPaused += OnGameplayPaused;
        GameManager.Instance.onGameplayResumed += OnGameplayResumed;
        GameManager.Instance.currentMission.onSwitchingMissionState += OnSwitchingMissionState;

        InputManager.Instance.onButtonKeepPressed += OnButtonKeepPressed;

        SceneLoader.Instance.onFinishedSetup += OnSceneSetupFinished;

        ProceduralWorld.Instance.onWorldOriginChanged += OnWorldOriginChanged;

        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onExpire += OnDrillRayExpired;

        rotationMultipliers.x = SaveSystem.SAVE_DATA.sensitivityX;
        rotationMultipliers.z = SaveSystem.SAVE_DATA.sensitivityZ;

        minMaxSpeed = GameManager.Instance.GetSelectedDifficultyData().minMaxMovementSpeed;
    }

    void OnDestroy()
    {
        GameManager.Instance.onGameplayPaused -= OnGameplayPaused;
        GameManager.Instance.onGameplayResumed -= OnGameplayResumed;
        GameManager.Instance.currentMission.onSwitchingMissionState -= OnSwitchingMissionState;

        InputManager.Instance.onButtonKeepPressed -= OnButtonKeepPressed;

        SceneLoader.Instance.onFinishedSetup -= OnSceneSetupFinished;

        if (ProceduralWorld.Instance != null)
        {
            ProceduralWorld.Instance.onWorldOriginChanged -= OnWorldOriginChanged;
        }

        if(drillRay != null)
        {
            drillRay.onExpire -= OnDrillRayExpired;
        }
    }

    void OnWorldOriginChanged(Vector3Int offset, Vector3Int chunkOffset)
    {
        transform.position -= offset;
    }

    void OnSwitchingMissionState(MissionState oldState, MissionState newState)
    {
        if (newState != null && newState.GetMissionStateType() == MissionStateType.RUNNING)
        {
            EnableControls(true);
        }
        if (oldState != null && oldState.GetMissionStateType() == MissionStateType.RUNNING)
        {
            EnableControls(false);
        }
    }


    public Vector3 GetVelocity()
    {
        return transform.forward * currentMovementSpeed;
    }
    public float GetMovementSpeed()
    {
        return currentMovementSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance.currentMission.GetCurrentMissionStateType() == MissionStateType.RUNNING)
        {
            EnableControls(false);
            rigidBody.useGravity = true;

            ContactPoint firstContact = collision.GetContact(0);
            Vector3 force = firstContact.normal + GetVelocity() * 2.0f * Time.deltaTime;
            force *= 1000.0f;
            rigidBody.AddForceAtPosition(force, firstContact.point, ForceMode.Impulse);

            if(!crashed)
            {
                crashed = true;
                GameManager.Instance.SetPlayerStatus(PlayerStatus.CRASHED);
            }
        }
    }

    void OnDrillRayExpired()
    {
        if (!crashed && drillRay.IsWithinRay(transform.position) &&  TerrainFunctionCPU.IsInTerrain(transform.position + ProceduralWorld.Instance.worldOriginOffset))
        {
            crashed = true;
            GameManager.Instance.SetPlayerStatus(PlayerStatus.CRUSHED);
        }
    }

    void OnGameplayPaused()
    {
        if (GameManager.Instance.currentMission.GetCurrentMissionStateType() == MissionStateType.RUNNING)
        {
            EnableControls(false);
        }
    }
    void OnGameplayResumed()
    {
        if (GameManager.Instance.currentMission.GetCurrentMissionStateType() == MissionStateType.RUNNING)
        {
            EnableControls(true);
        }
    }

    void OnSceneSetupFinished(SceneIndex oldSceneIndex, SceneIndex newSceneIndex)
    {
        if (newSceneIndex != SceneIndex.GAME_WORLD)
        {
            return;
        }

        //Small fix to handle some rare cases of the calculated spawn direction facing a somewhat close wall
        for (int i = 0; i < 360; i += 15)
        {
            if (Physics.Raycast(transform.position, transform.forward, 40.0f, 1 << 15))
            {
                transform.Rotate(Vector3.up, 15.0f);
            }
        }
    }



    public void SetPosition(Vector3 position)
    {
        rigidBody.position = position;
    }
    public void SetRotation(Vector3 rotation)
    {
        SetRotation(Quaternion.Euler(rotation));
    }
    public void SetRotation(Quaternion rotation)
    {
        rigidBody.rotation = rotation;
    }

    public void EnableControls(bool enable)
    {
        controlsEnabled = enable;
    }

    public bool AreControlsEnabled()
    {
        return controlsEnabled;
    }

    void Accelerate()
    {
        AddMovementSpeed(Time.deltaTime * acceleration);
    }

    void Decelerate()
    {
        //AddMovementSpeed(- Time.deltaTime * acceleration);
    }


    void OnButtonKeepPressed(int button, float pressedDuration)
    {
        if (!AreControlsEnabled())
        {
            return;
        }

        switch (button)
        {
            case 0:
                Accelerate();
                break;
            case 1:
                Decelerate();
                break;
        }
    }


    void Update()
    {
        if(AreControlsEnabled())
        {
            AddMovementSpeed(-Time.deltaTime * 2.0f); //Passive slowdown over time
        }
        vehicle.transform.localEulerAngles = new Vector3(0.0f, 0.0f, InputManager.Instance.GetRotation(2) * 1.25f);//rotationMultipliers.z
    }

    void AddMovementSpeed(float speed)
    {
        currentMovementSpeed = Mathf.Clamp(currentMovementSpeed + speed, minMaxSpeed.x, minMaxSpeed.y);
    }


    void FixedUpdate()
    {
        ApplyRotation();
        ApplyMovement();
    }

    void ApplyRotation()
    {
        Vector3 inputRotation = InputManager.Instance.GetRotation();
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = Mathf.Clamp(inputRotation.x * rotationMultipliers.x, -65.0f, 65.0f);
        if (AreControlsEnabled())
        {
            EventData<float> turnSpeedModifierData = new EventData<float>(1.0f);
            if(onUpdateTurnSpeedModifier != null)
            {
                onUpdateTurnSpeedModifier.Invoke(turnSpeedModifierData);
            }
    
            float turnSpeedModifier = turnSpeedModifierData.param0;
    
            newRotation.y -= inputRotation.z * rotationMultipliers.z * turnSpeedModifier * Time.fixedDeltaTime;
        }
        rigidBody.MoveRotation(Quaternion.Euler(newRotation));
    }


    void ApplyMovement()
    {
        if (!AreControlsEnabled())
        {
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 movementOffset = transform.forward * currentMovementSpeed * Time.fixedDeltaTime;

        if(currentPosition.y > 90.0f && movementOffset.y > 0.0f)
        {
            movementOffset.y *= 1.0f - Mathf.Clamp01((currentPosition.y - 90.0f) * 0.1f);
        }

        rigidBody.MovePosition(currentPosition + movementOffset);
    }
}
