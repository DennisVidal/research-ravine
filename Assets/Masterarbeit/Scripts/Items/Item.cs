using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum ItemType
{
    SCORE_CRATE,
    DRILL_RAY_CHARGE,
    SCORE_MULTIPLIER_2X
}

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public Vector3Int itemID;
    public float initialMoveTowardsSpeed = 10.0f;
    public float moveTowardsAcceleration = 10.0f;

    public float minDistance;
    public Vector3 minScale;

    public event Action<Item> onPickupStart;
    public event Action<Item> onPickupEnd;



    [NonSerialized] public bool isPickedUp;
    [NonSerialized] public SphereCollider pickUpTrigger;
    [NonSerialized] public Transform moveTowardsTarget;
    [NonSerialized] public Vector3 initialScale;
    [NonSerialized] public Coroutine pickedUpCoroutine;

    void Awake()
    {
        initialScale = gameObject.transform.localScale;
    }

    void Start()
    {
        pickUpTrigger = GetComponent<SphereCollider>();
    }
    void OnEnable()
    {
        ProceduralWorld.Instance.onWorldOriginChanged += OnWorldOriginChanged;
    }
    void OnDisable()
    {
        ProceduralWorld.Instance.onWorldOriginChanged -= OnWorldOriginChanged;
        ResetItem();
    }

    void OnWorldOriginChanged(Vector3Int offset, Vector3Int chunkOffset)
    {
        transform.position -= offset;
    }

    void OnTriggerEnter(Collider other)
    {
        if(isPickedUp)
        {
            return;
        }

        SetMoveTowardsTarget(other.gameObject.transform);
        StartPickedUpCoroutine();
    }

    public void ResetItem()
    {
        StopPickedUpCoroutine();
        isPickedUp = false;
        gameObject.transform.localScale = initialScale;
    }

    public bool IsPickedUp()
    {
        return isPickedUp;
    }


    public void SetMoveTowardsTarget(Transform targetTransform)
    {
        moveTowardsTarget = targetTransform;
    }

    void StartPickedUpCoroutine()
    {
        StopPickedUpCoroutine();
        pickedUpCoroutine = StartCoroutine(PickedUpCoroutine());
    }
    void StopPickedUpCoroutine()
    {
        if(pickedUpCoroutine != null)
        {
            StopCoroutine(pickedUpCoroutine);
        }    
    }

    IEnumerator PickedUpCoroutine()
    {
        isPickedUp = true;
        if (onPickupStart != null)
        {
            onPickupStart.Invoke(this);
        }

        Vector3 scaleSegment = initialScale - minScale;
        Vector3 scaleFactor = new Vector3(1.0f / scaleSegment.x, 1.0f / scaleSegment.y, 1.0f / scaleSegment.z);
        float distance = Vector3.Distance(gameObject.transform.position, moveTowardsTarget.position);
        float distanceFactor = 1.0f / (distance - minDistance);

        float speed = initialMoveTowardsSpeed;

        do
        {
            if(!GameManager.Instance.isGameplayPaused)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, moveTowardsTarget.position, Time.deltaTime * speed);
                gameObject.transform.localScale = Vector3.Scale(minScale + scaleSegment * Mathf.Clamp01((distance - minDistance) * distanceFactor), scaleFactor);

                distance = Vector3.Distance(gameObject.transform.position, moveTowardsTarget.position);
                speed += Time.deltaTime * moveTowardsAcceleration;
            }
            yield return null;
        }
        while (distance > minDistance);

        gameObject.transform.localScale = minScale;

        if(onPickupEnd != null)
        {
            onPickupEnd.Invoke(this);
        }
    }


    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void SetItemID(Vector3Int id)
    {
        itemID = id;
    }

    public void ShowItem()
    {
        gameObject.SetActive(true);
    }
    public void HideItem()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetWorldPosition()
    {
        return ProceduralWorld.Instance.worldOriginOffset + gameObject.transform.position;
    }
}
