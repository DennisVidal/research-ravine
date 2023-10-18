using UnityEngine;

public class ChunkOctreeSubdivisionData
{
    public Camera targetCamera;
    public Plane[] targetFrustumPlanes;

    public ChunkOctreeSubdivisionData(Camera targetCamera)
    {
        UpdateData(targetCamera);
    }

    public void UpdateData(Camera targetCamera)
    {
        this.targetCamera = targetCamera;
        targetFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(targetCamera);
    }

    public bool IsInFrustum(Bounds bounds)
    {
        return GeometryUtility.TestPlanesAABB(targetFrustumPlanes, bounds);
    }
}
