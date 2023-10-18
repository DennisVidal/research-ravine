using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Vector3 direction;
    public Vector3 angles;
    public float movementSpeed;
    public float rotationSpeed;

    void Update()
    {
        gameObject.transform.position += direction * movementSpeed * Time.deltaTime;
        gameObject.transform.Rotate(angles * rotationSpeed * Time.deltaTime);
    }
}
