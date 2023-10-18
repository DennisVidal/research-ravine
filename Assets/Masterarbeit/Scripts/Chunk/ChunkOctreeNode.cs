using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SeamRelevantData
{
    public int minChunkSize;
    public int[] chunkSizes;

    public SeamRelevantData(int minChunkSize, int[] chunkSizes)
    {
        this.minChunkSize = minChunkSize;
        this.chunkSizes = chunkSizes;
    }
}


public class ChunkOctreeNode
{
    public static int MIN_NODE_SIZE = Chunk.CHUNK_SIZE;
    public static int BIT_SHIFT_MIN_NODE_SIZE = (int)Mathf.Log(MIN_NODE_SIZE, 2);


    public Bounds bounds;
    public ulong id;
    public ChunkOctreeBorderCode borderCode;
    public bool isLeaf;
    public bool isLeafUpwards;

    public ChunkOctreeNode[] children;
    public ChunkOctreeNode[,,] neighbours;

    public ChunkOctreeNode(Vector3Int position, int size, ulong id, ChunkOctreeBorderCode borderCode = ChunkOctreeBorderCode.ALL)
    {
        int halfSize = size >> 1;
        bounds = new Bounds(position + new Vector3Int(halfSize, halfSize, halfSize), new Vector3(size, size, size));

        this.id = id;
        this.borderCode = borderCode;
        isLeafUpwards = false;
        isLeaf = false;
        children = null;
        neighbours = null;

        ChunkOctree.Instance.activeNodes[id] = this;
    }

    public void InitSubdivision()
    {
        if (GetSize() <= MIN_NODE_SIZE)
        {
            isLeaf = true;
            return;
        }

        CreateChildren();
        for (int i = 0; i < 8; i++)
        {
            children[i].InitSubdivision();
        }
    }
    void CreateChildren()
    {
        Vector3Int position = GetPositionInt();

        int childSize = GetSize() >> 1;
        children = new ChunkOctreeNode[8] {
                        new ChunkOctreeNode(position,                                                   childSize, GetChildID(0, 0, 0), ChunkOctreeBorderCode.BOTTOM_BACK_LEFT),
                        new ChunkOctreeNode(position + new Vector3Int(0, 0, childSize),                 childSize, GetChildID(0, 0, 1), ChunkOctreeBorderCode.BOTTOM_FRONT_LEFT),
                        new ChunkOctreeNode(position + new Vector3Int(childSize, 0, 0),                 childSize, GetChildID(1, 0, 0), ChunkOctreeBorderCode.BOTTOM_BACK_RIGHT),
                        new ChunkOctreeNode(position + new Vector3Int(childSize, 0, childSize),         childSize, GetChildID(1, 0, 1), ChunkOctreeBorderCode.BOTTOM_FRONT_RIGHT),
                        new ChunkOctreeNode(position + new Vector3Int(0, childSize, 0),                 childSize, GetChildID(0, 1, 0), ChunkOctreeBorderCode.TOP_BACK_LEFT),
                        new ChunkOctreeNode(position + new Vector3Int(0, childSize, childSize),         childSize, GetChildID(0, 1, 1), ChunkOctreeBorderCode.TOP_FRONT_LEFT),
                        new ChunkOctreeNode(position + new Vector3Int(childSize, childSize, 0),         childSize, GetChildID(1, 1, 0), ChunkOctreeBorderCode.TOP_BACK_RIGHT),
                        new ChunkOctreeNode(position + new Vector3Int(childSize, childSize, childSize), childSize, GetChildID(1, 1, 1), ChunkOctreeBorderCode.TOP_FRONT_RIGHT)
        };
    }


