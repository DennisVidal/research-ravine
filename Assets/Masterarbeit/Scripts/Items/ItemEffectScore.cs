using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectScore : MonoBehaviour
{
    public int score;

    Item item;

    void Start()
    {
        item = GetComponent<Item>();
        //item.onPickupStart += OnPickUpStart;
        item.onPickupEnd += OnPickUpEnd;
    }

    void OnDestroy()
    {
        if(item != null)
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
        GameManager.Instance.AddScore(score);
    }

}
