using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollowMainCamera : MonoBehaviour
{
    public Vector3 localOffset;
    public bool alwaysFaceCamera;

    Camera mainCamera;

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        Transform cameraTransform = mainCamera.transform;
        Vector3 newPosition = cameraTransform.position +
                              cameraTransform.right * localOffset.x +
                              cameraTransform.up * localOffset.y +
                              cameraTransform.forward * localOffset.z;
        transform.position = newPosition;

        if (alwaysFaceCamera)
        {
            transform.LookAt(cameraTransform.position, cameraTransform.up);
        }
    }
}
