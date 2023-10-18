using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNoise : MonoBehaviour
{
    ComputeBuffer b;
    public Transform targetTransform;
    public ComputeShader computeShader;

    void Update()
    {
        Vector3 targetPosition = targetTransform.position + ProceduralWorld.Instance.worldOriginOffset;
        b = new ComputeBuffer(1, 4);
        computeShader.SetVector("targetPosition", targetPosition);
        computeShader.SetBuffer(0, "floatBuffer", b);

        computeShader.Dispatch(0,1,1,1);

        float[] data = new float[1];
        b.GetData(data);

        float cpuValue = TerrainFunctionCPU.GetTerrainValue(targetPosition);
        float diff = cpuValue - data[0];
        if (Mathf.Abs(diff) > 0.01f)
        {
            Debug.Log("Values are different: " + diff + ", cpu: " + cpuValue + ", gpu: " + data[0]);
        }
        else
        {
            Debug.Log("Values are same");
        }
    }
}
