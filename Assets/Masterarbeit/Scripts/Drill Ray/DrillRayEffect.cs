using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillRayEffect : MonoBehaviour
{
    public LayerMask collisionLayer;
    public Transform drillRayBombOrigin;

    public GameObject drillRayBombObject;

    Coroutine projectileCoroutine;

    public float movementSpeed = 100.0f;
    public float minDistance = 0.5f;


    public ParticleSystem bombEffectParticleSystem;
    public ParticleSystem effectParticleSystem;

    Vector3 targetPosition;

    public event System.Action<Vector3> onDrillRayBombPositionReset;

    protected DrillRay drillRay; 

    void Start()
    {
        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onFire += OnFire;
        drillRay.onExpire += OnExpire;

        ProceduralWorld.Instance.onWorldOriginChanged += OnWorldOriginChanged;
    }
    void OnDestroy()
    {
        if (drillRay != null)
        {
            drillRay.onFire -= OnFire;
            drillRay.onExpire -= OnExpire;
        }
        if (ProceduralWorld.Instance != null)
        {
            ProceduralWorld.Instance.onWorldOriginChanged -= OnWorldOriginChanged;
        }

        StopProjectileCoroutine();
    }

    void OnWorldOriginChanged(Vector3Int offset, Vector3Int chunkOffset)
    {
        gameObject.transform.position -= offset;
        targetPosition -= offset;

        if(drillRayBombObject.transform.parent == null)
        {
            drillRayBombObject.transform.position -= offset;
        }
    }


    void OnFire()
    {
        gameObject.transform.position = drillRayBombOrigin.position;
        gameObject.transform.forward = drillRayBombOrigin.forward;
        StartProjectileCoroutine();
        effectParticleSystem.Play();
    }

    void OnExpire()
    {
        effectParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void StartProjectileCoroutine()
    {
        StopProjectileCoroutine();
        projectileCoroutine = StartCoroutine(ProjectileCoroutine());
    }

    void StopProjectileCoroutine()
    {
        if(drillRayBombObject != null && drillRayBombOrigin != null)
        {
            drillRayBombObject.transform.parent = drillRayBombOrigin;
            drillRayBombObject.transform.localPosition = Vector3.zero;
        }
        if (projectileCoroutine != null)
        {
            StopCoroutine(projectileCoroutine);
        }
    }
  
    IEnumerator ProjectileCoroutine()
    {
        bombEffectParticleSystem.Play();
        Vector3 originPosition = drillRayBombOrigin.position;
        Vector3 direction = drillRayBombOrigin.forward;
        float length = DrillRay.GetActiveDrillRay().length;
        targetPosition = originPosition + direction * length;

        drillRayBombObject.transform.parent = null;
        float distance = Vector3.Distance(drillRayBombObject.transform.position, targetPosition);
        while (distance > minDistance)
        {
            drillRayBombObject.transform.position = Vector3.MoveTowards(drillRayBombObject.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            distance = Vector3.Distance(drillRayBombObject.transform.position, targetPosition);
            yield return null;
        }
        bombEffectParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ResetBombPosition();
    }


    void ResetBombPosition()
    {
        if(drillRayBombOrigin && drillRayBombObject)
        {
            drillRayBombObject.transform.parent = drillRayBombOrigin;
            drillRayBombObject.transform.localPosition = Vector3.zero;

            if(onDrillRayBombPositionReset != null)
            {
                onDrillRayBombPositionReset.Invoke(drillRayBombObject.transform.position);
            }
        }
    }


}
