using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Diagnostics;
using UnityEngine.Rendering;
using Unity.Collections;
/*
public class Chunk : MonoBehaviour
{
    public static int CHUNK_SIZE = 32;
    public static int SAMPLE_SIZE = CHUNK_SIZE + 1;
    public int size;
    MeshFilter meshFilter;
    public float[] samples;


    int kernel;
    public ComputeShader chunkComputeShader;
    ComputeBuffer vertexBuffer;
    ComputeBuffer triangleBuffer;
    ComputeBuffer edgeGroupBuffer;


    Vector3[] meshVertices;
    int[] meshTriangles;

    bool debugLogged = false;


    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        kernel = chunkComputeShader.FindKernel("March");
    }

    void OnDrawGizmosSelected()
    {
        if(meshVertices != null)
        {
            Gizmos.color = Color.red;
            for(int i = 0; i < meshVertices.Length; i++)
            {
                Gizmos.DrawSphere(gameObject.transform.position + meshVertices[i], 0.1f);
            }
            if(!debugLogged)
            {
                debugLogged = true;
                for (int i = 0; i < meshTriangles.Length; i+=3)
                {
                    UnityEngine.Debug.Log("Triangle[" + i / 3 + "] = " + meshTriangles[i] + ", " + meshTriangles[i + 1] + ", " + meshTriangles[i + 2]);
                }

            }
        }
    }

    public void Init(Vector3Int position, int size)
    {
        if (samples == null)
        {
            samples = new float[SAMPLE_SIZE * SAMPLE_SIZE * SAMPLE_SIZE];
        }

        gameObject.transform.position = position;
        this.size = size;
        UpdateChunk();
    }

    public Vector3Int GetPosition()
    {
        Vector3 pos = gameObject.transform.position;
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }

    


    public void UpdateChunk()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        vertexBuffer = new ComputeBuffer(10000, 12);
        triangleBuffer = new ComputeBuffer(20000, 4);
        edgeGroupBuffer = new ComputeBuffer(SAMPLE_SIZE * SAMPLE_SIZE * SAMPLE_SIZE, 64);

        ComputeBuffer countsBuffer = new ComputeBuffer(2, 4);
        chunkComputeShader.SetBuffer(kernel, "countsBuffer", countsBuffer);
        countsBuffer.SetData(new int[2] { 0, 0 });


        chunkComputeShader.SetBuffer(kernel, "vertexBuffer", vertexBuffer);
        chunkComputeShader.SetBuffer(kernel, "triangleBuffer", triangleBuffer);
        chunkComputeShader.SetBuffer(kernel, "edges", edgeGroupBuffer);
        chunkComputeShader.SetVector("chunkPosition", gameObject.transform.position);
        chunkComputeShader.SetInt("chunkSize", size);

        
        chunkComputeShader.Dispatch(kernel, 8, 8, 8);

        int[] counts = new int[2];
        countsBuffer.GetData(counts);
        int vertexCount = counts[0];
        int triangleCount = counts[1];
        UnityEngine.Debug.Log("vertices = " + vertexCount);
        UnityEngine.Debug.Log("triangles = " + triangleCount / 3 + " (" + triangleCount + ")");
        
        
        meshVertices = new Vector3[vertexCount];
        vertexBuffer.GetData(meshVertices, 0, 0, vertexCount);
        

        meshTriangles = new int[triangleCount];
        triangleBuffer.GetData(meshTriangles, 0, 0, triangleCount);

        Mesh mesh = new Mesh();
        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        meshFilter.mesh = mesh;

        sw.Stop();
        //UnityEngine.Debug.Log("Time = " + sw.ElapsedMilliseconds + "ms");


        //AsyncGPUReadback.Request(vertexBuffer, OnVertexBufferReady);
        //AsyncGPUReadback.Request(triangleBuffer, OnTriangleBufferReady);

        vertexBuffer.Dispose();
        triangleBuffer.Dispose();
        edgeGroupBuffer.Dispose();
        countsBuffer.Dispose();
    }

    public void OnVertexBufferReady(AsyncGPUReadbackRequest request)
    {
        NativeArray<Vector3> vertices = request.GetData<Vector3>();
        meshFilter.sharedMesh.vertices = vertices.ToArray();
    }

    public void OnTriangleBufferReady(AsyncGPUReadbackRequest request)
    {
        NativeArray<int> triangles = request.GetData<int>();
        meshFilter.sharedMesh.triangles = triangles.ToArray();
    }
}



public class Chunk : MonoBehaviour
{
    public static int CHUNK_SIZE = 32;
    public static int SAMPLE_SIZE = CHUNK_SIZE + 1;
    public int size;
    public Vector3Int position;

    public float[,,] samples;

    MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        samples = null;
    }

    public void Show()
    {
       // gameObject.SetActive(true);
    }

    public void Hide()
    {
      //  gameObject.SetActive(false);
    }


    public void Init(Vector3Int position, int size)
    {
        if (samples == null)
        {
            samples = new float[SAMPLE_SIZE, SAMPLE_SIZE, SAMPLE_SIZE];
        }

        this.position = position;
        this.size = size;
        UpdateSamples();
    }


    public void UpdateSamples(Action callback = null)
    {
        StartCoroutine(UpdateSamplesThreaded(callback));
    }
    IEnumerator UpdateSamplesThreaded(Action callback = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
       // ComputeShader computeShader;
       // int kernel = computeShader.FindKernel("PerlinNoise3D");
       // computeShader.SetVector("WorldPosition", new Vector3(position.x, position.y, position.z));
       // computeShader.SetInt("StepSize", size);
       // computeShader.SetInt("ChunkSize", CHUNK_SIZE);
       // ComputeBuffer valueBuffer = new ComputeBuffer(CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE, 4);
       // computeShader.SetBuffer(kernel, "ValueBuffer", valueBuffer);
       // computeShader.Dispatch(kernel, 9, 9, 9);
       //
       // float[] values = new float[CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE];
       // valueBuffer.GetData(values);
       //
       //
       // sw.Stop();
       // UnityEngine.Debug.Log("Time for sampling gpu = " + sw.ElapsedMilliseconds);
       // sw.Restart();

        bool samplesUpdated = false;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        new Thread(() => {
            for (int z = 0; z <= CHUNK_SIZE; z++)
            {
                for (int y = 0; y <= CHUNK_SIZE; y++)
                {
                    for (int x = 0; x <= CHUNK_SIZE; x++)
                    {
                        samples[x, y, z] = ProceduralTerrain.instance.GetValue(new Vector3Int(position.x + x * size, position.y + y * size, position.z + z * size));
                    }
                }
            }
            samplesUpdated = true;
        }).Start();

        while (!samplesUpdated)
        {
            yield return null;
        }

        sw.Stop();
        UnityEngine.Debug.Log("Time for sampling = " + sw.ElapsedMilliseconds);
        OnSamplesUpdated();
        if (callback != null)
        {
            callback();
        }
    }


    public void UpdateMesh(Action callback = null)
    {
        StartCoroutine(UpdateMeshThreaded(callback));
    }
    IEnumerator UpdateMeshThreaded(Action callback = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        bool meshUpdated = false;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        new Thread(() => { 
            DualMarchingCubes.March(position, CHUNK_SIZE, size, vertices, triangles, normals);
            meshUpdated = true;
        }).Start();

        while(!meshUpdated)
        {
            yield return null;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        meshFilter.mesh = mesh;

        sw.Stop();
        UnityEngine.Debug.Log("Time for mesh = " + sw.ElapsedMilliseconds);

        OnMeshUpdated();
        if (callback != null)
        {
            callback();
        }
    }

    public void OnSamplesUpdated()
    {
        UpdateMesh();
    }
    public void OnMeshUpdated()
    {
        //gameObject.SetActive(true);
    }
}
 */

