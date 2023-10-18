using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurningSpeedBoosterUI : MonoBehaviour
{
    public TurnSpeedBooster boosterComponent;
    public List<Image> imageSegments;

    public Color activeColor;
    public Color inactiveColor;

    void Update()
    {
        UpdateSegments(boosterComponent.GetNormalizedCharge());
    }

    public void UpdateSegments(float charge)
    {
        int setSegments = (int)(charge * imageSegments.Count);
        for (int i = 0; i < imageSegments.Count; i++)
        {
            imageSegments[i].color = i < setSegments ? activeColor : inactiveColor;
        }
    }

}
