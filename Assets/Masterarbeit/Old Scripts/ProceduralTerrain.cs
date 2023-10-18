using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;


public class ProceduralTerrain : MonoBehaviour
{
    /*
    static int PRELOADED_CHUNK_POOL_OBJECTS = 128;
    public static int VIEW_DISTANCE = 512;
    public static ProceduralTerrain instance = null;


    public Dictionary<Vector3Int, Chunk> activeChunks;

    public bool isAwaitingChunkUpdate;
    public List<Chunk> chunksToFree;
    public List<Chunk> updatingChunks;
    public int updatedChunksCount;
    public List<Chunk> updatingSeams;
    public int updatedSeamsCount;

    public bool isUpdateCoroutineRunning;


    public int testMod = 10;
    public int testMod2 = 10;





    private void Start()
    {
        instance = this;


        activeChunks = new Dictionary<Vector3Int, Chunk>();
        chunksToFree = new List<Chunk>();
        updatingChunks = new List<Chunk>();
        updatingSeams = new List<Chunk>();


        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 90;

        InitChunkOctree();
        InitWater();
    }

    private void Update()
    {
        UpdateChunks();
    }




    void InitChunkOctree()
    {
        chunkOctree = new ChunkOctree(Camera.main, VIEW_DISTANCE);
        chunkOctree.onPositionsUpdated += OnChunkOctreePositionsUpdated;
    }

    void InitWater()
    {
        water = Instantiate(waterPrefab);
        Vector3 centerPosition = chunkOctree.GetCenterPosition();
        centerPosition.y = waterLevel;
        water.transform.position = centerPosition;
        water.transform.localScale = new Vector3(VIEW_DISTANCE, 1.0f, VIEW_DISTANCE);
    }


    void OnChunkOctreePositionsUpdated()
    {
        Vector3 centerPosition = chunkOctree.GetCenterPosition();
        centerPosition.y = waterLevel;
        water.transform.position = centerPosition;
    }


    void UpdateChunks()
    {
        if(isUpdateCoroutineRunning)
        {
            return;
        }

        //No update in progress => update octree
        if(!isAwaitingChunkUpdate)
        {
            UpdateOctree();
            return;
        }


        //update in progress but no chunks need updating => free unused chunks
        if (updatingChunks.Count == 0 && updatingSeams.Count == 0)
        {
            if (chunksToFree.Count > 0)
            {
                FreeChunks(chunksToFree);
                chunksToFree.Clear();
            }
            isAwaitingChunkUpdate = false;
            return;
        }

        //update in progress, but not done yet
        if (updatedChunksCount < updatingChunks.Count || updatedSeamsCount < updatingSeams.Count)
        {
            return;
        }

        //update done => free unused chunks and switch them with updated ones
        if (chunksToFree.Count > 0)
        {
            FreeChunks(chunksToFree);
            chunksToFree.Clear();
        }

        for (int i = 0; i < updatingChunks.Count; i++)
        {
            updatingChunks[i].ShowMesh();
            updatingChunks[i].SwitchChunkMesh();
        }
        updatingChunks.Clear();
        updatedChunksCount = 0;

        for (int i = 0; i < updatingSeams.Count; i++)
        {
            updatingSeams[i].SwitchSeamMesh();
        }
        updatingSeams.Clear();
        updatedSeamsCount = 0;

        isAwaitingChunkUpdate = false;
    }

    public void UpdateOctree()
    {
        //Update sub-octree positing and immediately free chunks outside the octree range
        FreeChunks(chunkOctree.UpdatePositions());
        StartCoroutine(UpdateOctreeSubdivisionCoroutine());
    }

    void RemoveChunk(Vector3Int chunkPosition)
    {
        if (activeChunks.TryGetValue(chunkPosition, out Chunk chunk))
        {
            chunksToFree.Add(chunk);
            activeChunks.Remove(chunkPosition);
        }
    }

    Chunk AddChunk(Vector3Int chunkPosition, int chunkSize, ulong octreeNodeID = 0)
    {
        Chunk chunk = GetFreeChunk();
        activeChunks.Add(chunkPosition, chunk);
        chunk.Show();
        chunk.HideMesh();
        chunk.SetPosition(chunkPosition);
        chunk.SetSize(chunkSize);
        chunk.SetOctreeNodeID(octreeNodeID);
        return chunk;
    }

    IEnumerator UpdateOctreeSubdivisionCoroutine()
    {
        isUpdateCoroutineRunning = true;
        isAwaitingChunkUpdate = true;
        List<ChunkOctreeNode> leafNodes = new List<ChunkOctreeNode>();
        List<ChunkOctreeNode> removedLeafNodes = new List<ChunkOctreeNode>();
        chunkOctree.UpdateSubdivision(leafNodes, removedLeafNodes);
        if (leafNodes.Count == 0 && removedLeafNodes.Count == 0)
        {
            isUpdateCoroutineRunning = false;
            isAwaitingChunkUpdate = false;
            yield break;
        }
        yield return null;


        for (int i = 0; i < removedLeafNodes.Count; i++)
        {
            RemoveChunk(removedLeafNodes[i].position);
        }

        for (int i = 0; i < leafNodes.Count; i++)
        {
            if (i % testMod == 0)
            {
                yield return null;
            }

            Vector3Int chunkPosition = leafNodes[i].position;
        
            //if (!leafNodes[i].isVisible)
            //{
            //    RemoveChunk(chunkPosition);
            //    continue;
            //}
        
            if(!activeChunks.TryGetValue(chunkPosition, out Chunk chunk))
            {
                chunk = AddChunk(leafNodes[i].position, leafNodes[i].size >> Chunk.EXPONENT_CHUNK_SIZE, leafNodes[i].id);
                updatingChunks.Add(chunk);
                chunk.onChunkMeshUpdated += OnChunkMeshUpdated;
                chunk.QueueChunkMeshUpdate();
            }
        }


        Dictionary<Vector3Int, Chunk> seamUpdatingChunks = new Dictionary<Vector3Int, Chunk>();
        for (int i = 0; i < updatingChunks.Count; i++)
        {
            if(i % testMod2 == 0)
            {
                yield return null;
            }
        
            //All chunks that update their meshes also need to update their seams
            if (!seamUpdatingChunks.ContainsKey(updatingChunks[i].GetPositionInt()))
            {
                updatingChunks[i].onSeamMeshUpdated += OnSeamMeshUpdated;
                updatingChunks[i].QueueSeamMeshUpdate(chunkOctree.GetSeamChunks(updatingChunks[i].octreeNodeID));
                updatingSeams.Add(updatingChunks[i]);
        
                seamUpdatingChunks.Add(updatingChunks[i].GetPositionInt(), updatingChunks[i]);
            }
        
            List<ChunkOctreeNode> seamNeighbours = new List<ChunkOctreeNode>();
            chunkOctree.GetNeighbouringNodesToUpdate(updatingChunks[i].octreeNodeID, seamNeighbours);
            for (int j = 0; j < seamNeighbours.Count; j++)
            {
                if (!activeChunks.TryGetValue(seamNeighbours[j].position, out Chunk chunk))
                {
                    continue;
                }
        
        
                //if (!seamNeighbours[j].isVisible)
                //{
                //    continue;
                //}
        
                if (!seamUpdatingChunks.ContainsKey(chunk.GetPositionInt()))
                {
                    chunk.onSeamMeshUpdated += OnSeamMeshUpdated;
                    chunk.QueueSeamMeshUpdate(chunkOctree.GetSeamChunks(seamNeighbours[j].id));
                    updatingSeams.Add(chunk);
        
                    seamUpdatingChunks.Add(chunk.GetPositionInt(), chunk);
                }
            }
        }
        isUpdateCoroutineRunning = false;
    }

    public void OnChunkMeshUpdated(Chunk updatedChunk)
    {
        updatedChunk.onChunkMeshUpdated -= OnChunkMeshUpdated;
        ++updatedChunksCount;
    }
    public void OnSeamMeshUpdated(Chunk updatedChunk)
    {
        updatedChunk.onSeamMeshUpdated -= OnSeamMeshUpdated;
        ++updatedSeamsCount;
    }

    
    public void FreeChunks(List<Chunk> chunks)
    {
        for(int i = 0; i < chunks.Count; i++)
        {
            FreeChunk(chunks[i]);
        }
    }
    public void FreeChunks(List<Vector3Int> chunkPositions)
    {
        for (int i = 0; i < chunkPositions.Count; i++)
        {
            FreeChunk(chunkPositions[i]);
        }
    }
    public void FreeChunk(Chunk chunk)
    {
        //activeChunks.Remove(chunk.GetPositionInt());
        chunk.Hide();
        chunkPool.Enqueue(chunk);
    }
    public void FreeChunk(Vector3Int chunkPosition)
    {
        if (activeChunks.TryGetValue(chunkPosition, out Chunk chunk))
        {
            activeChunks.Remove(chunkPosition);
            FreeChunk(chunk);
        }
    }

    private void OnDrawGizmos()
    {
        if(chunkOctree != null)
        {
            chunkOctree.DrawDebugCube();
        }
    }
    */
}