using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Collections;
using UnityEngine;
using System;



public class SeamMesh : MonoBehaviour
{
    #region Structs
    public struct SeamData
    {
        public List<SeamChunkMeta> meta;
        public List<SeamChunkData> data;
        public List<int> indexMapping;
        public int totalCellCount;


        public SeamData(List<SeamChunkMeta> meta, List<SeamChunkData> data, List<int> indexMapping, int totalCellCount)
        {
            this.meta = meta;
            this.data = data;
            this.indexMapping = indexMapping;
            this.totalCellCount = totalCellCount;
        }
    }
    public struct SeamChunkMeta
    {

        public int cellOffset;

        public int chunkDataCount;
        public int chunkDataOffset;

        public int arrayStepSize;
        public int arraySize;
        public int indexMappingOffset;

        public SeamChunkMeta(int cellOffset, int chunkDataCount, int chunkDataOffset, int arrayStepSize, int arraySize, int indexMappingOffset)
        {
            this.cellOffset = cellOffset;
            this.chunkDataCount = chunkDataCount;
            this.chunkDataOffset = chunkDataOffset;
            this.arrayStepSize = arrayStepSize;
            this.arraySize = arraySize;
            this.indexMappingOffset = indexMappingOffset;
        }
    }
    public struct SeamChunkData
    {
        public Vector3Int position;
        public int size;
        public int cellOffset;
        public int cellsPerAxis;

        public SeamChunkData(Vector3Int position, int size, int cellOffset, int cellsPerAxis)
        {
            this.position = position;
            this.size = size;
            this.cellOffset = cellOffset;
            this.cellsPerAxis = cellsPerAxis;
        }
    }
    #endregion Structs

