using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTexture : MonoBehaviour
{
    public ComputeShader noiseComputeShader;
    public RenderTexture noiseTexture;

    public float frequency = 1.0f;
    public float threshhold = 0.0f;
    public float threshhold2 = 1.0f;
    public float power = 0.0f;
    public int octaves = 1;
    public bool invertNoise = false;
    public float zCoord;
     MeshRenderer meshRenderer;
    void Start()
    {
        noiseTexture = new RenderTexture(1024, 1024, 24);
        noiseTexture.enableRandomWrite = true;
        noiseTexture.Create();

        noiseComputeShader.SetTexture(0, "Result", noiseTexture);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        noiseComputeShader.SetFloat("frequency", frequency);
        noiseComputeShader.SetFloat("threshhold", threshhold);
        noiseComputeShader.SetFloat("threshhold2", threshhold2);
        noiseComputeShader.SetFloat("power", power);
        noiseComputeShader.SetInt("octaves", octaves);
        noiseComputeShader.SetBool("invertNoise", invertNoise);
        noiseComputeShader.SetFloat("zCoord", zCoord);
        noiseComputeShader.Dispatch(0, noiseTexture.width / 8, noiseTexture.height / 8, 1);

        meshRenderer.material.SetTexture("_BaseMap", noiseTexture);
    }
}
