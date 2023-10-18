using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkNode : MonoBehaviour
{
    public Dictionary<Vector3Int, Chunk> chunks;//Should use hashtable instead

    
    public ChunkNode()
    {
        chunks = new Dictionary<Vector3Int, Chunk>();
    }

    public void AddChunk(Vector3Int position, Chunk chunk)
    {
        chunk.transform.parent = transform;
        chunk.chunkNode = this;
        chunks.Add(position, chunk);
    }

    public bool RemoveChunk(Vector3Int position)
    {
        return chunks.Remove(position);
    }
}
