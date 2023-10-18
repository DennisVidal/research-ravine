using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Collections;
using UnityEngine;
using System;

public class ChunkMesh : MonoBehaviour
{

    public static int MAX_VERTICES = 10000;
    public static int MAX_TRIANGLES = 60000;

    public Chunk chunk;
    public event Action<Chunk> onMeshUpdated;
    [NonSerialized] public bool needsMeshUpdate;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    [NonSerialized] public Mesh newMesh;
    [NonSerialized] public bool isAwaitingMeshUpdate;
    [NonSerialized] public bool isAwaitingNormalUpdate;
    [NonSerialized] public bool isAwaitingVertexUpdate;
    [NonSerialized] public bool isAwaitingTriangleUpdate;
    public ComputeShader computeShader;
    ComputeBuffer vertexBuffer;
    ComputeBuffer normalBuffer;
    ComputeBuffer triangleBuffer;
    ComputeBuffer edgeGroupBuffer;
    ComputeBuffer countsBuffer;
    ComputeBuffer sampleBuffer;

    void Update()
    {
        if (needsMeshUpdate)
        {
            needsMeshUpdate = false;
            UpdateMesh();
        }
    }

    public void SwitchMesh()
    {
        meshFilter.sharedMesh = newMesh;
    }

    public void QueueMeshUpdate()
    {
        needsMeshUpdate = true;
    }

    public void UpdateMesh()
    {
        isAwaitingMeshUpdate = true;
        vertexBuffer = new ComputeBuffer(MAX_VERTICES, 12);
        normalBuffer = new ComputeBuffer(MAX_VERTICES, 12);
        triangleBuffer = new ComputeBuffer(MAX_TRIANGLES, 4);
        edgeGroupBuffer = new ComputeBuffer(Chunk.CHUNK_SIZE_CUBED, 48);
        edgeGroupBuffer.SetData(new int[Chunk.CHUNK_SIZE_CUBED * 12]);
        countsBuffer = new ComputeBuffer(2, 4);
        countsBuffer.SetData(new int[2] { 0, 0 });
        sampleBuffer = new ComputeBuffer(Chunk.SAMPLE_SIZE_CUBED, 16);

        computeShader.SetBuffer(0, "sampleBuffer", sampleBuffer);
        
        computeShader.SetBuffer(1, "sampleBuffer", sampleBuffer);
        computeShader.SetBuffer(1, "edgeGroupBuffer", edgeGroupBuffer);
        computeShader.SetBuffer(1, "countsBuffer", countsBuffer);
        computeShader.SetBuffer(1, "vertexBuffer", vertexBuffer);
        computeShader.SetBuffer(1, "normalBuffer", normalBuffer);
        
        computeShader.SetBuffer(2, "sampleBuffer", sampleBuffer);
        computeShader.SetBuffer(2, "edgeGroupBuffer", edgeGroupBuffer);
        computeShader.SetBuffer(2, "countsBuffer", countsBuffer);
        computeShader.SetBuffer(2, "triangleBuffer", triangleBuffer);

        Vector3Int samplePosition = chunk.GetSamplePosition();
        computeShader.SetInts("chunkPosition", new int[3] { samplePosition.x, samplePosition.y, samplePosition.z });
        computeShader.SetInt("chunkSize", chunk.size);

        computeShader.SetFloat("terrainWidthFactor", ProceduralWorld.Instance.GetTerrainWidthFactor());

        //Drill Ray
        DrillRay drillRay = DrillRay.GetActiveDrillRay();
        computeShader.SetVector("drillRayStart", drillRay.GetStartPosition());
        computeShader.SetVector("drillRayEnd", drillRay.GetEndPosition());
        computeShader.SetVector("drillRayData", new Vector3(drillRay.GetMinRadius(), drillRay.GetMaxRadius(), drillRay.GetStrength()));
        computeShader.SetBool("drillRayEnabled", drillRay.IsEnabled());


        //Sample
        computeShader.Dispatch(0, 1123, 1, 1);
        //Create
        computeShader.Dispatch(1, 8, 8, 8);
        //Connect
        computeShader.Dispatch(2, 8, 8, 8);

        AsyncGPUReadback.Request(countsBuffer, OnCountsBufferReady);
    }


    public void OnCountsBufferReady(AsyncGPUReadbackRequest request)
    {
        NativeArray<int> counts = request.GetData<int>();
        countsBuffer.Dispose();
        if (counts[0] == 0 || counts[1] == 0)
        {
            vertexBuffer.Dispose();
            normalBuffer.Dispose();
            triangleBuffer.Dispose();
            edgeGroupBuffer.Dispose();
            newMesh = null;
            isAwaitingVertexUpdate = false;
            isAwaitingNormalUpdate = false;
            isAwaitingTriangleUpdate = false;
            OnMeshPartUpdated();
            return;
        }

        isAwaitingVertexUpdate = true;
        isAwaitingNormalUpdate = true;
        isAwaitingTriangleUpdate = true;
        newMesh = new Mesh();

        AsyncGPUReadback.Request(vertexBuffer, counts[0] * 12, 0, OnVertexBufferReady);
        AsyncGPUReadback.Request(normalBuffer, counts[0] * 12, 0, OnNormalBufferReady);
        AsyncGPUReadback.Request(triangleBuffer, counts[1] * 4, 0, OnTriangleBufferReady);
    }
    public void OnVertexBufferReady(AsyncGPUReadbackRequest request)
    {
        if(newMesh != null)
        {
            newMesh.vertices = request.GetData<Vector3>().ToArray();
        }
        vertexBuffer.Dispose();
        isAwaitingVertexUpdate = false;
        OnMeshPartUpdated();
    }
    public void OnNormalBufferReady(AsyncGPUReadbackRequest request)
    {
        if (newMesh != null)
        {
            newMesh.normals = request.GetData<Vector3>().ToArray();
        }
        normalBuffer.Dispose();
        isAwaitingNormalUpdate = false;
        OnMeshPartUpdated();
    }

    public void OnTriangleBufferReady(AsyncGPUReadbackRequest request)
    {
        if (newMesh != null)
        {
            newMesh.triangles = request.GetData<int>().ToArray();
        }
        triangleBuffer.Dispose();
        isAwaitingTriangleUpdate = false;
        OnMeshPartUpdated();
    }

    public bool IsAwaitingUpdate()
    {
        return isAwaitingVertexUpdate || isAwaitingNormalUpdate || isAwaitingTriangleUpdate || !isAwaitingMeshUpdate;
    }
    void OnMeshPartUpdated()
    {
        if (IsAwaitingUpdate())
        {
            return;
        }

        OnMeshUpdated();
    }
    void OnMeshUpdated()
    {
        edgeGroupBuffer.Dispose();
        sampleBuffer.Dispose();

        EnableMeshCollider(chunk.size <= 2);

        isAwaitingMeshUpdate = false;
        if (onMeshUpdated != null)
        {
            onMeshUpdated(chunk);
        }
    }

    public void EnableMeshCollider(bool enable)
    {
        if(meshCollider == null)
        {
            return;
        }

        if(enable && newMesh && newMesh.triangles.Length != 0)
        {
            meshCollider.sharedMesh = newMesh;
            meshCollider.enabled = true;
            return;
        }

        meshCollider.enabled = false;
    }
    public void ShowMesh(bool show)
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = show;
        }
    }
}
