using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillRayChargesUI : MonoBehaviour
{
    public static float SECONDS_TO_MINUTES = 1.0f / 60.0f;

    public TMPro.TextMeshProUGUI remainingChargesDisplayText;
    public TMPro.TextMeshProUGUI remainingTimeDisplayText;

    Coroutine updateRemainingTimeCoroutine;

    protected DrillRay drillRay;
    void Start()
    {
        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onChargeAmountChanged += OnChargeAmountChanged;
        drillRay.onFire += OnFire;
        SetRemainingCharges(drillRay.charges);
        SetRemainingTime("00:00");
    }

    void OnDestroy()
    {
        if(drillRay != null)
        {
            drillRay.onChargeAmountChanged -= OnChargeAmountChanged;
            drillRay.onFire -= OnFire;
        }
        StopUpdateRemainingTimeCoroutine();
    }

    void OnChargeAmountChanged(int charges)
    {
        SetRemainingCharges(charges);
    }

    void OnFire()
    {
        StartUpdateRemainingTimeCoroutine();
    }

    public void SetRemainingCharges(int charges)
    {
        remainingChargesDisplayText.SetText(charges.ToString());
    }

    public void SetRemainingTime(string timeString)
    {
        remainingTimeDisplayText.SetText(timeString);
    }


    void StartUpdateRemainingTimeCoroutine()
    {
        StopUpdateRemainingTimeCoroutine();
        updateRemainingTimeCoroutine = StartCoroutine(UpdateRemainingTimeCoroutine());
    }

    void StopUpdateRemainingTimeCoroutine()
    {
        if(updateRemainingTimeCoroutine != null)
        {
            StopCoroutine(updateRemainingTimeCoroutine);
        }

    }
  
    IEnumerator UpdateRemainingTimeCoroutine()
    {
        while (drillRay && drillRay.remainingLifetime > 0.0f)
        {
            SetRemainingTime(GetTimeString(drillRay.remainingLifetime));
            yield return null;
        }
    }
    public string GetTimeString(float time)
    {
        int minutes = (int)(time * SECONDS_TO_MINUTES);
        int seconds = (int)time - minutes * 60;

        string minutesPadding = minutes < 10 ? "0" : "";
        string secondsPadding = seconds < 10 ? "0" : "";
        return minutesPadding + minutes + ":" + secondsPadding + seconds;
    }
}
