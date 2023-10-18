using UnityEngine;


public class SpawnPointFinder : MonoBehaviour
{
    public ComputeShader computeShader;
    public ComputeBuffer resultBuffer;

    public void Find(Vector3 startLocation, out Vector3 foundPosition, out Vector3 foundDirection)
    {
        resultBuffer = new ComputeBuffer(2, 12);
        computeShader.SetBuffer(0, "resultBuffer", resultBuffer);
        computeShader.SetVector("startLocation", startLocation);
        computeShader.SetFloat("terrainWidthFactor", ProceduralWorld.Instance.GetTerrainWidthFactor());
        computeShader.Dispatch(0, 1, 1, 1);

        Vector3[] result = new Vector3[2];
        resultBuffer.GetData(result);

        foundPosition = result[0];
        foundDirection = result[1];
        resultBuffer.Dispose();
    }
}