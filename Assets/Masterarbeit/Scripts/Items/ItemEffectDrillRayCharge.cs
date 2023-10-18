using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectDrillRayCharge : MonoBehaviour
{
    public int chargeAmount = 1;
    Item item;

    void Start()
    {
        item = GetComponent<Item>();
        //item.onPickupStart += OnPickUpStart;
        item.onPickupEnd += OnPickUpEnd;
    }

    void OnDestroy()
    {
        if (item != null)
        {
            //item.onPickupStart -= OnPickUpStart;
            item.onPickupEnd -= OnPickUpEnd;
        }
    }

    void OnPickUpStart(Item item)
    {
    }
    void OnPickUpEnd(Item item)
    {
        GameManager.Instance.AddDrillRayCharge(chargeAmount);
    }

}
