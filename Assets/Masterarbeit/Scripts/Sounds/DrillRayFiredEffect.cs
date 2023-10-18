using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillRayFiredEffect : MonoBehaviour
{
    public AudioSource audioSource;

    protected DrillRay drillRay;
    void Start()
    {
        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onFire += OnFire;

        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnDestroy()
    {
        if(drillRay != null)
        {
            drillRay.onFire -= OnFire;
        }
    }

    void OnFire()
    {
        audioSource.Play();
    }
}
