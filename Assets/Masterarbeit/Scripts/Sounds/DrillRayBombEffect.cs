using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillRayBombEffect : MonoBehaviour
{
    public AudioSource audioSource;

    public DrillRayEffect rayEffect;

    public float delay = 0.1f;

    protected DrillRay drillRay;
    void Start()
    {
        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onFire += OnFire;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        rayEffect.onDrillRayBombPositionReset += OnDrillRayBombPositionReset;
    }

    void OnDestroy()
    {
        if (drillRay != null)
        {
            drillRay.onFire -= OnFire;
        }

        if(rayEffect != null)
        {
            rayEffect.onDrillRayBombPositionReset -= OnDrillRayBombPositionReset;
        }
    }

    void OnFire()
    {
        audioSource.PlayDelayed(delay);
    }

    void OnDrillRayBombPositionReset(Vector3 position)
    {
        audioSource.Stop();
    }
}
