using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    public List<GameObject> itemObjectsToSpawn;
    public List<int> itemPoolCounts;
    public List<float> itemProbabilities;


    int[] itemTypes;
    List<Queue<Item>> itemPools;
    HashSet<Vector3Int> pickedUpItems; 
    List<Vector3Int> neededItems;
    Vector3Int originOffsetAtUpdate;

    public ComputeShader computeShader;
    ComputeBuffer neededItemsBuffer;
    ComputeBuffer foundPositionsBuffer;

    GameObject itemPoolObject;

    [System.NonSerialized] public Dictionary<Vector3Int, Item> activeItems;

    bool needsUpdate;
    bool isUpdating;

    public event System.Action onItemsUpdated;

    void Awake()
    {
        Instance = this;
        activeItems = new Dictionary<Vector3Int, Item>();
        pickedUpItems = new HashSet<Vector3Int>();
        neededItems = new List<Vector3Int>();
        InitItemPools();
        NormalizeItemProbabilities();
    }


    void Start()
    {
        ProceduralWorld.Instance.onWorldOriginChanged += OnWorldOriginChanged;
    }

    void OnDestroy()
    {
        if (ProceduralWorld.Instance != null)
        {
            ProceduralWorld.Instance.onWorldOriginChanged -= OnWorldOriginChanged;
        }
    }
    void Update()
    {
        if (needsUpdate && !isUpdating)
        {
            UpdateItems();
        }
    }

    void InitItemPools()
    {
        if (itemObjectsToSpawn == null || itemPoolCounts == null || itemObjectsToSpawn.Count != itemPoolCounts.Count)
        {
            return;
        }

        itemPoolObject = new GameObject("Item Pool Object");
        itemPools = new List<Queue<Item>>();
        itemTypes = new int[itemObjectsToSpawn.Count];

        for (int i = 0; i < itemObjectsToSpawn.Count; i++)
        {
            itemPools.Add(new Queue<Item>(itemPoolCounts[i]));
            for (int j = 0; j < itemPoolCounts[i]; j++)
            {
                CreateNewItem(i);
            }

            itemTypes[(int)itemPools[i].Peek().itemType] = i;
        }
    }

    void CreateNewItem(int index)
    {
        GameObject itemObject = Instantiate(itemObjectsToSpawn[index], itemPoolObject.transform);
        Item item = itemObject.GetComponent<Item>();
        item.HideItem();
        itemPools[index].Enqueue(item);
    }

    void NormalizeItemProbabilities()
    {
        float totalProbability = 0.0f;
        for (int i = 0; i < itemProbabilities.Count; i++)
        {
            totalProbability += itemProbabilities[i];
        }

        for (int j = 0; j < itemProbabilities.Count; j++)
        {
            itemProbabilities[j] /= totalProbability;
        }
    }


    void FreeItem(Item item, bool removeFromActive = true)
    {
        if (removeFromActive)
        {
            activeItems.Remove(item.itemID);
        }

        int poolIndex = itemTypes[(int)item.itemType];
        item.HideItem();
        itemPools[poolIndex].Enqueue(item);
    }

    Item GetItemForPosition(Vector3Int position)
    {
        Vector3 samplePosition = position;
        samplePosition *= 120.1397f;
        samplePosition += new Vector3(0.123f, 0.111f, 0.173f);
        float randomValue = Mathf.Clamp01(Mathf.PerlinNoise(samplePosition.x, samplePosition.z));//(TerrainFunctionCPU.PerlinNoise(samplePosition) + 1.0f)*0.5f;
        float cumulativeProbability = 0.0f;
        for (int i = 0; i < itemProbabilities.Count; i++)
        {
            cumulativeProbability += itemProbabilities[i];
            if (cumulativeProbability > randomValue)
            {
                return GetNewItem(i);
            }
        }
        return GetNewItem(0);
    }

    Item GetNewItem(int index)
    {
        if (itemPools[index].Count == 0)
        {
            CreateNewItem(index);
        }


        return itemPools[index].Dequeue();
    }

    void OnWorldOriginChanged(Vector3Int offset, Vector3Int chunkOffset)
    {
        UpdateItems();
    }

    void UpdateItems()
    {
        if (isUpdating)
        {
            needsUpdate = true;
            return;
        }
        isUpdating = true;
        int itemGap = 256;
        int itemsPerChunk = ProceduralWorld.CHUNK_NODE_SIZE / itemGap - 1;
        Vector3Int itemOffset = new Vector3Int(itemGap, 0, itemGap);
        originOffsetAtUpdate = ProceduralWorld.Instance.worldOriginOffset;

        int chunkNodeSize = ProceduralWorld.CHUNK_NODE_SIZE;

        int nodeDistance = 3;


        Vector3Int startPosition = originOffsetAtUpdate + itemOffset - new Vector3Int(nodeDistance, 0, nodeDistance) * chunkNodeSize;

        //Get a list of items that are needed for the current origin offset
        List<Vector3Int> itemsNeededInWorld = new List<Vector3Int>();
        for (int x = 0; x < 2 * nodeDistance; x++)
        {
            for (int z = 0; z < 2 * nodeDistance; z++)
            {
                Vector3Int currentPosition = startPosition + new Vector3Int(x * chunkNodeSize, 0, z * chunkNodeSize); //50 for y

                for(int i = 0; i < itemsPerChunk; i++)
                {
                    Vector3Int basePosition = currentPosition + i * itemOffset;
                    basePosition.y += 30 + (int)(Mathf.PerlinNoise(basePosition.x, basePosition.z) * 40);
                    itemsNeededInWorld.Add(basePosition);
                }
            }
        }

        //Copy all still needed active items
        Dictionary<Vector3Int, Item> newActiveitems = new Dictionary<Vector3Int, Item>(activeItems.Count);
        neededItems.Clear();
        for (int i = 0; i < itemsNeededInWorld.Count; i++)
        {
            Vector3Int itemID = itemsNeededInWorld[i];
            if (activeItems.TryGetValue(itemID, out Item item))
            {
                newActiveitems.Add(itemID, item);
                activeItems.Remove(itemID);
                continue;
            }

            //Sort out needed but picked up items
            if (!pickedUpItems.Contains(itemID))
            {
                neededItems.Add(itemID);
            }
        }

        //Free unneeded active items
        Item[] itemsToFree = new Item[activeItems.Values.Count];
        activeItems.Values.CopyTo(itemsToFree, 0);
        for (int i = 0; i < itemsToFree.Length; i++)
        {
            FreeItem(itemsToFree[i]);
        }

        //replace old dictionary with the new one
        activeItems = newActiveitems;

        
        QueueNeededItems();
    }

    void QueueNeededItems()
    {
        neededItemsBuffer = new ComputeBuffer(neededItems.Count, 12);
        foundPositionsBuffer = new ComputeBuffer(neededItems.Count, 12);

        neededItemsBuffer.SetData(neededItems.ToArray());
        computeShader.SetBuffer(0, "neededItemsBuffer", neededItemsBuffer);
        computeShader.SetBuffer(0, "foundPositionsBuffer", foundPositionsBuffer);
        computeShader.SetInt("neededItemCount", neededItems.Count);
        computeShader.SetFloat("terrainWidthFactor", ProceduralWorld.Instance.GetTerrainWidthFactor());

        computeShader.Dispatch(0, (int)Mathf.Ceil(neededItems.Count / 32.0f), 1, 1);


        AsyncGPUReadback.Request(foundPositionsBuffer, OnPositionsFound);
    }

    void OnPositionsFound(AsyncGPUReadbackRequest request)
    {
        if(Instance == null || foundPositionsBuffer == null)
        {
            return;
        }

        Vector3[] foundPositions = request.GetData<Vector3>().ToArray();
        foundPositionsBuffer.Dispose();
        neededItemsBuffer.Dispose();

        for (int i = 0; i < foundPositions.Length; i++)
        {
            Vector3Int itemID = neededItems[i];
            Vector3 itemPosition = foundPositions[i] - originOffsetAtUpdate;

            if (activeItems.ContainsKey(itemID) || (itemID - Vector3Int.FloorToInt(foundPositions[i])).sqrMagnitude >= 262144)
            {
                continue;
            }

            Item item = GetItemForPosition(itemID);

            item.SetPosition(itemPosition);
            item.SetItemID(itemID);
            activeItems.Add(itemID, item);
            item.ShowItem();
            item.onPickupEnd += OnItemPickedUp;
        }

        if (onItemsUpdated != null)
        {
            onItemsUpdated.Invoke();
        }

        isUpdating = false;
    }

    void OnItemPickedUp(Item item)
    {
        pickedUpItems.Add(item.itemID);
        activeItems.Remove(item.itemID);
        FreeItem(item);

        if (onItemsUpdated != null)
        {
            onItemsUpdated.Invoke();
        }
    }
}
