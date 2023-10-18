using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyData : MonoBehaviour
{
    public int survivalMultiplier;

    public Vector2 minMaxMovementSpeed;

    public int drillRayCharges;
    public int drillRayDuration;
    public float drillRayRadius;

    public float proximityScoringRadius;

    public float terrainWidthFactor;

    public float turnSpeedBoosterMaxCharge;
    public float turnSpeedBoosterRechargeRate;
    public float turnSpeedBoosterMultiplier;
}

public enum Difficulty
{
    NORMAL,
    HARD,
    VERY_HARD,
    COUNT
}
