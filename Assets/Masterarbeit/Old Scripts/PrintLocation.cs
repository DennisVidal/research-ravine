using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = gameObject.transform.position;
        Vector3 localPosition = gameObject.transform.localPosition;
        Debug.Log("position = " + position + ", localPosition = " + localPosition);
    }
}
