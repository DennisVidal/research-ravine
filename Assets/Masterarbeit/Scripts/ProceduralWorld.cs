using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWorld : MonoBehaviour
{
    public static ProceduralWorld Instance;

    public static int PRELOADED_CHUNK_POOL_OBJECTS = 128;

    public static int CHUNK_NODE_SIZE = 512;
    public static int HALF_CHUNK_NODE_SIZE = CHUNK_NODE_SIZE >> 1;
    public static float INVERSE_CHUNK_NODE_SIZE = 1.0f / CHUNK_NODE_SIZE;
    public static float INVERSE_HALF_CHUNK_NODE_SIZE = 1.0f / HALF_CHUNK_NODE_SIZE;
    public static int[] CHUNK_NODE_OFFSETS = new int[3] { -(CHUNK_NODE_SIZE + HALF_CHUNK_NODE_SIZE), -HALF_CHUNK_NODE_SIZE, HALF_CHUNK_NODE_SIZE };
    public static int TOTAL_LOADED_CHUNKS_COUNT;



    [System.NonSerialized] public Vector3Int worldOriginOffset;

    [System.NonSerialized] public SpawnPointFinder spawnPointFinder;
    [System.NonSerialized] public Vector3 seedOffset;


    public Camera mainCamera;

    public GameObject chunkPrefab;

    public GameObject waterPrefab;
    public float waterLevel = 5.0f;
    [System.NonSerialized] public GameObject water;

    public GameObject sandstormPrefab;
    public float sandstormHeight = 180.0f;

    [System.NonSerialized] public ChunkOctree chunkOctree;

    [System.NonSerialized] public ChunkNode[,] chunkNodes;




     public bool isAwaitingTerrainUpdate;
    [System.NonSerialized] public List<Chunk> chunksToFree;
    [System.NonSerialized] public List<Chunk> updatingChunks;
    [System.NonSerialized] public List<Chunk> updatingSeams;
    [System.NonSerialized] public int updatedChunksCount;
    [System.NonSerialized] public int updatedSeamsCount;
     public bool isUpdateCoroutineRunning;
    [System.NonSerialized] public HashSet<Vector3Int> chunksToPotentiallyFree;

    [System.NonSerialized] public Dictionary<Vector3Int, Chunk> activeChunks;

    [System.NonSerialized] public Queue<Chunk> chunkPool;
    [System.NonSerialized] public GameObject chunkPoolObject;


    public int chunkMeshQueuesPerFrame = 10;
    public int seamMeshQueuesPerFrame = 10;


    //Drill Ray
    [System.NonSerialized] public DrillRay drillRay;
    [System.NonSerialized] public bool drillRayStateChanged;

    List<ChunkOctreeNode> leafNodes = new List<ChunkOctreeNode>();
    List<ChunkOctreeNode> oldLeafNodes = new List<ChunkOctreeNode>();


    public GameObject playerObject;

    [System.NonSerialized] public bool isWorldGenerated = false;
    [System.NonSerialized] public float worldGenerationProgress = 0.0f;

    public event System.Action<Vector3Int, Vector3Int> onWorldOriginChanged;


    public Material terrainMaterial;


    void Awake()
    {
        Instance = this;

        chunksToFree = new List<Chunk>();
        updatingChunks = new List<Chunk>();
        updatingSeams = new List<Chunk>();
        chunksToPotentiallyFree = new HashSet<Vector3Int>();
        activeChunks = new Dictionary<Vector3Int, Chunk>();

        terrainMaterial.SetVector("WorldOriginOffset", Vector4.zero);

       // material.SetFloat("DrillRayEnabled", 0.0f);
    }

    void InitWorldSeed()
    {
        Random.InitState(SaveSystem.SAVE_DATA.GetLastSeed());
        float seedOffsetRange = 100000.0f;
        seedOffset.x = Random.Range(-seedOffsetRange, seedOffsetRange);
        seedOffset.y = Random.Range(-seedOffsetRange, seedOffsetRange);
        seedOffset.z = Random.Range(-seedOffsetRange, seedOffsetRange);
    }
    void InitSpawnPointFinder()
    {
        spawnPointFinder = GetComponent<SpawnPointFinder>();
        if (spawnPointFinder == null)
        {
            spawnPointFinder = gameObject.AddComponent<SpawnPointFinder>();
        }
    }
    void InitChunkPool()
    {
        chunkPool = new Queue<Chunk>();
        chunkPoolObject = new GameObject("Chunk Pool Object");
        for (int i = 0; i < PRELOADED_CHUNK_POOL_OBJECTS; i++)
        {
            AddNewChunkToPool();
        }
    }
    void InitChunkNodes()
    {
        chunkNodes = new ChunkNode[3, 3];
        Vector3Int nodePosition = new Vector3Int();
        for (int x = 0; x < 3; x++)
        {
            nodePosition.x = CHUNK_NODE_OFFSETS[x];
            for (int z = 0; z < 3; z++)
            {
                nodePosition.z = CHUNK_NODE_OFFSETS[z];
                GameObject obj = new GameObject("Chunk Node [" + x + ", " + z + "]");
                obj.transform.position = nodePosition;
                chunkNodes[x, z] = obj.AddComponent<ChunkNode>();


                GameObject sandstormObj = Instantiate(sandstormPrefab, obj.transform);
                sandstormObj.transform.position += new Vector3(CHUNK_NODE_SIZE / 2, sandstormHeight, CHUNK_NODE_SIZE / 2);
            }
        }
    }
    void InitWater()
    {
        water = Instantiate(waterPrefab);
        water.name = "Water";
        water.transform.position = new Vector3(0, waterLevel, 0);
        water.transform.localScale = new Vector3(CHUNK_NODE_SIZE, 1.0f, CHUNK_NODE_SIZE);
    }

    public void GenerateWorld()
    {
        StartCoroutine(GenerateWorldCoroutine());
    }
    IEnumerator GenerateWorldCoroutine()
    {
        mainCamera = Camera.main;
        chunkOctree = new ChunkOctree(mainCamera, CHUNK_NODE_SIZE);
        
        InitWorldSeed();
        InitSpawnPointFinder();
        InitChunkPool();
        InitChunkNodes();
        InitWater();

        FindAndSetSpawnPoint();
        worldGenerationProgress = 0.2f;
        yield return null;

        do
        {
            UpdateTerrain();
            if (!isUpdateCoroutineRunning)
            {
                float meshUpdateProgress = 0.5f*(updatingChunks.Count / Mathf.Max(updatedChunksCount, 1.0f) + updatingSeams.Count / Mathf.Max(updatedSeamsCount, 1.0f));
                worldGenerationProgress = Mathf.Clamp01(0.2f + 0.8f * meshUpdateProgress);
            }
            yield return null;
        }
        while (isAwaitingTerrainUpdate);
        isWorldGenerated = true;
    }



    void Start()
    {
        drillRay = DrillRay.GetActiveDrillRay();
        drillRay.onFire += OnDrillRayFired;
        drillRay.onExpire += OnDrillRayExpired;
    }

    private void Update()
    {
        if(isWorldGenerated)
        {
            UpdateTerrain();
        }
    }

    void OnDrillRayFired()
    {
        drillRayStateChanged = true;
    }
    
    void OnDrillRayExpired()
    {
        drillRayStateChanged = true;
    }

    void UpdateTerrain()
    {
        if (isUpdateCoroutineRunning)
        {
            return;
        }

        if (!isAwaitingTerrainUpdate)
        {
            StartCoroutine(UpdateTerrainCoroutine());
            return;
        }

        //update in progress, but not done yet
        if (updatedChunksCount < updatingChunks.Count || updatedSeamsCount < updatingSeams.Count)
        {
            return;
        }

        //update done => free unused chunks and switch them with updated ones
        for (int i = 0; i < chunksToFree.Count; i++)
        {
            chunksToFree[i].chunkNode = null;
            chunksToFree[i].transform.parent = chunkPoolObject.transform;
            chunksToFree[i].Hide();
            chunkPool.Enqueue(chunksToFree[i]);
        }
        chunksToFree.Clear();

        for (int i = 0; i < updatingChunks.Count; i++)
        {
            updatingChunks[i].ShowMesh(true);
            updatingChunks[i].chunkMesh.SwitchMesh();
        }
        updatingChunks.Clear();
        updatedChunksCount = 0;

        for (int i = 0; i < updatingSeams.Count; i++)
        {
            updatingSeams[i].seamMesh.SwitchMesh();
        }
        updatingSeams.Clear();
        updatedSeamsCount = 0;

        isAwaitingTerrainUpdate = false;
    }
    void AddNewChunkToPool()
    {
        Chunk chunk = InstantiateNewChunk();
        chunk.ShowMesh(false);
        chunk.Hide();
        chunkPool.Enqueue(chunk);
    }
    Chunk InstantiateNewChunk()
    {
        GameObject obj = Instantiate(chunkPrefab, chunkPoolObject.transform);
        obj.name = "Chunk " + TOTAL_LOADED_CHUNKS_COUNT++;
        return obj.GetComponent<Chunk>();
    }
    public Chunk GetFreeChunk()
    {
        if (chunkPool.Count == 0)
        {
            AddNewChunkToPool();
        }
        return chunkPool.Dequeue();
    }

    public void FreeNodeChunks(ChunkNode node)
    {
        Dictionary<Vector3Int, Chunk>.KeyCollection keys = node.chunks.Keys;
        foreach (Vector3Int key in keys)
        {
            if (activeChunks.TryGetValue(key, out Chunk chunk))
            {
                activeChunks.Remove(key);
                chunk.transform.parent = chunkPoolObject.transform;
                chunk.Hide();
                chunkPool.Enqueue(chunk);
            }
        }
        node.chunks.Clear();
    }





    public void UpdateOrigin()
    {
        Vector3 targetPosition = mainCamera.transform.position;
        //INVERSE_CHUNK_NODE_SIZE = 1/512
        int x = Mathf.RoundToInt(targetPosition.x * INVERSE_CHUNK_NODE_SIZE);
        int z = Mathf.RoundToInt(targetPosition.z * INVERSE_CHUNK_NODE_SIZE);
        if (x == 0 && z == 0)
        {
            return;
        }

        Vector3Int chunkNodeOffset = new Vector3Int(x, 0, z);
        Vector3Int offset = chunkNodeOffset * CHUNK_NODE_SIZE;
        worldOriginOffset += offset;
        OnWorldOriginChanged(offset, chunkNodeOffset);
    }
    void OnWorldOriginChanged(Vector3Int offset, Vector3Int chunkNodeOffset)
    {
        if(onWorldOriginChanged != null)
        {
            onWorldOriginChanged.Invoke(offset, chunkNodeOffset);
        }

        terrainMaterial.SetVector("WorldOriginOffset", new Vector4(worldOriginOffset.x, worldOriginOffset.y, worldOriginOffset.z, 0.0f));

        ChunkNode[,] oldChunkNodes = (ChunkNode[,])chunkNodes.Clone();
        int modX = (-chunkNodeOffset.x) % 3;
        int modZ = (-chunkNodeOffset.z) % 3;
        int[] newPosX = new int[3] { (modX + 3) % 3, (modX + 4) % 3, (modX + 5) % 3 };
        int[] newPosZ = new int[3] { (modZ + 3) % 3, (modZ + 4) % 3, (modZ + 5) % 3 };

        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                ChunkNode node = oldChunkNodes[x, z];
                int newX = newPosX[x];
                int newZ = newPosZ[z];
                node.transform.position = new Vector3Int(CHUNK_NODE_OFFSETS[newX], 0, CHUNK_NODE_OFFSETS[newZ]);
                chunkNodes[newX, newZ] = node;

                int offsetX = x - chunkNodeOffset.x;
                int offsetZ = z - chunkNodeOffset.z;
                if (offsetX < 0 || 2 < offsetX || offsetZ < 0 || 2 < offsetZ)
                {
                    FreeNodeChunks(node);
                    continue;
                }
                chunksToPotentiallyFree.UnionWith(node.chunks.Keys);
            }
        }
    }


    void QueueNewChunk(ChunkOctreeNode correspondingNode)
    {
        Chunk chunk = AddChunk(correspondingNode);
        updatingChunks.Add(chunk);
        chunk.chunkMesh.onMeshUpdated += OnChunkMeshUpdated;
        chunk.chunkMesh.QueueMeshUpdate();
    }
    void FreeOldChunk(Chunk chunk)
    {
        Vector3Int chunkPosition = chunk.GetSamplePosition();
        activeChunks.Remove(chunkPosition);
        chunk.chunkNode.chunks.Remove(chunkPosition);
        chunksToFree.Add(chunk);
    }

    IEnumerator UpdateTerrainCoroutine()
    {
        isUpdateCoroutineRunning = true;
        isAwaitingTerrainUpdate = true;
        UpdateOrigin();

        leafNodes.Clear();
        oldLeafNodes.Clear();
        chunkOctree.UpdateLeafNodes(leafNodes, oldLeafNodes);

        //Queue old chunks to be freed
        for (int i = 0; i < oldLeafNodes.Count; i++)
        {
            Vector3Int chunkPosition = worldOriginOffset + oldLeafNodes[i].GetPositionInt();
            if (activeChunks.TryGetValue(chunkPosition, out Chunk chunk))
            {
                chunksToPotentiallyFree.Remove(chunkPosition);
                FreeOldChunk(chunk);
            }
        }

        //Queue new chunks
        for (int i = 0; i < leafNodes.Count; i++)
        {
            if (i % chunkMeshQueuesPerFrame == 0)
            {
                yield return null;
            }

            Vector3Int chunkPosition = worldOriginOffset + leafNodes[i].GetPositionInt();

            //A leaf node for that chunk exists -> doesn't need to be freed later anymore
            chunksToPotentiallyFree.Remove(chunkPosition);

            //Chunk doesn't exist yet -> queue new one
            if (!activeChunks.TryGetValue(chunkPosition, out Chunk chunk))
            {
                QueueNewChunk(leafNodes[i]);
                continue;
            }

            //Chunk exists, but the size is wrong -> free the old one and queue a new one
            if ((leafNodes[i].GetSize() >> Chunk.EXPONENT_CHUNK_SIZE) != chunk.size)
            {
                FreeOldChunk(chunk);
                QueueNewChunk(leafNodes[i]);
                continue;
            }
            chunk.SetOctreeNodeID(leafNodes[i].id);
        }

        if (drillRayStateChanged)
        {
            drillRayStateChanged = false;
            List<ChunkOctreeNode> intersectedLeafs = new List<ChunkOctreeNode>();
            chunkOctree.FindDrillRayIntersectedLeafs(intersectedLeafs);
            for (int i = 0; i < intersectedLeafs.Count; i++)
            {
                if (i % chunkMeshQueuesPerFrame == 0)
                {
                    yield return null;
                }

                Vector3Int chunkPosition = worldOriginOffset + intersectedLeafs[i].GetPositionInt();
                if (activeChunks.TryGetValue(chunkPosition, out Chunk chunk) && !(chunk.chunkMesh.needsMeshUpdate || chunk.chunkMesh.isAwaitingMeshUpdate))
                {
                    updatingChunks.Add(chunk);
                    chunk.chunkMesh.onMeshUpdated += OnChunkMeshUpdated;
                    chunk.chunkMesh.QueueMeshUpdate();
                }
            }
        }

        //Free any chunks that aren't referenced by some leaf node (lower level of subdivision etc.)
        foreach (Vector3Int key in chunksToPotentiallyFree)
        {
            if (activeChunks.TryGetValue(key, out Chunk chunk))
            {
                FreeOldChunk(chunk);
            }
        }
        chunksToPotentiallyFree.Clear();


        for (int i = 0; i < updatingChunks.Count; i++)
        {
            if (i % seamMeshQueuesPerFrame == 0)
            {
                yield return null;
            }

            Chunk updatingChunk = updatingChunks[i];
            ulong octreeNodeID = updatingChunk.octreeNodeID;
            if (!(updatingChunk.seamMesh.needsMeshUpdate || updatingChunk.seamMesh.isAwaitingMeshUpdate))
            {
                updatingChunk.seamMesh.onMeshUpdated += OnSeamMeshUpdated;
                updatingChunk.seamMesh.QueueMeshUpdate(chunkOctree.GetSeamChunks(octreeNodeID));
                updatingSeams.Add(updatingChunk);
            }


            List<ChunkOctreeNode> seamNeighbours = new List<ChunkOctreeNode>();
            chunkOctree.GetNeighbouringNodesToUpdate(octreeNodeID, seamNeighbours);
            for (int j = 0; j < seamNeighbours.Count; j++)
            {

                Vector3Int chunkPosition = worldOriginOffset + seamNeighbours[j].GetPositionInt();
                if (!activeChunks.TryGetValue(chunkPosition, out Chunk neighbouringChunk))
                {
                    continue;
                }


                if (!(neighbouringChunk.seamMesh.needsMeshUpdate || neighbouringChunk.seamMesh.isAwaitingMeshUpdate))
                {
                    neighbouringChunk.seamMesh.onMeshUpdated += OnSeamMeshUpdated;
                    neighbouringChunk.seamMesh.QueueMeshUpdate(chunkOctree.GetSeamChunks(seamNeighbours[j].id));
                    updatingSeams.Add(neighbouringChunk);
                }
            }
        }

        isUpdateCoroutineRunning = false;
        yield return null;
    }

    public void OnChunkMeshUpdated(Chunk updatedChunk)
    {
        updatedChunk.chunkMesh.onMeshUpdated -= OnChunkMeshUpdated;
        ++updatedChunksCount;
    }
    public void OnSeamMeshUpdated(Chunk updatedChunk)
    {
        updatedChunk.seamMesh.onMeshUpdated -= OnSeamMeshUpdated;
        ++updatedSeamsCount;
    }

    public Chunk AddChunk(ChunkOctreeNode correspondingOctreeNode)
    {
        Chunk chunk = GetFreeChunk();
        chunk.Show();
        chunk.ShowMesh(false);
        Vector3Int position = correspondingOctreeNode.GetPositionInt();
        chunk.SetPosition(position);
        chunk.SetSize(correspondingOctreeNode.GetSize() >> Chunk.EXPONENT_CHUNK_SIZE);
        chunk.SetOctreeNodeID(correspondingOctreeNode.id);

        Vector3Int samplePosition = worldOriginOffset + position;
        activeChunks.Add(samplePosition, chunk);

        chunkNodes[correspondingOctreeNode.GetOctreeIDX(), correspondingOctreeNode.GetOctreeIDZ()].AddChunk(samplePosition, chunk);
        return chunk;
    }

    public void FindAndSetSpawnPoint()
    {
        Vector3 startSearchLocation = new Vector3(seedOffset.x, 50.0f, seedOffset.z);
        spawnPointFinder.Find(startSearchLocation, out Vector3 foundPosition, out Vector3 foundDirection);

        playerObject.transform.position = foundPosition;
        playerObject.transform.rotation = Quaternion.LookRotation(foundDirection, Vector3.up);
    }

    public float GetTerrainWidthFactor()
    {
        return GameManager.Instance.GetSelectedDifficultyData().terrainWidthFactor;
    }
}
