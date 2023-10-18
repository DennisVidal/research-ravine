using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMultiplierUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI scoreMultiplierDisplayText;

    public ProximityScoring proximityScoring;


    void Update()
    {
        float proximityScoringMultiplier = proximityScoring.currentSpeedMultiplier * proximityScoring.currentProximityMultiplier;
        SetScoreMultipliers(GameManager.Instance.scoreMultiplier * proximityScoringMultiplier);
    }

    void SetScoreMultipliers(float scoreMultiplier)
    {
        scoreMultiplierDisplayText.SetText(scoreMultiplier.ToString("n2") + "x");
    }
}
