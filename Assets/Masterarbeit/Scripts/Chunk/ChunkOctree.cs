using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChunkOctreeBorderCode
{
    ANY = 0,

    LEFT = 1,
    FRONT = 2,
    RIGHT = 4,
    BACK = 8,
    BOTTOM = 16,
    TOP = 32,

    FRONT_LEFT = FRONT + LEFT,
    FRONT_RIGHT = FRONT + RIGHT,
    BACK_LEFT = BACK + LEFT,
    BACK_RIGHT = BACK + RIGHT,
    BOTTOM_LEFT = BOTTOM + LEFT,
    BOTTOM_RIGHT = BOTTOM + RIGHT,
    BOTTOM_BACK = BOTTOM + BACK,
    BOTTOM_BACK_LEFT = BOTTOM + BACK_LEFT,
    BOTTOM_BACK_RIGHT = BOTTOM + BACK_RIGHT,
    BOTTOM_FRONT = BOTTOM + FRONT,
    BOTTOM_FRONT_LEFT = BOTTOM + FRONT_LEFT,
    BOTTOM_FRONT_RIGHT = BOTTOM + FRONT_RIGHT,
    TOP_LEFT = TOP + LEFT,
    TOP_RIGHT = TOP + RIGHT,
    TOP_BACK = TOP + BACK,
    TOP_BACK_LEFT = TOP + BACK_LEFT,
    TOP_BACK_RIGHT = TOP + BACK_RIGHT,
    TOP_FRONT = TOP + FRONT,
    TOP_FRONT_LEFT = TOP + FRONT_LEFT,
    TOP_FRONT_RIGHT = TOP + FRONT_RIGHT,

    ALL = LEFT + FRONT + RIGHT + BACK + BOTTOM + TOP
}


public class ChunkOctree
{
    public static int ID_MASK_COORDINATE = 0x7FFFF;
    public static ulong ID_MASK_ALL_COORDINATES = 0xFFFFFFFFFFFFFF80;
    public static int ID_MASK_DEPTH = 0x7F;
    public static int ID_SHIFT_X = 45;
    public static int ID_SHIFT_Y = 26;
    public static int ID_SHIFT_Z = 7;
    public static int ID_MAX_DEPTH = 17;

    public static ChunkOctree Instance;

    public int viewDistance;
    public float inverseViewDistance;
    public int halfViewDistance;
    public float inverseHalfViewDistance;

    public ChunkOctreeNode[,] nodes;
    public Dictionary<ulong, ChunkOctreeNode> activeNodes;

    [System.NonSerialized]
    public int[,] borderMasks;

    Camera targetCamera;

    [System.NonSerialized]
    public ChunkOctreeSubdivisionData subdivisionData;

    public ChunkOctree(Camera targetCamera, int viewDistance)
    {
        Instance = this;
        SetViewDistance(viewDistance); 
        borderMasks = new int[,] { { 0x1FFFF, 0x5FFFF }, { 0x1FFFF, 0x5FFFF }, { 0x1FFFF, 0x5FFFF } };

        this.targetCamera = targetCamera;
        subdivisionData = new ChunkOctreeSubdivisionData(targetCamera);

        InitNodes();
    }

    void SetViewDistance(int distance)
    {
        viewDistance = distance;
        halfViewDistance = distance >> 1;
        inverseViewDistance = 1.0f / distance;
        inverseHalfViewDistance = 1.0f / halfViewDistance;
    }