/*
public class Chunk : MonoBehaviour
{
    public int size;
    public Vector3Int position;
    public bool isActive;
    public bool isInitialized;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    public Material material;


    MeshData updatedMeshData;

    private void Update()
    {
        if(isInitialized && !isActive)
        {
            Destroy(gameObject);
            return;
        }
        isActive = false;

        if(updatedMeshData.isUpdated)
        {
            meshFilter.mesh.vertices = updatedMeshData.vertices.ToArray();
            meshFilter.mesh.triangles = updatedMeshData.triangles.ToArray();
            //meshFilter.mesh.RecalculateNormals();
            updatedMeshData.isUpdated = false;
        }
    }
    public void Init(Vector3Int p, int s)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;

        position = p;
        size = s;
        CreateMesh(false);

        isInitialized = true;
        isActive = true;
    }

    public void UpdateSize(int s)
    {
        size = s;
        CreateMesh(false);
    }

    public void CreateMesh(bool threaded = true)
    {
        if(threaded)
        {
            new Thread(CreateMeshThreaded).Start();
            return;
        }
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normal = new List<Vector3>();
        Mesh mesh = new Mesh();
        DualMarchingCubes.March(position, 16, size, vertices, triangles, normal);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    public struct MeshData
    {
        public bool isUpdated;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector3> normal;
    }

    public void CreateMeshThreaded()
    {
        MeshData meshData = new MeshData();
        meshData.vertices = new List<Vector3>();
        meshData.triangles = new List<int>();
        meshData.normal = new List<Vector3>();
        DualMarchingCubes.March(position, 16, size, meshData.vertices, meshData.triangles, meshData.normal);
        updatedMeshData = meshData;
        updatedMeshData.isUpdated = true;
    }
}


public class Chunk1
{

    static int CHUNK_SIZE = 16;
    Vector3 position;
    PerlinNoiseCPU noise;

    ChunkOctree octree;
    TerrainDensityField terrainFunction;


    List<Vector3[]> cubePositions;
    List<Vector4[]> cubeValues;

    public Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    public Material material;

    void Start()
    {
        position = gameObject.transform.position;
        noise = new PerlinNoiseCPU();

        terrainFunction = new TerrainDensityField();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        octree = new ChunkOctree(new Bounds(position + new Vector3(CHUNK_SIZE * 0.5f, CHUNK_SIZE * 0.5f, CHUNK_SIZE * 0.5f), new Vector3(CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE)));

        float[] cornerValues = new float[8];
        cornerValues[0] = noise.GetNoise(position + new Vector3(0, 0, 0));
        cornerValues[1] = noise.GetNoise(position + new Vector3(0, 0, CHUNK_SIZE));
        cornerValues[2] = noise.GetNoise(position + new Vector3(CHUNK_SIZE, 0, CHUNK_SIZE));
        cornerValues[3] = noise.GetNoise(position + new Vector3(CHUNK_SIZE, 0, 0));
        cornerValues[4] = noise.GetNoise(position + new Vector3(0, CHUNK_SIZE, 0));
        cornerValues[5] = noise.GetNoise(position + new Vector3(0, CHUNK_SIZE, CHUNK_SIZE));
        cornerValues[6] = noise.GetNoise(position + new Vector3(CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE));
        cornerValues[7] = noise.GetNoise(position + new Vector3(CHUNK_SIZE, CHUNK_SIZE, 0));
        octree.Subdivide(terrainFunction, 0.1f, cornerValues);

        cubePositions = new List<Vector3[]>();
        cubeValues = new List<Vector4[]>();
        NodeProc(octree);
        sw.Stop();
        UnityEngine.Debug.Log("cubePositions Count =" + cubePositions.Count);
        UnityEngine.Debug.Log("cubeValues Count =" + cubeValues.Count);
        UnityEngine.Debug.Log("Chunk creation took " + sw.ElapsedMilliseconds + "ms");

        MarchingCubes marchingCubes = new MarchingCubes();
        marchingCubes.March(cubePositions, cubeValues);

        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();

        mesh.vertices = marchingCubes.vertices.ToArray();
        mesh.triangles = marchingCubes.triangles.ToArray();
        mesh.normals = marchingCubes.normals.ToArray();
        meshFilter.mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    #region Proc
    void NodeProc(ChunkOctree n)
    {
        if (n.IsLeaf())
        {
            return;
        }

        ChunkOctree c0 = n.children[0];
        ChunkOctree c1 = n.children[1];
        ChunkOctree c2 = n.children[2];
        ChunkOctree c3 = n.children[3];
        ChunkOctree c4 = n.children[4];
        ChunkOctree c5 = n.children[5];
        ChunkOctree c6 = n.children[6];
        ChunkOctree c7 = n.children[7];

        NodeProc(c0);
        NodeProc(c1);
        NodeProc(c2);
        NodeProc(c3);
        NodeProc(c4);
        NodeProc(c5);
        NodeProc(c6);
        NodeProc(c7);

        FaceProcXZ(c0, c4);
        FaceProcXZ(c1, c5);
        FaceProcXZ(c2, c6);
        FaceProcXZ(c3, c7);

        FaceProcXY(c0, c1);
        FaceProcXY(c3, c2);
        FaceProcXY(c4, c5);
        FaceProcXY(c7, c6);

        FaceProcYZ(c0, c3);
        FaceProcYZ(c1, c2);
        FaceProcYZ(c4, c7);
        FaceProcYZ(c5, c6);

        EdgeProcX(c0, c1, c4, c5);
        EdgeProcX(c3, c2, c7, c6);

        EdgeProcY(c0, c3, c1, c2);
        EdgeProcY(c4, c7, c5, c6);

        EdgeProcZ(c0, c3, c4, c7);
        EdgeProcZ(c1, c2, c5, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);

    }

    void FaceProcXY(ChunkOctree n0, ChunkOctree n1)
    {
        if (!(n0.IsSubdivided() || n1.IsSubdivided()))
        {
            return;
        }

        ChunkOctree c0 = n0;
        ChunkOctree c1 = n1;
        ChunkOctree c2 = n1;
        ChunkOctree c3 = n0;
        ChunkOctree c4 = n0;
        ChunkOctree c5 = n1;
        ChunkOctree c6 = n1;
        ChunkOctree c7 = n0;

        if (n0.IsSubdivided())
        {
            c0 = n0.children[1];
            c3 = n0.children[2];
            c4 = n0.children[5];
            c7 = n0.children[6];
        }

        if (n1.IsSubdivided())
        {
            c1 = n1.children[0];
            c2 = n1.children[3];
            c5 = n1.children[4];
            c6 = n1.children[7];
        }

        FaceProcXY(c0, c1);
        FaceProcXY(c3, c2);
        FaceProcXY(c4, c5);
        FaceProcXY(c7, c6);

        EdgeProcX(c0, c1, c4, c5);
        EdgeProcX(c3, c2, c7, c6);

        EdgeProcY(c0, c3, c1, c2);
        EdgeProcY(c4, c7, c5, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);

    }
    void FaceProcYZ(ChunkOctree n0, ChunkOctree n1)
    {
        if (!(n0.IsSubdivided() || n1.IsSubdivided()))
        {
            return;
        }

        ChunkOctree c0 = n0;
        ChunkOctree c1 = n0;
        ChunkOctree c2 = n1;
        ChunkOctree c3 = n1;
        ChunkOctree c4 = n0;
        ChunkOctree c5 = n0;
        ChunkOctree c6 = n1;
        ChunkOctree c7 = n1;

        if (n0.IsSubdivided())
        {
            c0 = n0.children[3];
            c1 = n0.children[2];
            c4 = n0.children[7];
            c5 = n0.children[6];
        }

        if (n1.IsSubdivided())
        {
            c2 = n1.children[1];
            c3 = n1.children[0];
            c6 = n1.children[5];
            c7 = n1.children[4];
        }

        FaceProcYZ(c0, c3);
        FaceProcYZ(c1, c2);
        FaceProcYZ(c4, c7);
        FaceProcYZ(c5, c6);

        EdgeProcY(c0, c3, c1, c2);
        EdgeProcY(c4, c7, c5, c6);

        EdgeProcZ(c0, c3, c4, c7);
        EdgeProcZ(c1, c2, c5, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);
    }
    void FaceProcXZ(ChunkOctree n0, ChunkOctree n1)
    {
        if (!(n0.IsSubdivided() || n1.IsSubdivided()))
        {
            return;
        }

        ChunkOctree c0 = n0;
        ChunkOctree c1 = n0;
        ChunkOctree c2 = n0;
        ChunkOctree c3 = n0;
        ChunkOctree c4 = n1;
        ChunkOctree c5 = n1;
        ChunkOctree c6 = n1;
        ChunkOctree c7 = n1;

        if (n0.IsSubdivided())
        {
            c0 = n0.children[4];
            c1 = n0.children[5];
            c2 = n0.children[6];
            c3 = n0.children[7];
        }

        if (n1.IsSubdivided())
        {
            c4 = n1.children[0];
            c5 = n1.children[1];
            c6 = n1.children[2];
            c7 = n1.children[3];
        }

        FaceProcXZ(c0, c4);
        FaceProcXZ(c1, c5);
        FaceProcXZ(c2, c6);
        FaceProcXZ(c3, c7);

        EdgeProcX(c0, c1, c4, c5);
        EdgeProcX(c3, c2, c7, c6);

        EdgeProcZ(c0, c3, c4, c7);
        EdgeProcZ(c1, c2, c5, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);
    }

    void EdgeProcY(ChunkOctree n0, ChunkOctree n1, ChunkOctree n2, ChunkOctree n3)
    {
        if (!(n0.IsSubdivided() || n1.IsSubdivided() || n2.IsSubdivided() || n3.IsSubdivided()))
        {
            return;
        }

        ChunkOctree c0 = n0;
        ChunkOctree c1 = n2;
        ChunkOctree c2 = n3;
        ChunkOctree c3 = n1;
        ChunkOctree c4 = n0;
        ChunkOctree c5 = n2;
        ChunkOctree c6 = n3;
        ChunkOctree c7 = n1;


        if (n0.IsSubdivided())
        {
            c0 = n0.children[2];
            c4 = n0.children[6];
        }

        if (n1.IsSubdivided())
        {
            c3 = n1.children[1];
            c7 = n1.children[5];
        }

        if (n2.IsSubdivided())
        {
            c1 = n2.children[3];
            c5 = n2.children[7];
        }

        if (n3.IsSubdivided())
        {
            c2 = n3.children[0];
            c6 = n3.children[4];
        }

        EdgeProcY(c0, c3, c1, c2);
        EdgeProcY(c4, c7, c5, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);
    }
    void EdgeProcZ(ChunkOctree n0, ChunkOctree n1, ChunkOctree n2, ChunkOctree n3)
    {
        if (!(n0.IsSubdivided() || n1.IsSubdivided() || n2.IsSubdivided() || n3.IsSubdivided()))
        {
            return;
        }

        ChunkOctree c0 = n0;
        ChunkOctree c1 = n0;
        ChunkOctree c2 = n1;
        ChunkOctree c3 = n1;
        ChunkOctree c4 = n2;
        ChunkOctree c5 = n2;
        ChunkOctree c6 = n3;
        ChunkOctree c7 = n3;


        if (n0.IsSubdivided())
        {
            c0 = n0.children[7];
            c1 = n0.children[6];
        }

        if (n1.IsSubdivided())
        {
            c2 = n1.children[5];
            c3 = n1.children[4];
        }

        if (n2.IsSubdivided())
        {
            c4 = n2.children[3];
            c5 = n2.children[2];
        }

        if (n3.IsSubdivided())
        {
            c6 = n3.children[1];
            c7 = n3.children[0];
        }

        EdgeProcZ(c0, c3, c4, c7);
        EdgeProcZ(c1, c2, c5, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);
    }
    void EdgeProcX(ChunkOctree n0, ChunkOctree n1, ChunkOctree n2, ChunkOctree n3)
    {
        if (!(n0.IsSubdivided() || n1.IsSubdivided() || n2.IsSubdivided() || n3.IsSubdivided()))
        {
            return;
        }

        ChunkOctree c0 = n0;
        ChunkOctree c1 = n1;
        ChunkOctree c2 = n1;
        ChunkOctree c3 = n0;
        ChunkOctree c4 = n2;
        ChunkOctree c5 = n3;
        ChunkOctree c6 = n3;
        ChunkOctree c7 = n2;


        if (n0.IsSubdivided())
        {
            c0 = n0.children[5];
            c3 = n0.children[6];
        }

        if (n1.IsSubdivided())
        {
            c1 = n1.children[4];
            c2 = n1.children[7];
        }

        if (n2.IsSubdivided())
        {
            c4 = n2.children[1];
            c7 = n2.children[2];
        }

        if (n3.IsSubdivided())
        {
            c5 = n3.children[0];
            c6 = n3.children[3];
        }

        EdgeProcX(c0, c1, c4, c5);
        EdgeProcX(c3, c2, c7, c6);

        VertProc(c0, c1, c2, c3, c4, c5, c6, c7);
    }

    void VertProc(ChunkOctree n0, ChunkOctree n1, ChunkOctree n2, ChunkOctree n3, ChunkOctree n4, ChunkOctree n5, ChunkOctree n6, ChunkOctree n7)
    {
        if (n0.IsSubdivided() || n1.IsSubdivided() || n2.IsSubdivided() || n3.IsSubdivided() || n4.IsSubdivided() || n5.IsSubdivided() || n6.IsSubdivided() || n7.IsSubdivided())
        {
            ChunkOctree c0 = n0.IsLeaf() ? n0 : n0.children[6];
            ChunkOctree c1 = n1.IsLeaf() ? n1 : n1.children[7];
            ChunkOctree c2 = n2.IsLeaf() ? n2 : n2.children[4];
            ChunkOctree c3 = n3.IsLeaf() ? n3 : n3.children[5];
            ChunkOctree c4 = n4.IsLeaf() ? n4 : n4.children[2];
            ChunkOctree c5 = n5.IsLeaf() ? n5 : n5.children[3];
            ChunkOctree c6 = n6.IsLeaf() ? n6 : n6.children[0];
            ChunkOctree c7 = n7.IsLeaf() ? n7 : n7.children[1];

            VertProc(c0, c1, c2, c3, c4, c5, c6, c7);
            return;
        }
        CreateDualCell(new Vector3[]
                                    {
                                            n0.GetPosition(),
                                            n1.GetPosition(),
                                            n2.GetPosition(),
                                            n3.GetPosition(),
                                            n4.GetPosition(),
                                            n5.GetPosition(),
                                            n6.GetPosition(),
                                            n7.GetPosition() },
                        new Vector4[]
                        {
                                n0.sample,
                                n1.sample,
                                n2.sample,
                                n3.sample,
                                n4.sample,
                                n5.sample,
                                n6.sample,
                                n7.sample
                        });

        CreateBorderCells(n0, n1, n2, n3, n4, n5, n6, n7);
    }
    #endregion Proc


    void CreateDualCell(Vector3[] positions, Vector4[] gradientsAndValues)
    {
        cubePositions.Add(positions);
        cubeValues.Add(gradientsAndValues);
    }

    void CreateBorderCells(ChunkOctree n0, ChunkOctree n1, ChunkOctree n2, ChunkOctree n3, ChunkOctree n4, ChunkOctree n5, ChunkOctree n6, ChunkOctree n7)
    {
        ChunkOctree.BorderCode c0 = n0.borderCode;
        ChunkOctree.BorderCode c1 = n1.borderCode;
        ChunkOctree.BorderCode c2 = n2.borderCode;
        ChunkOctree.BorderCode c3 = n3.borderCode;
        ChunkOctree.BorderCode c4 = n4.borderCode;
        ChunkOctree.BorderCode c5 = n5.borderCode;
        ChunkOctree.BorderCode c6 = n6.borderCode;
        ChunkOctree.BorderCode c7 = n7.borderCode;



        //BACK
        ChunkOctree.BorderCode c0c3 = c0 & c3;
        ChunkOctree.BorderCode c4c7 = c4 & c7;
        ChunkOctree.BorderCode c0c4 = c0 & c4;
        ChunkOctree.BorderCode c3c7 = c3 & c7;
        if ((c0c3 & c4c7 & ChunkOctree.BorderCode.BACK) != 0)
        {
            CreateDualCell(new Vector3[] { n0.GetPos110(), n0.GetPos111(), n3.GetPos111(), n3.GetPos110(), n4.GetPos110(), n4.GetPos111(), n7.GetPos111(), n7.GetPos110() },
                           new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos110()), terrainFunction.GetValueAndGradient(n0.GetPos111()), terrainFunction.GetValueAndGradient(n3.GetPos111()), terrainFunction.GetValueAndGradient(n3.GetPos110()), terrainFunction.GetValueAndGradient(n4.GetPos110()), terrainFunction.GetValueAndGradient(n4.GetPos111()), terrainFunction.GetValueAndGradient(n7.GetPos111()), terrainFunction.GetValueAndGradient(n7.GetPos110()) });

            if ((c0c3 & ChunkOctree.BorderCode.BOTTOM) != 0)
            {
                CreateDualCell(new Vector3[] { n0.GetPos100(),
                                               n0.GetPos101(),
                                               n3.GetPos101(),
                                               n3.GetPos100(),
                                               n0.GetPos110(),
                                               n0.GetPos111(),
                                               n3.GetPos111(),
                                               n3.GetPos110() },
                               new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos100()),
                                               terrainFunction.GetValueAndGradient(n0.GetPos101()),
                                               terrainFunction.GetValueAndGradient(n3.GetPos101()),
                                               terrainFunction.GetValueAndGradient(n3.GetPos100()),
                                               terrainFunction.GetValueAndGradient(n0.GetPos110()),
                                               terrainFunction.GetValueAndGradient(n0.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n3.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n3.GetPos110())});

                if ((c0 & ChunkOctree.BorderCode.LEFT) != 0)
                {
                    CreateDualCell(new Vector3[] { n0.GetPos000(),
                                                   n0.GetPos001(),
                                                   n0.GetPos101(),
                                                   n0.GetPos100(),
                                                   n0.GetPos010(),
                                                   n0.GetPos011(),
                                                   n0.GetPos111(),
                                                   n0.GetPos110() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos000()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos001()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos101()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos100()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos010()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos011()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n0.GetPos110())});
                }
                if ((c3 & ChunkOctree.BorderCode.RIGHT) != 0)
                {
                    CreateDualCell(new Vector3[] { n3.GetPos100(),
                                                   n3.GetPos101(),
                                                   n3.GetPos201(),
                                                   n3.GetPos200(),
                                                   n3.GetPos110(),
                                                   n3.GetPos111(),
                                                   n3.GetPos211(),
                                                   n3.GetPos210() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n3.GetPos100()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos101()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos201()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos200()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos110()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos211()),
                                                   terrainFunction.GetValueAndGradient(n3.GetPos210())});
                }

            }

            if ((c4c7 & ChunkOctree.BorderCode.TOP) != 0)
            {
                CreateDualCell(new Vector3[] { n4.GetPos110(),
                                               n4.GetPos111(),
                                               n7.GetPos111(),
                                               n7.GetPos110(),
                                               n4.GetPos120(),
                                               n4.GetPos121(),
                                               n7.GetPos121(),
                                               n7.GetPos120() },
                               new Vector4[] { terrainFunction.GetValueAndGradient(n4.GetPos110()),
                                               terrainFunction.GetValueAndGradient(n4.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n7.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n7.GetPos110()),
                                               terrainFunction.GetValueAndGradient(n4.GetPos120()),
                                               terrainFunction.GetValueAndGradient(n4.GetPos121()),
                                               terrainFunction.GetValueAndGradient(n7.GetPos121()),
                                               terrainFunction.GetValueAndGradient(n7.GetPos120())});

                if ((c4 & ChunkOctree.BorderCode.LEFT) != 0)
                {
                    CreateDualCell(new Vector3[] { n4.GetPos010(),
                                                   n4.GetPos011(),
                                                   n4.GetPos111(),
                                                   n4.GetPos110(),
                                                   n4.GetPos020(),
                                                   n4.GetPos021(),
                                                   n4.GetPos121(),
                                                   n4.GetPos120() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n4.GetPos010()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos011()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos110()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos020()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos021()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos121()),
                                                   terrainFunction.GetValueAndGradient(n4.GetPos120())});

                }
                if ((c7 & ChunkOctree.BorderCode.RIGHT) != 0)
                {
                    CreateDualCell(new Vector3[] { n7.GetPos110(),
                                                   n7.GetPos111(),
                                                   n7.GetPos211(),
                                                   n7.GetPos210(),
                                                   n7.GetPos120(),
                                                   n7.GetPos121(),
                                                   n7.GetPos221(),
                                                   n7.GetPos220() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n7.GetPos110()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos211()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos210()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos120()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos121()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos221()),
                                                   terrainFunction.GetValueAndGradient(n7.GetPos220())});
                }

            }

            if ((c0c4 & ChunkOctree.BorderCode.LEFT) != 0)
            {
                CreateDualCell(new Vector3[] { n0.GetPos010(),
                                               n0.GetPos011(),
                                               n0.GetPos111(),
                                               n0.GetPos110(),
                                               n4.GetPos010(),
                                               n4.GetPos011(),
                                               n4.GetPos111(),
                                               n4.GetPos110() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos010()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos110()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos010()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos110()) });
            }

            if ((c3c7 & ChunkOctree.BorderCode.RIGHT) != 0)
            {
                CreateDualCell(new Vector3[] { n3.GetPos110(),
                                               n3.GetPos111(),
                                               n3.GetPos211(),
                                               n3.GetPos210(),
                                               n7.GetPos110(),
                                               n7.GetPos111(),
                                               n7.GetPos211(),
                                               n7.GetPos210() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n3.GetPos110()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos210()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos110()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos210()) });
            }
        }

        //FRONT
        ChunkOctree.BorderCode c1c2 = c1 & c2;
        ChunkOctree.BorderCode c5c6 = c5 & c6;
        ChunkOctree.BorderCode c1c5 = c1 & c5;
        ChunkOctree.BorderCode c2c6 = c2 & c6;
        if ((c1c2 & c5c6 & ChunkOctree.BorderCode.FRONT) != 0)
        {
            CreateDualCell(new Vector3[] { n1.GetPos111(),
                                           n1.GetPos112(),
                                           n2.GetPos112(),
                                           n2.GetPos111(),
                                           n5.GetPos111(),
                                           n5.GetPos112(),
                                           n6.GetPos112(),
                                           n6.GetPos111() },
                           new Vector4[] { terrainFunction.GetValueAndGradient(n1.GetPos111()),
                                           terrainFunction.GetValueAndGradient(n1.GetPos112()),
                                           terrainFunction.GetValueAndGradient(n2.GetPos112()),
                                           terrainFunction.GetValueAndGradient(n2.GetPos111()),
                                           terrainFunction.GetValueAndGradient(n5.GetPos111()),
                                           terrainFunction.GetValueAndGradient(n5.GetPos112()),
                                           terrainFunction.GetValueAndGradient(n6.GetPos112()),
                                           terrainFunction.GetValueAndGradient(n6.GetPos111()) });

            if ((c1c2 & ChunkOctree.BorderCode.BOTTOM) != 0)
            {
                CreateDualCell(new Vector3[] { n1.GetPos101(),
                                               n1.GetPos102(),
                                               n2.GetPos102(),
                                               n2.GetPos101(),
                                               n1.GetPos111(),
                                               n1.GetPos112(),
                                               n2.GetPos112(),
                                               n2.GetPos111() },
                               new Vector4[] { terrainFunction.GetValueAndGradient(n1.GetPos101()),
                                               terrainFunction.GetValueAndGradient(n1.GetPos102()),
                                               terrainFunction.GetValueAndGradient(n2.GetPos102()),
                                               terrainFunction.GetValueAndGradient(n2.GetPos101()),
                                               terrainFunction.GetValueAndGradient(n1.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n1.GetPos112()),
                                               terrainFunction.GetValueAndGradient(n2.GetPos112()),
                                               terrainFunction.GetValueAndGradient(n2.GetPos111())});

                if ((c1 & ChunkOctree.BorderCode.LEFT) != 0)
                {
                    CreateDualCell(new Vector3[] { n1.GetPos001(),
                                                   n1.GetPos002(),
                                                   n1.GetPos102(),
                                                   n1.GetPos101(),
                                                   n1.GetPos011(),
                                                   n1.GetPos012(),
                                                   n1.GetPos112(),
                                                   n1.GetPos111() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n1.GetPos001()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos002()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos102()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos101()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos011()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos012()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos112()),
                                                   terrainFunction.GetValueAndGradient(n1.GetPos111())});
                }
                if ((c2 & ChunkOctree.BorderCode.RIGHT) != 0)
                {
                    CreateDualCell(new Vector3[] { n2.GetPos101(),
                                                   n2.GetPos102(),
                                                   n2.GetPos202(),
                                                   n2.GetPos201(),
                                                   n2.GetPos111(),
                                                   n2.GetPos112(),
                                                   n2.GetPos212(),
                                                   n2.GetPos211() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n2.GetPos101()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos102()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos202()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos201()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos112()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos212()),
                                                   terrainFunction.GetValueAndGradient(n2.GetPos211())});
                }

            }

            if ((c5c6 & ChunkOctree.BorderCode.TOP) != 0)
            {
                CreateDualCell(new Vector3[] { n5.GetPos111(),
                                               n5.GetPos112(),
                                               n6.GetPos112(),
                                               n6.GetPos111(),
                                               n5.GetPos121(),
                                               n5.GetPos122(),
                                               n6.GetPos122(),
                                               n6.GetPos121() },
                               new Vector4[] { terrainFunction.GetValueAndGradient(n5.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n5.GetPos112()),
                                               terrainFunction.GetValueAndGradient(n6.GetPos112()),
                                               terrainFunction.GetValueAndGradient(n6.GetPos111()),
                                               terrainFunction.GetValueAndGradient(n5.GetPos121()),
                                               terrainFunction.GetValueAndGradient(n5.GetPos122()),
                                               terrainFunction.GetValueAndGradient(n6.GetPos122()),
                                               terrainFunction.GetValueAndGradient(n6.GetPos121())});

                if ((c5 & ChunkOctree.BorderCode.LEFT) != 0)
                {
                    CreateDualCell(new Vector3[] { n5.GetPos011(),
                                                   n5.GetPos012(),
                                                   n5.GetPos112(),
                                                   n5.GetPos111(),
                                                   n5.GetPos021(),
                                                   n5.GetPos022(),
                                                   n5.GetPos122(),
                                                   n5.GetPos121() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n5.GetPos011()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos012()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos112()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos021()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos022()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos122()),
                                                   terrainFunction.GetValueAndGradient(n5.GetPos121())});
                }
                if ((c6 & ChunkOctree.BorderCode.RIGHT) != 0)
                {
                    CreateDualCell(new Vector3[] { n6.GetPos111(),
                                                   n6.GetPos112(),
                                                   n6.GetPos212(),
                                                   n6.GetPos211(),
                                                   n6.GetPos121(),
                                                   n6.GetPos122(),
                                                   n6.GetPos222(),
                                                   n6.GetPos221() },
                                   new Vector4[] { terrainFunction.GetValueAndGradient(n6.GetPos111()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos112()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos212()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos211()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos121()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos122()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos222()),
                                                   terrainFunction.GetValueAndGradient(n6.GetPos221())});
                }

            }

            if ((c1c5 & ChunkOctree.BorderCode.LEFT) != 0)
            {
                CreateDualCell(new Vector3[] { n1.GetPos011(),
                                               n1.GetPos012(),
                                               n1.GetPos112(),
                                               n1.GetPos111(),
                                               n5.GetPos011(),
                                               n5.GetPos012(),
                                               n5.GetPos112(),
                                               n5.GetPos111() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n1.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos012()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos112()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos012()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos112()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos111()) });
            }

            if ((c2c6 & ChunkOctree.BorderCode.RIGHT) != 0)
            {
                CreateDualCell(new Vector3[] { n2.GetPos111(),
                                               n2.GetPos112(),
                                               n2.GetPos212(),
                                               n2.GetPos211(),
                                               n6.GetPos111(),
                                               n6.GetPos112(),
                                               n6.GetPos212(),
                                               n6.GetPos211() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n2.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos112()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos212()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos112()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos212()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos211()) });
            }
        }


        //LEFT
        ChunkOctree.BorderCode c0c1 = c0 & c1;
        ChunkOctree.BorderCode c4c5 = c4 & c5;

        if ((c0c1 & c4c5 & ChunkOctree.BorderCode.LEFT) != 0)
        {
            CreateDualCell(new Vector3[] { n0.GetPos011(),
                                           n1.GetPos011(),
                                           n1.GetPos111(),
                                           n0.GetPos111(),
                                           n4.GetPos011(),
                                           n5.GetPos011(),
                                           n5.GetPos111(),
                                           n4.GetPos111() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos111()) });

            if ((c0c1 & ChunkOctree.BorderCode.BOTTOM) != 0)
            {
                CreateDualCell(new Vector3[] { n0.GetPos001(),
                                               n1.GetPos001(),
                                               n1.GetPos101(),
                                               n0.GetPos101(),
                                               n0.GetPos011(),
                                               n1.GetPos011(),
                                               n1.GetPos111(),
                                               n0.GetPos111() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos001()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos001()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos111()) });
            }

            if ((c4c5 & ChunkOctree.BorderCode.TOP) != 0)
            {
                CreateDualCell(new Vector3[] { n4.GetPos011(),
                                               n5.GetPos011(),
                                               n5.GetPos111(),
                                               n4.GetPos111(),
                                               n4.GetPos021(),
                                               n5.GetPos021(),
                                               n5.GetPos121(),
                                               n4.GetPos121() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n4.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos011()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos021()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos021()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos121()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos121()) });
            }
        }

        //RIGHT
        ChunkOctree.BorderCode c2c3 = c2 & c3;
        ChunkOctree.BorderCode c6c7 = c6 & c7;

        if ((c2c3 & c6c7 & ChunkOctree.BorderCode.RIGHT) != 0)
        {
            CreateDualCell(new Vector3[] { n3.GetPos111(),
                                           n2.GetPos111(),
                                           n2.GetPos211(),
                                           n3.GetPos211(),
                                           n7.GetPos111(),
                                           n6.GetPos111(),
                                           n6.GetPos211(),
                                           n7.GetPos211() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n3.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos211()) });

            if ((c2c3 & ChunkOctree.BorderCode.BOTTOM) != 0)
            {
                CreateDualCell(new Vector3[] { n3.GetPos101(),
                                               n2.GetPos101(),
                                               n2.GetPos201(),
                                               n3.GetPos201(),
                                               n3.GetPos111(),
                                               n2.GetPos111(),
                                               n2.GetPos211(),
                                               n3.GetPos211() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n3.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos201()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos201()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos211()) });
            }

            if ((c6c7 & ChunkOctree.BorderCode.TOP) != 0)
            {
                CreateDualCell(new Vector3[] { n7.GetPos111(),
                                               n6.GetPos111(),
                                               n6.GetPos211(),
                                               n7.GetPos211(),
                                               n7.GetPos121(),
                                               n6.GetPos121(),
                                               n6.GetPos221(),
                                               n7.GetPos221() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n7.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos211()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos121()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos121()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos221()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos221()) });
            }
        }

        //BOTTOM
        if ((c0c1 & c2c3 & ChunkOctree.BorderCode.BOTTOM) != 0)
        {
            CreateDualCell(new Vector3[] { n0.GetPos101(),
                                           n1.GetPos101(),
                                           n2.GetPos101(),
                                           n3.GetPos101(),
                                           n0.GetPos111(),
                                           n1.GetPos111(),
                                           n2.GetPos111(),
                                           n3.GetPos111() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n0.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos101()),
                                          terrainFunction.GetValueAndGradient(n0.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n1.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n2.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n3.GetPos111()) });
        }
        //TOP
        if ((c4c5 & c6c7 & ChunkOctree.BorderCode.TOP) != 0)
        {
            CreateDualCell(new Vector3[] { n4.GetPos111(),
                                           n5.GetPos111(),
                                           n6.GetPos111(),
                                           n7.GetPos111(),
                                           n4.GetPos121(),
                                           n5.GetPos121(),
                                           n6.GetPos121(),
                                           n7.GetPos121() },
                          new Vector4[] { terrainFunction.GetValueAndGradient(n4.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos111()),
                                          terrainFunction.GetValueAndGradient(n4.GetPos121()),
                                          terrainFunction.GetValueAndGradient(n5.GetPos121()),
                                          terrainFunction.GetValueAndGradient(n6.GetPos121()),
                                          terrainFunction.GetValueAndGradient(n7.GetPos121()) });
        }
    }
}
*/