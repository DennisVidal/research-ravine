using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public ComputeShader computeShader;
    [System.NonSerialized] public RenderTexture texture;
    public Transform playerTransform;
    public int minimapResolution = 256;
    public float playerDotRadius = 16.0f;
    public float zoomLevel;
    public float itemDotRadius = 16.0f;

    ComputeBuffer itemPositionBuffer;

    public Material minimapMaterial;

    void Start()
    {
        texture = new RenderTexture(minimapResolution, minimapResolution, 16);
        texture.enableRandomWrite = true;
        computeShader.SetTexture(0, "minimapTexture", texture);
        computeShader.SetInt("resolution", minimapResolution);
        computeShader.SetInt("itemCount", 0);

        ColorModeData colorModeData = GameManager.Instance.GetSelectedColorModeData();
        computeShader.SetVector("mapColor", colorModeData.mapColor);
        computeShader.SetVector("mapBaseColor", colorModeData.mapBaseColor);
        computeShader.SetVector("playerColor", colorModeData.playerColor);
        computeShader.SetVector("gridColor", colorModeData.gridColor);
        computeShader.SetVector("itemColor", colorModeData.itemColor);

        DifficultyData difficultyData = GameManager.Instance.GetSelectedDifficultyData();
        computeShader.SetFloat("terrainWidthFactor", difficultyData.terrainWidthFactor);



        playerTransform = Camera.main.transform;
        itemPositionBuffer = new ComputeBuffer(1, 12);
        itemPositionBuffer.SetData(new Vector3[1]);


        if (minimapMaterial != null)
        {
            minimapMaterial.SetTexture("_BaseMap", texture);
        }

        ItemSpawner.Instance.onItemsUpdated += OnItemsUpdated;
    }


    void OnDestroy()
    {
        ItemSpawner.Instance.onItemsUpdated -= OnItemsUpdated;

        if (itemPositionBuffer != null)
        {
            itemPositionBuffer.Dispose();
        }
    }

    void OnItemsUpdated()
    {
        Dictionary<Vector3Int, Item> activeItems = ItemSpawner.Instance.activeItems;

        int dataSize = Mathf.Max(activeItems.Count, 1);
        List<Vector3> itemPositions = new List<Vector3>(dataSize);
        foreach(Item item in activeItems.Values)
        {
            itemPositions.Add(item.GetWorldPosition());
        }
        if(activeItems.Count == 0)
        {
            itemPositions.Add(new Vector3(float.MinValue, float.MinValue, float.MinValue));
        }
        if (itemPositionBuffer != null)
        {
            itemPositionBuffer.Dispose();
        }
        itemPositionBuffer = new ComputeBuffer(dataSize, 12);
        itemPositionBuffer.SetData(itemPositions.ToArray());
        computeShader.SetInt("itemCount", itemPositions.Count);
    }

    void Update()
    {
        computeShader.SetVector("playerPosition", ProceduralWorld.Instance.worldOriginOffset + playerTransform.position);
        computeShader.SetFloat("playerDotRadius", playerDotRadius);
        computeShader.SetFloat("zoomLevel", zoomLevel);
        computeShader.SetFloat("itemDotRadius", itemDotRadius);
        if(itemPositionBuffer != null)
        {
            computeShader.SetBuffer(0, "itemPositionBuffer", itemPositionBuffer);
        }
        computeShader.Dispatch(0, minimapResolution, minimapResolution, 1);
    }
}