    void InitNodes()
    {
        activeNodes = new Dictionary<ulong, ChunkOctreeNode>();
        nodes = new ChunkOctreeNode[3, 3];
        int startOffset = -viewDistance - halfViewDistance;
        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                Vector3Int nodePosition = new Vector3Int(x * viewDistance + startOffset, 0, z * viewDistance + startOffset);
                ulong nodeID = ((ulong)x << ID_SHIFT_X) | ((ulong)z << ID_SHIFT_Z);
                nodes[x, z] = new ChunkOctreeNode(nodePosition, viewDistance, nodeID, ChunkOctreeBorderCode.ALL);
                nodes[x, z].InitSubdivision();
            }
        }

        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                nodes[x, z].InitNeighbourConnections(null);
            }
        }

    }

    public void UpdateLeafNodes(List<ChunkOctreeNode> leafNodes, List<ChunkOctreeNode> oldLeafNodes)
    {
        subdivisionData.UpdateData(targetCamera);
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                nodes[x, z].UpdateLeafNodes(subdivisionData, leafNodes, oldLeafNodes);
            }
        }
    }


    public Vector3Int GetCenterPosition()
    {
        return Vector3Int.RoundToInt(nodes[1, 1].GetCenter());
    }

    public List<ChunkOctreeNode> GetNeighbouringNodes(ulong nodeID, int x, int y, int z)
    {
        List<ChunkOctreeNode> neighbouringNodes = new List<ChunkOctreeNode>();
        if (!FindNeighbouringNode(out ChunkOctreeNode neighbouringNode, nodeID, x, y, z))
        {
            return neighbouringNodes;
        }

        if (neighbouringNode.isLeaf)
        {
            neighbouringNodes.Add(neighbouringNode);
            return neighbouringNodes;
        }

        return neighbouringNodes;
    }

    public void GetNeighbouringNodesToUpdate(ulong nodeID, List<ChunkOctreeNode> nodesToUpdate)
    {
        if(!activeNodes.TryGetValue(nodeID, out ChunkOctreeNode node))
        {
            return;
        }

        if (node.neighbours[1, 0, 0] != null) { node.neighbours[1, 0, 0].FindLeafs(nodesToUpdate, ChunkOctreeBorderCode.TOP_FRONT); }
        if (node.neighbours[0, 1, 0] != null) { node.neighbours[0, 1, 0].FindLeafs(nodesToUpdate, ChunkOctreeBorderCode.FRONT_RIGHT); }
        if (node.neighbours[1, 1, 0] != null) { node.neighbours[1, 1, 0].FindLeafs(nodesToUpdate, ChunkOctreeBorderCode.FRONT); }
        if (node.neighbours[0, 0, 1] != null) { node.neighbours[0, 0, 1].FindLeafs(nodesToUpdate, ChunkOctreeBorderCode.TOP_RIGHT); }
        if (node.neighbours[1, 0, 1] != null) { node.neighbours[1, 0, 1].FindLeafs(nodesToUpdate, ChunkOctreeBorderCode.TOP); }
        if (node.neighbours[0, 1, 1] != null) { node.neighbours[0, 1, 1].FindLeafs(nodesToUpdate, ChunkOctreeBorderCode.RIGHT); }
    }

    public List<ChunkOctreeNode>[] GetSeamChunks(ulong nodeID)
    {
        List<ChunkOctreeNode> emptyList = new List<ChunkOctreeNode>();
        if (!activeNodes.TryGetValue(nodeID, out ChunkOctreeNode node))
        {
            return new List<ChunkOctreeNode>[6] { emptyList, emptyList, emptyList, emptyList, emptyList, emptyList };
        }

        return new List<ChunkOctreeNode>[6] {
            node.neighbours[2, 1, 1] != null ? node.neighbours[2, 1, 1].GetLeafs(ChunkOctreeBorderCode.LEFT)        : emptyList,
            node.neighbours[1, 2, 1] != null ? node.neighbours[1, 2, 1].GetLeafs(ChunkOctreeBorderCode.BOTTOM)      : emptyList,
            node.neighbours[2, 2, 1] != null ? node.neighbours[2, 2, 1].GetLeafs(ChunkOctreeBorderCode.BOTTOM_LEFT) : emptyList,
            node.neighbours[1, 1, 2] != null ? node.neighbours[1, 1, 2].GetLeafs(ChunkOctreeBorderCode.BACK)        : emptyList,
            node.neighbours[2, 1, 2] != null ? node.neighbours[2, 1, 2].GetLeafs(ChunkOctreeBorderCode.BACK_LEFT)   : emptyList,
            node.neighbours[1, 2, 2] != null ? node.neighbours[1, 2, 2].GetLeafs(ChunkOctreeBorderCode.BOTTOM_BACK) : emptyList
        };                                                                     
    }


    public bool FindNeighbouringNode(out ChunkOctreeNode neighbouringNode, ulong nodeID, int x, int y, int z)
    {
        neighbouringNode = null;
        //Get the axis components and depth from id
        int nodeIDX = GetIDX(nodeID);
        int nodeIDY = GetIDY(nodeID);
        int nodeIDZ = GetIDZ(nodeID);
        int depth = GetDepth(nodeID);

        nodeIDX += x;
        nodeIDY += y;
        nodeIDZ += z;
        
        if (nodeIDX < 0 || nodeIDX >> depth == 3) { return false; }
        if (nodeIDY < 0 || nodeIDY >> depth == 3) { return false; }
        if (nodeIDZ < 0 || nodeIDZ >> depth == 3) { return false; }

        //nodeIDX += x;
        //if (nodeIDX < 0) { nodeIDX = (3 << depth) - 1; }
        //else if (nodeIDX >> depth == 3) { nodeIDX = 0; }
        //
        //nodeIDY += y;
        //if (nodeIDY < 0) { nodeIDY = (3 << depth) - 1; }
        //else if (nodeIDY >> depth == 3) { nodeIDY = 0; }
        //
        //nodeIDZ += z;
        //if (nodeIDZ < 0) { nodeIDZ = (3 << depth) - 1; }
        //else if (nodeIDZ >> depth == 3) { nodeIDZ = 0; }


        //Build neighbouring node id
        ulong neighbourID = ((ulong)nodeIDX << ID_SHIFT_X)
                          | ((ulong)nodeIDY << ID_SHIFT_Y)
                          | ((ulong)nodeIDZ << ID_SHIFT_Z)
                          | (ulong)depth;
        return activeNodes.TryGetValue(neighbourID, out neighbouringNode);
    }

    public static int GetIDX(ulong id)
    {
        return (int)(id >> ID_SHIFT_X) & ID_MASK_COORDINATE;
    }
    public static int GetIDY(ulong id)
    {
        return (int)(id >> ID_SHIFT_Y) & ID_MASK_COORDINATE;
    }
    public static int GetIDZ(ulong id)
    {
        return (int)(id >> ID_SHIFT_Z) & ID_MASK_COORDINATE;
    }

    public static int GetOctreeIDX(ulong id)
    {
        int idX = (int)(id >> ID_SHIFT_X) & ID_MASK_COORDINATE;
        return idX >> GetDepth(id);
    }
    public static int GetOctreeIDY(ulong id)
    {
        int idY = (int)(id >> ID_SHIFT_Y) & ID_MASK_COORDINATE;
        return idY >> GetDepth(id);
    }
    public static int GetOctreeIDZ(ulong id)
    {
        int idZ = (int)(id >> ID_SHIFT_Z) & ID_MASK_COORDINATE;
        return idZ >> GetDepth(id);
    }
    public static int GetDepth(ulong id)
    {
        return (int)id & ID_MASK_DEPTH;
    }

    public void FullySubdivideSubOctree(int x, int z)
    {
        nodes[x, z].FullySubdivide();
    }


    public void FindDrillRayIntersectedLeafs(List<ChunkOctreeNode> intersectedLeafs)
    {
        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                nodes[x, z].FindDrillRayIntersectedLeafs(intersectedLeafs);
            }
        }
    }


    public void DrawDebugCube()
    {
      if (nodes != null)
      {
          Gizmos.color = Color.white;
          for (int z = 0; z < 3; z++)
          {
                for (int x = 0; x < 3; x++)
                {
                    if (nodes[x, z] != null)
                    {
                        nodes[x, z].DrawDebugCube();
                    }
                }
           }
      }
    }
}