    public static int[][] SEAM_MAPPING_AXIS = new int[6][]
    {
        new int[]{ 2,  1},  //x  => z y
        new int[]{ 0,  2},  //y  => x z
        new int[]{ 2, -1},  //xy => z
        new int[]{ 1,  0},  //z  => x y
        new int[]{ 1, -1},  //xz => y
        new int[]{ 0, -1}   //yz => x
    };
    public static int[] SEAM_CHUNK_CELLS = new int[6]
    {
        1024,
        1024,
        32,
        1024,
        32,
        32//,
        //1
    };
    public static Vector3Int[] SEAM_CHUNK_OFFSETS = new Vector3Int[6]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(0, 1, 1)
    };

    public static int MAX_VERTICES = 3000;
    public static int MAX_TRIANGLES = 10000;

    public Chunk chunk;
    public event Action<Chunk> onMeshUpdated;
    [NonSerialized] public bool needsMeshUpdate;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    Mesh newMesh;
    [NonSerialized] public bool isAwaitingMeshUpdate;
    [NonSerialized] public bool isAwaitingVertexUpdate;
    [NonSerialized] public bool isAwaitingNormalUpdate;
    [NonSerialized] public bool isAwaitingTriangleUpdate;
    public ComputeShader computeShader;
    ComputeBuffer vertexBuffer;
    ComputeBuffer normalBuffer;
    ComputeBuffer triangleBuffer;
    ComputeBuffer edgeGroupBuffer;
    ComputeBuffer countsBuffer;
    ComputeBuffer metaBuffer;
    ComputeBuffer dataBuffer;
    ComputeBuffer indexMappingBuffer;


    SeamData seamData;


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





    public void QueueMeshUpdate(List<ChunkOctreeNode>[] seamLeafList)
    {
        List<SeamChunkMeta> metaList = new List<SeamChunkMeta>();
        List<SeamChunkData> dataList = new List<SeamChunkData>();
        List<int> indexMappingList = new List<int>();
        int totalCells = FillSeamInfo(seamLeafList, metaList, dataList, indexMappingList);
        seamData = new SeamData(metaList, dataList, indexMappingList, totalCells);
        needsMeshUpdate = true;
    }

    public int FillSeamInfo(List<ChunkOctreeNode>[] seamLeafList, List<SeamChunkMeta> metaList, List<SeamChunkData> dataList, List<int> indexMappingList)
    {
        int totalCells = 2977;
        Vector3Int minChunkPos = chunk.GetPositionInt();
        Vector3Int maxChunkPos = chunk.GetMaxPositionInt();
        int chunkUnitSize = chunk.size << Chunk.EXPONENT_CHUNK_SIZE;
        for (int i = 0; i < 6; i++)
        {
            List<ChunkOctreeNode> seamLeafs = seamLeafList[i];
            int[] axis = SEAM_MAPPING_AXIS[i];
            Vector3Int seamStartPosition = minChunkPos + SEAM_CHUNK_OFFSETS[i] * chunkUnitSize;
            int minChunkSize = chunk.size;

            SeamChunkMeta meta = new SeamChunkMeta();
            meta.chunkDataOffset = dataList.Count;
            meta.cellOffset = totalCells;
            meta.indexMappingOffset = indexMappingList.Count;
            meta.chunkDataCount = seamLeafs.Count;

            for (int j = 0; j < seamLeafs.Count; j++)
            {
                SeamChunkData data = ConstructSeamChunkData(seamLeafs[j], seamStartPosition, axis, maxChunkPos, totalCells);
                dataList.Add(data);

                minChunkSize = Mathf.Min(minChunkSize, data.size);
                totalCells += (axis[1] != -1 ? data.cellsPerAxis * data.cellsPerAxis : data.cellsPerAxis);
            }

            meta.arrayStepSize = minChunkSize;
            meta.arraySize = chunk.size / minChunkSize;
            metaList.Add(meta);

            if (seamLeafs.Count > 0)
            {
                int minLeafSize = minChunkSize << Chunk.EXPONENT_CHUNK_SIZE;
                int[] indexMapping = GetSeamIndexMapping(seamLeafs, seamStartPosition, axis, meta.arraySize, minLeafSize);
                indexMappingList.AddRange(indexMapping);
            }
        }
        return totalCells;
    }

    SeamChunkData ConstructSeamChunkData(ChunkOctreeNode leaf, Vector3Int seamStartPosition, int[] axis, Vector3Int maxChunkPosition, int totalCellCount)
    {
        Vector3Int seamChunkPosition = GetSeamChunkPosition(seamStartPosition, leaf.GetPositionInt(), axis);
        SeamChunkData data = new SeamChunkData();
        data.size = leaf.GetSize() >> Chunk.EXPONENT_CHUNK_SIZE;
        data.position = ProceduralWorld.Instance.worldOriginOffset + seamChunkPosition;
        data.cellsPerAxis = GetCellsPerAxis(seamChunkPosition, maxChunkPosition, data.size, axis[0]);
        data.cellOffset = totalCellCount;
        return data;
    }
    Vector3Int GetSeamChunkPosition(Vector3Int seamStartPosition, Vector3Int leafPosition, int[] axis)
    {
        Vector3Int seamChunkPosition = seamStartPosition;
        int a0 = axis[0];
        int a1 = axis[1];
        if (seamChunkPosition[a0] < leafPosition[a0])
        {
            seamChunkPosition[a0] = leafPosition[a0];
        }
        if (a1 != -1 && seamChunkPosition[a1] < leafPosition[a1])
        {
            seamChunkPosition[a1] = leafPosition[a1];
        }
        return seamChunkPosition;
    }
    int GetCellsPerAxis(Vector3Int seamChunkPosition, Vector3Int maxChunkPosition, int leafChunkSize, int axis0)
    {
        int cellsPerAxis = (maxChunkPosition[axis0] - seamChunkPosition[axis0]) / leafChunkSize;
        return Mathf.Min(cellsPerAxis, Chunk.CHUNK_SIZE);
    }

    public int[] GetSeamIndexMapping(List<ChunkOctreeNode> seamLeafs, Vector3Int seamStartPosition, int[] axis, int arraySize, int minNodeSize)
    {
        bool isAxis1Valid = axis[1] != -1;
        int[] mappingData = new int[isAxis1Valid ? arraySize * arraySize : arraySize];
        if (arraySize == 1) { return mappingData; } //Only 1 seam chunk with index 0

        for (int i = 0; i < seamLeafs.Count; i++)
        {
            Vector3Int seamLeafPosition = seamLeafs[i].GetPositionInt();
            int startCoord0 = (seamLeafPosition[axis[0]] - seamStartPosition[axis[0]]) / minNodeSize;
            int startCoord1 = isAxis1Valid ? (seamLeafPosition[axis[1]] - seamStartPosition[axis[1]]) / minNodeSize : 0;
            int nodeScale = seamLeafs[i].GetSize() / minNodeSize;
            for (int j = 0; j < nodeScale; j++)
            {
                if (!isAxis1Valid)
                {
                    mappingData[startCoord0 + j] = i;
                    continue;
                }

                for (int k = 0; k < nodeScale; k++)
                {
                    mappingData[startCoord0 + j + arraySize * (startCoord1 + k)] = i;
                }
            }
        }
        return mappingData;
    }



    /*
    public void QueueMeshUpdate_old(List<ChunkOctreeNode>[] seamLeafList)
    {
        List<SeamChunkMeta> metaList = new List<SeamChunkMeta>();
        List<SeamChunkData> dataList = new List<SeamChunkData>();
        List<int> indexMappingList = new List<int>();
        int totalCellCount = 2977; //31*31*3 + 31*3 + 1 = 2977 initial chunk cells

        Vector3Int minChunkPosition = chunk.GetPositionInt();
        Vector3Int maxChunkPosition = chunk.GetMaxPositionInt();

        for (int i = 0; i < 6; i++)
        {
            List<ChunkOctreeNode> seamLeafs = seamLeafList[i];

            int startDataOffset = dataList.Count;
            int startIndexMappingOffset = indexMappingList.Count;
            int startCellCount = totalCellCount;

            int axis0 = SEAM_MAPPING_AXIS[i, 0];
            int axis1 = SEAM_MAPPING_AXIS[i, 1];
            Vector3Int seamChunkStartPosition = minChunkPosition + (SEAM_CHUNK_OFFSETS[i] * (chunk.size << Chunk.EXPONENT_CHUNK_SIZE));
            int minChunkSize = chunk.size;
            for (int j = 0; j < seamLeafs.Count; j++)
            {
                Vector3Int seamLeafPosition = seamLeafs[j].GetPositionInt();
                int seamChunkSize = seamLeafs[j].GetSize() >> Chunk.EXPONENT_CHUNK_SIZE;
                minChunkSize = Mathf.Min(minChunkSize, seamChunkSize);


                Vector3Int seamChunkPosition = seamChunkStartPosition;
                if (minChunkPosition[axis0] < seamLeafPosition[axis0])
                {
                    seamChunkPosition[axis0] = seamLeafPosition[axis0];
                }
                int cellsForChunk = Mathf.Min((maxChunkPosition[axis0] - seamChunkPosition[axis0]) / seamChunkSize, Chunk.CHUNK_SIZE);
                int cellCount = cellsForChunk;

                if (axis1 != -1)
                {
                    cellCount *= cellCount;
                    if (minChunkPosition[axis1] < seamLeafPosition[axis1])
                    {
                        seamChunkPosition[axis1] = seamLeafPosition[axis1];
                    }
                }

                dataList.Add(new SeamChunkData(ProceduralWorld.Instance.worldOriginOffset + seamChunkPosition, seamChunkSize, totalCellCount, cellsForChunk));
                totalCellCount += cellCount;
            }

            int arraySize = chunk.size / minChunkSize;
            metaList.Add(new SeamChunkMeta(startCellCount, seamLeafs.Count, startDataOffset, minChunkSize, arraySize, startIndexMappingOffset));

            if (seamLeafs.Count > 0)
            {
                indexMappingList.AddRange(GetChunkIndexMapping_old(axis0, axis1, arraySize, seamLeafs, minChunkSize << Chunk.EXPONENT_CHUNK_SIZE, seamChunkStartPosition));
            }
        }
        seamData = new SeamData(metaList, dataList, indexMappingList, totalCellCount);
        needsMeshUpdate = true;
    }

    public int[] GetChunkIndexMapping_old(int axis0, int axis1, int arraySize, List<ChunkOctreeNode> seamLeafs, int minNodeSize, Vector3Int seamChunkPosition)
    {
        bool isAxis1Valid = axis1 != -1;
        int[] mappingData = new int[isAxis1Valid ? arraySize * arraySize : arraySize];
        if (arraySize == 1) return mappingData;

        for (int i = 0; i < seamLeafs.Count; i++)
        {
            Vector3Int seamLeafPosition = seamLeafs[i].GetPositionInt();
            int startCoord0 = (seamLeafPosition[axis0] - seamChunkPosition[axis0]) / minNodeSize;
            int startCoord1 = isAxis1Valid ? (seamLeafPosition[axis1] - seamChunkPosition[axis1]) / minNodeSize : 0;

            int nodeScale = seamLeafs[i].GetSize() / minNodeSize;
            if (nodeScale == 1)
            {
                mappingData[startCoord0 + arraySize * startCoord1] = i;
                continue;
            }

            for (int j = 0; j < nodeScale; j++)
            {
                if (!isAxis1Valid)
                {
                    mappingData[startCoord0 + j] = i;
                    continue;
                }

                for (int k = 0; k < nodeScale; k++)
                {
                    mappingData[startCoord0 + j + arraySize * (startCoord1 + k)] = i;
                }
            }

        }
        return mappingData;
    }
    */


    public void ClearBuffer()
    {
        metaBuffer?.Dispose();
        dataBuffer?.Dispose();
        indexMappingBuffer?.Dispose();
        edgeGroupBuffer?.Dispose();
        vertexBuffer?.Dispose();
        normalBuffer?.Dispose();
        triangleBuffer?.Dispose();
        countsBuffer?.Dispose();
    }
    public void UpdateMesh()
    {
        isAwaitingMeshUpdate = true;
        if (seamData.totalCellCount == 2977)
        {
            OnMeshUpdated();
            return;
        }
        ClearBuffer();
        metaBuffer = new ComputeBuffer(seamData.meta.Count, 24);
        metaBuffer.SetData(seamData.meta);
        dataBuffer = new ComputeBuffer(seamData.data.Count, 24);
        dataBuffer.SetData(seamData.data);
        indexMappingBuffer = new ComputeBuffer(seamData.indexMapping.Count, 4);
        indexMappingBuffer.SetData(seamData.indexMapping);
        edgeGroupBuffer = new ComputeBuffer(seamData.totalCellCount, 48);
        edgeGroupBuffer.SetData(new int[seamData.totalCellCount * 12]);
        countsBuffer = new ComputeBuffer(2, 4);
        countsBuffer.SetData(new int[2] { 0, 0 });
        vertexBuffer = new ComputeBuffer(MAX_VERTICES, 12);
        normalBuffer = new ComputeBuffer(MAX_VERTICES, 12);
        triangleBuffer = new ComputeBuffer(MAX_TRIANGLES, 4);

        computeShader.SetBuffer(0, "metaBuffer", metaBuffer);
        computeShader.SetBuffer(0, "dataBuffer", dataBuffer);
        computeShader.SetBuffer(0, "indexMappingBuffer", indexMappingBuffer);
        computeShader.SetBuffer(0, "edges", edgeGroupBuffer);
        computeShader.SetBuffer(0, "countsBuffer", countsBuffer);
        computeShader.SetBuffer(0, "vertexBuffer", vertexBuffer);
        computeShader.SetBuffer(0, "normalBuffer", normalBuffer);

        computeShader.SetBuffer(1, "metaBuffer", metaBuffer);
        computeShader.SetBuffer(1, "dataBuffer", dataBuffer);
        computeShader.SetBuffer(1, "indexMappingBuffer", indexMappingBuffer);
        computeShader.SetBuffer(1, "edges", edgeGroupBuffer);
        computeShader.SetBuffer(1, "countsBuffer", countsBuffer);
        computeShader.SetBuffer(1, "triangleBuffer", triangleBuffer);


        Vector3Int samplePosition = chunk.GetSamplePosition();
        computeShader.SetInts("chunkPosition", new int[3] { samplePosition.x, samplePosition.y, samplePosition.z });

        Vector3Int maxSamplePosition = chunk.GetMaxSamplePosition();
        computeShader.SetInts("maxChunkPosition", new int[3] { maxSamplePosition.x, maxSamplePosition.y, maxSamplePosition.z });
        computeShader.SetInts("maxChunkPositionMinusSize", new int[3] { maxSamplePosition.x - chunk.size, maxSamplePosition.y - chunk.size, maxSamplePosition.z - chunk.size });
        computeShader.SetInt("chunkSize", chunk.size);
        computeShader.SetInt("totalCellCount", seamData.totalCellCount);

        computeShader.SetFloat("terrainWidthFactor", ProceduralWorld.Instance.GetTerrainWidthFactor());

        //Drill Ray
        DrillRay drillRay = DrillRay.GetActiveDrillRay();
        computeShader.SetVector("drillRayStart", drillRay.GetStartPosition());
        computeShader.SetVector("drillRayEnd", drillRay.GetEndPosition());
        computeShader.SetVector("drillRayData", new Vector3(drillRay.GetMinRadius(), drillRay.GetMaxRadius(), drillRay.GetStrength()));
        computeShader.SetBool("drillRayEnabled", drillRay.IsEnabled());


        //int threadGroupSize = (seamData.totalCellCount - 1) / 32;
        int threadGroupSize = (int)Mathf.Ceil(seamData.totalCellCount / 32.0f);
        computeShader.Dispatch(0, threadGroupSize, 1, 1);

        computeShader.Dispatch(1, 93, 1, 1);

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
            metaBuffer.Dispose();
            dataBuffer.Dispose();
            indexMappingBuffer.Dispose();
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
        if (newMesh != null)
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
        if (edgeGroupBuffer != null)//TODO: Add better check
        {
            edgeGroupBuffer.Dispose();
            metaBuffer.Dispose();
            dataBuffer.Dispose();
            indexMappingBuffer.Dispose();
        }

        EnableMeshCollider(chunk.size <= 2);

        isAwaitingMeshUpdate = false;
        if (onMeshUpdated != null)
        {
            onMeshUpdated(chunk);
        }
    }

    public void EnableMeshCollider(bool enable)
    {
        if (meshCollider == null)
        {
            return;
        }

        if (enable && newMesh && newMesh.triangles.Length != 0)
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