    public void InitNeighbourConnections(ChunkOctreeNode parent)
    {
        neighbours = new ChunkOctreeNode[3, 3, 3];
        for(int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if(x == 1 && y == 1 && z == 1)
                    {
                        neighbours[1, 1, 1] = parent;
                        continue;
                    }

                    ChunkOctree.Instance.FindNeighbouringNode(out ChunkOctreeNode neighbour, id, x - 1, y - 1, z - 1);
                    neighbours[x, y, z] = neighbour;
                }
            }
        }

        if(children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                children[i].InitNeighbourConnections(this);
            }
        }
    }






    public bool IsWithinBounds(Vector3 point)
    {
        return bounds.Contains(point);
    }

    public Vector3 GetMin()
    {
        return bounds.min;
    }
    public Vector3 GetMax()
    {
        return bounds.max;
    }
    public Vector3 GetCenter()
    {
        return bounds.center;
    }
    public Vector3 GetPosition()
    {
        return bounds.min;
    }
    public Vector3Int GetPositionInt()
    {
        return Vector3Int.RoundToInt(bounds.min);
    }
    public int GetSize()
    {
        return (int)bounds.size.x;
    }

    public ulong GetChildID(int x, int y, int z)
    {
        //Make room for new layer of coords and add coords
        ulong childID = (id << 1)
                       | ((ulong)x << ChunkOctree.ID_SHIFT_X)  //ID_SHIFT_X = 46
                       | ((ulong)y << ChunkOctree.ID_SHIFT_Y)  //ID_SHIFT_Y = 26
                       | ((ulong)z << ChunkOctree.ID_SHIFT_Z); //ID_SHIFT_Z = 7

        return (childID & ChunkOctree.ID_MASK_ALL_COORDINATES) //ID_MASK_ALL_COORDINATES = 0xFFFFFFFFFFFFFF80
             | ((id + 1) & (ulong)ChunkOctree.ID_MASK_DEPTH); //ID_MASK_DEPTH = 0x7F
    }

    public ChunkOctreeNode GetNeighbour(int x, int y, int z)
    {
        return neighbours[x + 1, y + 1, z + 1];
    }

    public ChunkOctreeNode GetParent()
    {
        return neighbours[1, 1, 1];
    }

    public bool ShouldSubdivide(ChunkOctreeSubdivisionData subdivisionData)
    {
        int size = GetSize();
        //If max subdivision reached => don't subdivide
        if (size <= MIN_NODE_SIZE)
        {
            return false;
        }

        //If node isn't visible => don't subdivide
        //if (!subdivisionData.IsInFrustum(bounds))
        //{
        //    return false;
        //}

        //If outside terrain min or max height => don't subdivide
        if (GetMax().y < 0.0f || 110.0f < GetMin().y)
        {
            return false;
        }

        Vector3 directionTargetToCenter = GetCenter() - subdivisionData.targetCamera.transform.position;
        return directionTargetToCenter.magnitude < size * (subdivisionData.IsInFrustum(bounds) ? 4.0f : 1.0f);
        //Vector3 directionTargetToCenter = GetCenter() - subdivisionData.targetCamera.transform.position;
        //float cosWeight = Vector3.Dot(directionTargetToCenter.normalized, subdivisionData.targetCamera.transform.forward);
        //return directionTargetToCenter.magnitude < size * (cosWeight + 2.0f);
    }




    public void UpdateLeafNodes(ChunkOctreeSubdivisionData subdivisionData, List<ChunkOctreeNode> newleafNodes, List<ChunkOctreeNode> oldLeafNodes)
    {
        isLeafUpwards = false;
        if (ShouldSubdivide(subdivisionData))
        {
            if (isLeaf)
            {
                oldLeafNodes.Add(this);
                isLeaf = false;
            }

            if (children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    children[i].UpdateLeafNodes(subdivisionData, newleafNodes, oldLeafNodes);
                }
            }
            return;
        }

        newleafNodes.Add(this);
        if (isLeaf)
        {
            return;
        }

        isLeaf = true;
        if (children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                children[i].ClearLeafRecursively(oldLeafNodes);
            }
        }
    }



    void ClearLeafRecursively(List<ChunkOctreeNode> oldLeafNodes)
    {
        isLeafUpwards = true;
        if (isLeaf)
        {
            oldLeafNodes.Add(this);
            isLeaf = false;
            return;
        }

        if(children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                children[i].ClearLeafRecursively(oldLeafNodes);
            }
        }
    }


    public void FindLeafs(List<ChunkOctreeNode> leafs, ChunkOctreeBorderCode border = ChunkOctreeBorderCode.ANY)
    {
        if (isLeafUpwards)
        {
            if(neighbours[1, 1, 1] != null)
            {
                neighbours[1, 1, 1].FindLeafs(leafs, border);
            }
            return;
        }

        if (isLeaf)
        {
            leafs.Add(this);
            return;
        }

        if(children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                if ((children[i].borderCode & border) == border)
                {
                    children[i].FindLeafs(leafs, border);
                }
            }
        }
    }

    public List<ChunkOctreeNode> GetLeafs(ChunkOctreeBorderCode border = ChunkOctreeBorderCode.ANY)
    {
        List<ChunkOctreeNode> leafs = new List<ChunkOctreeNode>();
        FindLeafs(leafs, border);
        return leafs;
    }


    public int GetIDX()
    {
        return (int)(id >> ChunkOctree.ID_SHIFT_X) & ChunkOctree.ID_MASK_COORDINATE;
    }
    public int GetIDY()
    {
        return (int)(id >> ChunkOctree.ID_SHIFT_Y) & ChunkOctree.ID_MASK_COORDINATE;
    }
    public int GetIDZ()
    {
        return (int)(id >> ChunkOctree.ID_SHIFT_Z) & ChunkOctree.ID_MASK_COORDINATE;
    }
    public int GetDepth()
    {
        return (int)id & ChunkOctree.ID_MASK_DEPTH;
    }
    public int GetOctreeIDX()
    {
        return GetIDX() >> GetDepth();
    }
    public int GetOctreeIDY()
    {
        return GetIDY() >> GetDepth();
    }
    public int GetOctreeIDZ()
    {
        return GetIDZ() >> GetDepth();
    }


    public void FullySubdivide()
    {
        isLeafUpwards = false;
        if (children != null)
        {
            isLeaf = false;
            for (int i = 0; i < 8; i++)
            {
                children[i].FullySubdivide();
            }
            return;
        }
        isLeaf = true;
    }

    public void FindDrillRayIntersectedLeafs(List<ChunkOctreeNode> intersectedLeafs)
    {
        if (!Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, DrillRay.PHYSICS_LAYER))
        {
            return;
        }

        if (isLeaf)
        {
            intersectedLeafs.Add(this);
            return;
        }

        if (children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                children[i].FindDrillRayIntersectedLeafs(intersectedLeafs);
            }
        }
    }



    public void DrawDebugCube()
    {
        if (isLeaf)
        {
            int size = GetSize();
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(GetCenter(), new Vector3Int(size, size, size));
            return;
        }

        if(children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                children[i].DrawDebugCube();
            }
        }
    }
}
