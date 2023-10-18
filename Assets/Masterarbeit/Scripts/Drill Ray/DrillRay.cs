using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class DrillRay : MonoBehaviour
{
    
    public static int PHYSICS_LAYER = 1 << 13;


    public float length = 10.0f;

    public event Action onFire;
    public event Action onExpire;
    public event Action<int> onChargeAmountChanged;

    public static event Action<DrillRay, DrillRay> onActiveDrillRayChanges;

    public GameObject drillRayObject;
    public CapsuleCollider rayCollider;

    [NonSerialized] public bool canFire;

    public int maxCharges = 3;
    [NonSerialized] public int charges = 3;


    protected Vector3 startPosition;
    protected Vector3 endPosition;
    protected float minRadius;
    protected float maxRadius;
    protected bool isEnabled;


    protected static DrillRay activeDrillRay;

    public static DrillRay GetActiveDrillRay()
    {
        return activeDrillRay;
    }
    public static void SetActiveDrillRay(DrillRay ray)
    {
        activeDrillRay = ray;
    }

    private void Awake()
    {
        SetActiveDrillRay(this);

        drillRayObject = new GameObject("Drill Ray Object");
        drillRayObject.transform.parent = null;
        drillRayObject.layer = LayerMask.NameToLayer("Drill Ray");
        rayCollider = drillRayObject.AddComponent<CapsuleCollider>();
        rayCollider.direction = 2;

        canFire = true;
    }

    private void Start()
    {
        ProceduralWorld.Instance.onWorldOriginChanged += OnWorldOriginChanged;

        InputManager.Instance.onButtonPressed += OnButtonPressed;


        DifficultyData difficultyData = GameManager.Instance.GetSelectedDifficultyData();
        maxCharges = difficultyData.drillRayCharges;
        charges = maxCharges;
    }

    void OnDestroy()
    {
        if(ProceduralWorld.Instance != null)
        {
            ProceduralWorld.Instance.onWorldOriginChanged -= OnWorldOriginChanged;
        }

        InputManager.Instance.onButtonPressed -= OnButtonPressed;
    }

    [NonSerialized] public float remainingLifetime;
    void Update()
    {
        if(isEnabled && !GameManager.Instance.isGameplayPaused)
        {
            remainingLifetime -= Time.deltaTime;
            if(remainingLifetime < 0.0f)
            {
                OnExpire();
            }
        }
    }

    public float GetStrength()
    {
        return 1000.0f;
    }    
    public Vector3 GetStartPosition()
    {
        return startPosition;
    }
    public Vector3 GetEndPosition()
    {
        return endPosition;
    }
    public float GetMinRadius()
    {
        return minRadius;
    }
    public float GetMaxRadius()
    {
        return maxRadius;
    }
    public bool IsEnabled()
    {
        return isEnabled;
    }

    void OnButtonPressed(int button)
    {
        if(button != 2 || !CanFire() || GameManager.Instance.currentMission.GetCurrentMissionStateType() != MissionStateType.RUNNING)
        {
            return;
        }

        Fire();
    }



    void OnWorldOriginChanged(Vector3Int offset, Vector3Int chunkOffset)
    {
        drillRayObject.transform.position -= offset;
    }

    public bool CanFire()
    {
        return canFire && charges > 0;
    }

    public bool CanAddCharge()
    {
        return charges < maxCharges;
    }
    public void AddCharge(int amount)
    {
        charges = Mathf.Clamp(charges + amount, 0, maxCharges);
        if (onChargeAmountChanged != null)
        {
            onChargeAmountChanged.Invoke(charges);
        }
    }

    public void Fire()
    {
        OnFire();
    }


    void OnFire()
    {
        AddCharge(-1);
        canFire = false;
        startPosition = transform.position + ProceduralWorld.Instance.worldOriginOffset;
        endPosition = startPosition + transform.forward * length;

        DifficultyData difficultyData = GameManager.Instance.GetSelectedDifficultyData();

        minRadius = difficultyData.drillRayRadius;
        maxRadius = difficultyData.drillRayRadius + 1;

        drillRayObject.transform.position = transform.position;
        drillRayObject.transform.forward = transform.forward;

        rayCollider.center = new Vector3(0.0f, 0.0f, length * 0.5f);
        rayCollider.radius = maxRadius;
        rayCollider.height = length + 2 * maxRadius;
        
        remainingLifetime = GameManager.Instance.GetSelectedDifficultyData().drillRayDuration;
        isEnabled = true;
        if (onFire != null)
        {
            onFire.Invoke();
        }
    }

    void OnExpire()
    {
        isEnabled = false;
        canFire = true;
        if (onExpire != null)
        {
            onExpire.Invoke();
        }
    }


    public bool IsWithinRay(Vector3 position)
    {
        Vector3 drillRayStart = transform.position;
        Vector3 drillRayEnd = drillRayStart + transform.forward * length;
        Vector3 startToEnd = drillRayEnd - drillRayStart;
        Vector3 startToPosition = position - drillRayStart;
        Vector3 endToPosition = position - drillRayEnd;

        float dotAtStart = Vector3.Dot(startToEnd, startToPosition);
        float dotAtEnd = Vector3.Dot(startToEnd, endToPosition);

        float distanceToRay = dotAtStart < 0.0f ? startToPosition.magnitude : (dotAtEnd > 0.0f ? endToPosition.magnitude : Vector3.Cross(startToEnd, startToPosition).magnitude / startToEnd.magnitude);
        return distanceToRay < maxRadius;
    }
}
