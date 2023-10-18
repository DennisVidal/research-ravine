using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Collections;



public class Chunk : MonoBehaviour
{
    
    public static int CHUNK_SIZE = 32;
    public static int CHUNK_SIZE_CUBED = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;
    public static int EXPONENT_CHUNK_SIZE = (int)Mathf.Log(CHUNK_SIZE, 2);
    public static int SAMPLE_SIZE = CHUNK_SIZE + 1;
    public static int SAMPLE_SIZE_CUBED = SAMPLE_SIZE * SAMPLE_SIZE * SAMPLE_SIZE;


    public int size;


    public ulong octreeNodeID;

    public ChunkNode chunkNode;

    public Vector3Int samplePosition;


    public ChunkMesh chunkMesh;
    public SeamMesh seamMesh;



    public Vector3Int GetSamplePosition()
    {
        samplePosition = ProceduralWorld.Instance.worldOriginOffset + GetPositionInt();
        return samplePosition;
    }
    public Vector3Int GetMaxSamplePosition()
    {
        int offset = size << EXPONENT_CHUNK_SIZE;
        return GetSamplePosition() + new Vector3Int(offset, offset, offset);
    }


    public void SetOctreeNodeID(ulong id)
    {
        octreeNodeID = id;
    }


    public Vector3Int GetPosition()
    {
        Vector3 pos = gameObject.transform.position;
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }
    public Vector3Int GetPositionInt()
    {
        Vector3 pos = gameObject.transform.position;
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }

    public Vector3Int GetMaxPositionInt()
    {
        int offset = size << EXPONENT_CHUNK_SIZE;
        return GetPositionInt() + new Vector3Int(offset, offset, offset);
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }
    
    public void SetSize(int s)
    {
        size = s;
    }

    public void ShowMesh(bool show)
    {
        chunkMesh.ShowMesh(show);
        seamMesh.ShowMesh(show);
    }
    public void SwitchMesh()
    {
        chunkMesh.SwitchMesh();
        seamMesh.SwitchMesh();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
