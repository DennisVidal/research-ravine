using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAtHeight : MonoBehaviour
{
    public Transform followTransform;
    public float height;

    void Update()
    {
        Vector3 newPosition = followTransform.position;
        newPosition.y = height;
        transform.position = newPosition;
    }
}
