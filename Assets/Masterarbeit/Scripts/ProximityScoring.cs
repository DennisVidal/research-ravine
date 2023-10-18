using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityScoring : MonoBehaviour
{
    public int scorePerSecond = 100;

    public float currentProximityMultiplier = 1.0f;
    public float currentSpeedMultiplier = 1.0f;
    public float MaxMultiplier = 10.0f;

    public float multiplierIncreaseRate = 2.0f;
    public float multiplierDecreaseRate = 1.0f;


    public Vector2 minMaxMovementSpeed = new Vector2(10.0f, 40.0f);
    public Vector2 minMaxMovementSpeedMultiplier = new Vector2(1.0f, 10.0f);


    float remainingScoreToAdd = 0.0f;
    bool isCloseEnough;

    public SphereCollider scoringCollider;
    public Vehicle vehicle;

    void Start()
    {
        isCloseEnough = false;
        currentProximityMultiplier = 1.0f;
        minMaxMovementSpeed = GameManager.Instance.GetSelectedDifficultyData().minMaxMovementSpeed + new Vector2(5.0f, -10.0f);

        scoringCollider.radius = GameManager.Instance.GetSelectedDifficultyData().proximityScoringRadius;
    }

    void OnTriggerStay(Collider other)
    {
        isCloseEnough = true;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameplayPaused && GameManager.Instance.currentMission.GetCurrentMissionStateType() == MissionStateType.RUNNING)
        {
            UpdateMultiplier();
            UpdateScore();
            int intScoreToAdd = (int)remainingScoreToAdd;
            if (intScoreToAdd != 0)
            {
                remainingScoreToAdd -= intScoreToAdd;
                GameManager.Instance.AddScore(intScoreToAdd);
            }
        }
        isCloseEnough = false;
    }

    void AddToDistanceMultiplier(float value)
    {
        currentProximityMultiplier = Mathf.Clamp(currentProximityMultiplier + value, 1.0f, MaxMultiplier);
    }


    void UpdateMultiplier()
    {
        float rate = isCloseEnough ? multiplierIncreaseRate : -multiplierDecreaseRate;
        AddToDistanceMultiplier(rate * Time.deltaTime);


        float speedFactor = GetSpeedFactor(vehicle.GetMovementSpeed());
        currentSpeedMultiplier = GetSpeedMultiplier(speedFactor);
    }

    void UpdateScore()
    {
        if (!isCloseEnough)
        {
            return;
        }

        float scoreToAdd = scorePerSecond * currentSpeedMultiplier * currentProximityMultiplier * GameManager.Instance.scoreMultiplier * Time.deltaTime;

        remainingScoreToAdd += scoreToAdd;
    }

    public float GetSpeedFactor(float movementSpeed)
    {
        return Mathf.Clamp01((movementSpeed - minMaxMovementSpeed.x) / (minMaxMovementSpeed.y - minMaxMovementSpeed.x));
    }
    public float GetSpeedMultiplier(float speedFactor)
    {
        float minMult = minMaxMovementSpeedMultiplier.x;
        float maxMult = minMaxMovementSpeedMultiplier.y;
        return Mathf.Lerp(minMult, maxMult, speedFactor);
    }
}
