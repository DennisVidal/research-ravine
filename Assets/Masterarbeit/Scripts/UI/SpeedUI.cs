using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI speedDisplayText;
    public Vehicle vehicle;

    void Update()
    {
        SetSpeed(vehicle.currentMovementSpeed);
    }

    void SetSpeed(float speed)
    {
        speedDisplayText.SetText(Mathf.RoundToInt(speed) + "km/h");
    }
}
