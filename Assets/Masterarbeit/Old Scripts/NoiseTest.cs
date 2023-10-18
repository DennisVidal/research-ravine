using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System;

public class NoiseTest : MonoBehaviour
{
    public enum LoadingType
    {
        CPU,
        GPU,
        CPU_THREADED,
        GPU_THREADED
    }
    public ComputeShader terrainComputeShader;
    public LoadingType loadType;
    public float[] noiseValues;
    static int CHUNK_SIZE = 32;
    static int SAMPLE_SIZE = CHUNK_SIZE + 1;

    public bool useGPU = false;
    // Start is called before the first frame update
    void Start()
    {
        noiseValues = new float[SAMPLE_SIZE * SAMPLE_SIZE * SAMPLE_SIZE];
        if(loadType == LoadingType.GPU)
        {
            LoadValuesGPU();
        }
        else if (loadType == LoadingType.CPU)
        {
            LoadValuesCPU();
        }
        else if (loadType == LoadingType.CPU_THREADED)
        {
            LoadValuesCPUThreaded();
        }
    }

    void LoadValuesGPU()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        int kernel = terrainComputeShader.FindKernel("Terrain");
        ComputeBuffer valueBuffer = new ComputeBuffer(SAMPLE_SIZE * SAMPLE_SIZE * SAMPLE_SIZE, 4);
        terrainComputeShader.SetBuffer(kernel, "ValueBuffer", valueBuffer);
        terrainComputeShader.SetInt("SampleSize", SAMPLE_SIZE);
        terrainComputeShader.SetVector("Position", gameObject.transform.position);
        terrainComputeShader.Dispatch(kernel, 9, 9, 9);
        valueBuffer.GetData(noiseValues);
        valueBuffer.Dispose();

        sw.Stop();
        UnityEngine.Debug.Log("Time for sampling GPU = " + sw.ElapsedMilliseconds);
    }

    void LoadValuesCPU()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Vector3 position = gameObject.transform.position;
        int SAMPLE_SIZE_SQRT = SAMPLE_SIZE * SAMPLE_SIZE;

        for (int z = 0; z <= CHUNK_SIZE; z++)
        {
            for (int y = 0; y <= CHUNK_SIZE; y++)
            {
                for (int x = 0; x <= CHUNK_SIZE; x++)
                {
                    noiseValues[x + y * SAMPLE_SIZE + z * SAMPLE_SIZE_SQRT] = GetValue(new Vector3(position.x + x, position.y + y, position.z + z));
                }
            }
        }

        sw.Stop();
        UnityEngine.Debug.Log("Time for sampling CPU = " + sw.ElapsedMilliseconds);
    }


    void LoadValuesCPUThreaded()
    {
        StartCoroutine(UpdateSamplesCPUThreaded());
    }

    IEnumerator UpdateSamplesCPUThreaded()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        bool samplesUpdated = false;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        Vector3 position = gameObject.transform.position;
        int SAMPLE_SIZE_SQRT = SAMPLE_SIZE * SAMPLE_SIZE;

        new Thread(() => {
            for (int z = 0; z <= CHUNK_SIZE; z++)
            {
                for (int y = 0; y <= CHUNK_SIZE; y++)
                {
                    for (int x = 0; x <= CHUNK_SIZE; x++)
                    {
                        noiseValues[x + y * SAMPLE_SIZE + z * SAMPLE_SIZE_SQRT] = GetValue(new Vector3(position.x + x, position.y + y, position.z + z));
                    }
                }
            }
            samplesUpdated = true;
        }).Start();

        while (!samplesUpdated)
        {
            yield return null;
        }

        sw.Stop();
        UnityEngine.Debug.Log("Time for sampling CPU Threaded = " + sw.ElapsedMilliseconds);
    }


    public float GetValue(Vector3 pos)
    {
        float value = -pos.y + 5.0f;
        value += PerlinNoise.GetNoise(pos * 0.011f) * 32.1f;
        value += PerlinNoise.GetNoise(pos * 0.021f) * 16.3f;
        value += PerlinNoise.GetNoise(pos * 0.041f) * 8.1f;
        value += PerlinNoise.GetNoise(pos * 0.081f) * 4.1f;
        value += PerlinNoise.GetNoise(pos * 0.161f) * 2.1f;
        value += PerlinNoise.GetNoise(pos * 0.322f) * 1.2f;
        value += PerlinNoise.GetNoise(pos * 0.642f) * 0.6f;
        value += PerlinNoise.GetNoise(pos * 1.282f) * 0.22f;
        value += PerlinNoise.GetNoise(pos * 2.562f) * 0.123f;
        return value;
    }
}
