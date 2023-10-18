using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillRayParticleSystem : MonoBehaviour
{
    public ParticleSystem particles;

    protected DrillRay drillRay;
    void Start()
    {
        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onFire += OnFire;
    }

    private void OnDestroy()
    {
        if(drillRay != null)
        {
            drillRay.onFire -= OnFire;
        }
    }

    void OnFire()
    {
        ParticleSystem.ShapeModule shapeModule = particles.shape;
        shapeModule.radius = drillRay.GetMaxRadius();
    }
}
