using UnityEngine;
/*
public class ChunkOctree
{
    //     5----------6
    //    /|         /|
    //   / |        / |
    //  4----------7  |
    //  |  |       |  |
    //  |  1-------|--2     
    //  | /        | /      y z
    //  |/         |/       |/
    //  0----------3        +--X

    public enum BorderCode
    {
        NONE = 0,

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

    public static Vector3[] LERP_POSITIONS =
    {
        new Vector3(0.0f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.5f, 0.0f),
        new Vector3(0.0f, 0.5f, 1.0f),
        new Vector3(1.0f, 0.5f, 1.0f),
        new Vector3(1.0f, 0.5f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.5f),
        new Vector3(0.5f, 1.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 0.5f),
        new Vector3(0.5f, 1.0f, 0.0f),
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.0f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 1.0f),
        new Vector3(1.0f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, 1.0f, 0.5f)
    };

    public static float MIN_SIZE = 0.5f;

    public Vector4 sample { get; set; }
    public Bounds bounds { get; set; }
    public ChunkOctree[] children { get; set; }
    public BorderCode borderCode { get; set; }


    public ChunkOctree()
    {

    }
    public ChunkOctree(Bounds b, BorderCode code = BorderCode.ALL)
    {
        bounds = b;
        borderCode = code;
    }

    public bool IsLeaf()
    {
        return children == null;
    }
    public bool IsSubdivided()
    {
        return children != null;
    }

    public void Subdivide(ProceduralTerrain terrainFunction, float maxError, float[] cornerValues)
    {
        Vector3 mid = bounds.center;
        sample = terrainFunction.GetValueAndGradient(mid);
        if (bounds.size.x <= MIN_SIZE)
        {
            return;
        }

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Vector3[] positions = { // 0 -> 17, 18(center) separately
            new Vector3(min.x, min.y, mid.z),
            new Vector3(mid.x, min.y, max.z),
            new Vector3(max.x, min.y, mid.z),
            new Vector3(mid.x, min.y, min.z),
            new Vector3(min.x, mid.y, min.z),
            new Vector3(min.x, mid.y, max.z),
            new Vector3(max.x, mid.y, max.z),
            new Vector3(max.x, mid.y, min.z),
            new Vector3(min.x, max.y, mid.z),
            new Vector3(mid.x, max.y, max.z),
            new Vector3(max.x, max.y, mid.z),
            new Vector3(mid.x, max.y, min.z),
            new Vector3(mid.x, min.y, mid.z),
            new Vector3(min.x, mid.y, mid.z),
            new Vector3(mid.x, mid.y, max.z),
            new Vector3(max.x, mid.y, mid.z),
            new Vector3(mid.x, mid.y, min.z),
            new Vector3(mid.x, max.y, mid.z)
        };


        float[] samples = new float[18];
        float error = 0.0f, interpolatedValue;
        bool shouldSubdivide = false;
        int i;
        for (i = 0; i < 18; i++)
        {
            Vector4 gradientAndValue = terrainFunction.GetValueAndGradient(positions[i]);
            samples[i] = gradientAndValue.w;

            Vector3 pos = LERP_POSITIONS[i];
            float a = 1.0f - pos.y;
            float b = 1.0f - pos.z;
            float c = pos.y * pos.z;
            float d = a * b;
            b *= pos.y;
            a *= pos.z;
            interpolatedValue = (cornerValues[0] * d + cornerValues[4] * b + cornerValues[1] * a + cornerValues[5] * c) * (1.0f - pos.x) + (cornerValues[3] * d + cornerValues[7] * b + cornerValues[2] * a + cornerValues[6] * c) * pos.x;
            
            float difference = Mathf.Abs(samples[i] - interpolatedValue);
            float gradientMagnitude = new Vector3(gradientAndValue.x, gradientAndValue.y, gradientAndValue.z).magnitude;
            error += difference / (gradientMagnitude < float.Epsilon ? 1.0f : gradientMagnitude);
            shouldSubdivide = error >= maxError;
            if(shouldSubdivide)
            {
                break;
            }
        }

        if (!shouldSubdivide)
        {
            interpolatedValue = (cornerValues[0] + cornerValues[4] + cornerValues[1] + cornerValues[5] + cornerValues[3] + cornerValues[7] + cornerValues[2] + cornerValues[6]) * 0.125f;
            float gradientMagnitude = Mathf.Sqrt(sample.x * sample.x + sample.y * sample.y + sample.z * sample.z);
            error += Mathf.Abs(sample.w - interpolatedValue) / (gradientMagnitude < float.Epsilon ? 1.0f : gradientMagnitude);
            if (error < maxError)
            {
                return;
            }
        }

        //Should subdivide further => get missing values
        for (; i < 18; i++)
        {
            Vector3 pos = positions[i];
            samples[i] = terrainFunction.GetValue(new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z));
        }



        Vector3 size = bounds.extents;
        Vector3 extents = size * 0.5f;

        children = new ChunkOctree[]
        {
            new ChunkOctree(new Bounds(mid - extents, size),                                        borderCode & BorderCode.BOTTOM_BACK_LEFT),
            new ChunkOctree(new Bounds(mid + new Vector3(-extents.x, -extents.y, extents.z), size), borderCode & BorderCode.BOTTOM_FRONT_LEFT),
            new ChunkOctree(new Bounds(mid + new Vector3(extents.x, -extents.y, extents.z), size),  borderCode & BorderCode.BOTTOM_FRONT_RIGHT),
            new ChunkOctree(new Bounds(mid + new Vector3(extents.x, -extents.y, -extents.z), size), borderCode & BorderCode.BOTTOM_BACK_RIGHT),
            new ChunkOctree(new Bounds(mid + new Vector3(-extents.x, extents.y, -extents.z), size), borderCode & BorderCode.TOP_BACK_LEFT),
            new ChunkOctree(new Bounds(mid + new Vector3(-extents.x, extents.y, extents.z), size),  borderCode & BorderCode.TOP_FRONT_LEFT),
            new ChunkOctree(new Bounds(mid + extents, size),                                        borderCode & BorderCode.TOP_FRONT_RIGHT),
            new ChunkOctree(new Bounds(mid + new Vector3(extents.x, extents.y, -extents.z), size),  borderCode & BorderCode.TOP_BACK_RIGHT)
        };

        children[0].Subdivide(terrainFunction, maxError, new float[] { cornerValues[0], samples[0], samples[12], samples[3], samples[4], samples[13], sample.w, samples[16] });
        children[1].Subdivide(terrainFunction, maxError, new float[] { samples[0], cornerValues[1], samples[1], samples[12], samples[13], samples[5], samples[14], sample.w });
        children[2].Subdivide(terrainFunction, maxError, new float[] { samples[12], samples[1], cornerValues[2], samples[2], sample.w, samples[14], samples[6], samples[13] });
        children[3].Subdivide(terrainFunction, maxError, new float[] { samples[3], samples[12], samples[2], cornerValues[3], samples[16], sample.w, samples[15], samples[7] });
        children[4].Subdivide(terrainFunction, maxError, new float[] { samples[4], samples[13], sample.w, samples[16], cornerValues[4], samples[8], samples[17], samples[11] });
        children[5].Subdivide(terrainFunction, maxError, new float[] { samples[13], samples[5], samples[14], sample.w, samples[8], cornerValues[5], samples[9], samples[17] });
        children[6].Subdivide(terrainFunction, maxError, new float[] { sample.w, samples[14], samples[6], samples[15], samples[17], samples[9], cornerValues[6], samples[10] });
        children[7].Subdivide(terrainFunction, maxError, new float[] { samples[16], sample.w, samples[15], samples[7], samples[11], samples[17], samples[10], cornerValues[7] });
    }

    #region GetPos

    public Vector3 GetPosition()
    {
        return bounds.center;
    }

    public Vector3 GetPos(int posX, int posY, int posZ)
    {
        float x = posX == 0 ? bounds.min.x : posX == 1 ? bounds.center.x : bounds.max.x;
        float y = posY == 0 ? bounds.min.y : posY == 1 ? bounds.center.y : bounds.max.y;
        float z = posZ == 0 ? bounds.min.z : posZ == 1 ? bounds.center.z : bounds.max.z;
        return new Vector3(x, y, z);
    }

    public Vector3 GetPos000()
    {
        return bounds.min;
    }
    public Vector3 GetPos100()
    {
        return new Vector3(bounds.center.x, bounds.min.y, bounds.min.z);
    }
    public Vector3 GetPos200()
    {
        return new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
    }
    public Vector3 GetPos010()
    {
        return new Vector3(bounds.min.x, bounds.center.y, bounds.min.z);
    }
    public Vector3 GetPos110()
    {
        return new Vector3(bounds.center.x, bounds.center.y, bounds.min.z);
    }
    public Vector3 GetPos210()
    {
        return new Vector3(bounds.max.x, bounds.center.y, bounds.min.z);
    }
    public Vector3 GetPos020()
    {
        return new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
    }
    public Vector3 GetPos120()
    {
        return new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
    }
    public Vector3 GetPos220()
    {
        return new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
    }

    public Vector3 GetPos001()
    {
        return new Vector3(bounds.min.x, bounds.min.y, bounds.center.z);
    }
    public Vector3 GetPos101()
    {
        return new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
    }
    public Vector3 GetPos201()
    {
        return new Vector3(bounds.max.x, bounds.min.y, bounds.center.z);
    }
    public Vector3 GetPos011()
    {
        return new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);
    }
    public Vector3 GetPos111()
    {
        return bounds.center;
    }
    public Vector3 GetPos211()
    {
        return new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
    }
    public Vector3 GetPos021()
    {
        return new Vector3(bounds.min.x, bounds.max.y, bounds.center.z);
    }
    public Vector3 GetPos121()
    {
        return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
    }
    public Vector3 GetPos221()
    {
        return new Vector3(bounds.max.x, bounds.max.y, bounds.center.z);
    }

    public Vector3 GetPos002()
    {
        return new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
    }
    public Vector3 GetPos102()
    {
        return new Vector3(bounds.center.x, bounds.min.y, bounds.max.z);
    }
    public Vector3 GetPos202()
    {
        return new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
    }
    public Vector3 GetPos012()
    {
        return new Vector3(bounds.min.x, bounds.center.y, bounds.max.z);
    }
    public Vector3 GetPos112()
    {
        return new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
    }
    public Vector3 GetPos212()
    {
        return new Vector3(bounds.max.x, bounds.center.y, bounds.max.z);
    }
    public Vector3 GetPos022()
    {
        return new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    }
    public Vector3 GetPos122()
    {
        return new Vector3(bounds.center.x, bounds.max.y, bounds.max.z);
    }
    public Vector3 GetPos222()
    {
        return bounds.max;
    }
    #endregion GetPos

}
*/

/*
 using UnityEngine;

public enum DensityOctreeBorderCode
{
    NONE = 0,

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

public class DensityOctree
{
    static Vector3[] interpolationPositions =
    {
        new Vector3(0.0f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.5f, 0.0f),
        new Vector3(0.0f, 0.5f, 1.0f),
        new Vector3(1.0f, 0.5f, 1.0f),
        new Vector3(1.0f, 0.5f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.5f),
        new Vector3(0.5f, 1.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 0.5f),
        new Vector3(0.5f, 1.0f, 0.0f),
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.0f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 1.0f),
        new Vector3(1.0f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, 1.0f, 0.5f)
    };


    static float MIN_SIZE = 0.5f;

    Vector3 gradient;
    float value;
    public Bounds bounds;
    public DensityOctree[] children;

    public DensityOctreeBorderCode borderCode;

    public void OnGizmo()
    {
        if (IsLeaf())
        {
            //float val = 0.5f * (value + 1.0f);
            //Gizmos.color = new Color(val, val, val);

            Gizmos.color = Color.white;
            if (borderCode != DensityOctreeBorderCode.NONE)
            {
                Gizmos.color = Color.black;
            }

            Gizmos.DrawSphere(bounds.center, 0.1f);
        }
        else
        {
            for(int i = 0; i < children.Length; i++)
            {
                children[i].OnGizmo();
            }
        }
    }
    
    public DensityOctree()
    {
    
    }

    public Vector3 GetPosition()
    {
        return bounds.center;
    }

    public Vector4 GetGradientAndValue()
    {
        return new Vector4(gradient.x, gradient.y, gradient.z, value);
    }

    DensityOctree CreateChild(Vector3 center, Vector3 size, DensityOctreeBorderCode code)
    {
        DensityOctree child = new DensityOctree();
        child.Init(center, size, code);
        return child;
    }

    public bool IsLeaf()
    {
        return children == null;
    }

    public bool IsSubdivided()
    {
        return children != null;
    }

    public void Init(Vector3 center, Vector3 size, DensityOctreeBorderCode code)
    {
        bounds = new Bounds(center, size);
        borderCode = code;
    }

    public void Subdivide(TerrainDensityField terrainFunction, float maxError, float[] cornerValues)
    {
        Vector3 mid = bounds.center;
        Vector4 midValueAndGradient = terrainFunction.GetValueAndGradient(mid);
        gradient = new Vector3(midValueAndGradient.x, midValueAndGradient.y, midValueAndGradient.z);
        value = midValueAndGradient.w;
        if (bounds.size.x <= MIN_SIZE)
        {
            return;
        }

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Vector3[] positions = { // 0 -> 17, 18(center) separately
            new Vector3(min.x, min.y, mid.z),
            new Vector3(mid.x, min.y, max.z),
            new Vector3(max.x, min.y, mid.z),
            new Vector3(mid.x, min.y, min.z),
            new Vector3(min.x, mid.y, min.z),
            new Vector3(min.x, mid.y, max.z),
            new Vector3(max.x, mid.y, max.z),
            new Vector3(max.x, mid.y, min.z),
            new Vector3(min.x, max.y, mid.z),
            new Vector3(mid.x, max.y, max.z),
            new Vector3(max.x, max.y, mid.z),
            new Vector3(mid.x, max.y, min.z),
            new Vector3(mid.x, min.y, mid.z),
            new Vector3(min.x, mid.y, mid.z),
            new Vector3(mid.x, mid.y, max.z),
            new Vector3(max.x, mid.y, mid.z),
            new Vector3(mid.x, mid.y, min.z),
            new Vector3(mid.x, max.y, mid.z)
        };

        float[] samples = new float[18];

        float error = 0.0f, a, b, c, d, interpolatedValue, difference, gradientMagnitude;
        bool shouldSubdivide = false;
        int i;
        for (i = 0; i < 18 && !shouldSubdivide; i++)
        {
            Vector4 gradientAndValue = terrainFunction.GetValueAndGradient(positions[i]);
            samples[i] = gradientAndValue.w;
            Vector3 pos = interpolationPositions[i];
            a = 1.0f - pos.y;
            b = 1.0f - pos.z;
            c = pos.y * pos.z;
            d = a * b;
            b *= pos.y;
            a *= pos.z;
            interpolatedValue = (cornerValues[0] * d + cornerValues[4] * b + cornerValues[1] * a + cornerValues[5] * c) * (1.0f - pos.x) + (cornerValues[3] * d + cornerValues[7] * b + cornerValues[2] * a + cornerValues[6] * c) * pos.x;
            difference = Mathf.Abs(samples[i] - interpolatedValue);
            gradientMagnitude = Mathf.Max(new Vector3(gradientAndValue.x, gradientAndValue.y, gradientAndValue.z).magnitude, 1.0f);
            error += difference / gradientMagnitude;

            shouldSubdivide = error >= maxError;
        }

        if(!shouldSubdivide)
        {
            interpolatedValue = (cornerValues[0] + cornerValues[4] + cornerValues[1] + cornerValues[5] + cornerValues[3] + cornerValues[7] + cornerValues[2] + cornerValues[6]) * 0.125f;
            error += Mathf.Abs(value - interpolatedValue) / Mathf.Max(gradient.magnitude, 1.0f);
            if (error < maxError)
            {
                return;
            }
        }

        //Should subdivide further => get missing values
        for (; i < 18; i++)
        {
            samples[i] = terrainFunction.GetValue(positions[i]);
        }



        Vector3 size = bounds.extents;
        Vector3 extents = size * 0.5f;

        children = new DensityOctree[8];
        children[0] = CreateChild(mid - extents, size, borderCode & DensityOctreeBorderCode.BOTTOM_BACK_LEFT);
        children[1] = CreateChild(mid + new Vector3(-extents.x, -extents.y, extents.z), size, borderCode & DensityOctreeBorderCode.BOTTOM_FRONT_LEFT);
        children[2] = CreateChild(mid + new Vector3(extents.x, -extents.y, extents.z), size, borderCode & DensityOctreeBorderCode.BOTTOM_FRONT_RIGHT);
        children[3] = CreateChild(mid + new Vector3(extents.x, -extents.y, -extents.z), size, borderCode & DensityOctreeBorderCode.BOTTOM_BACK_RIGHT);
        children[4] = CreateChild(mid + new Vector3(-extents.x, extents.y, -extents.z), size, borderCode & DensityOctreeBorderCode.TOP_BACK_LEFT);
        children[5] = CreateChild(mid + new Vector3(-extents.x, extents.y, extents.z), size, borderCode & DensityOctreeBorderCode.TOP_FRONT_LEFT);
        children[6] = CreateChild(mid + extents, size, borderCode & DensityOctreeBorderCode.TOP_FRONT_RIGHT);
        children[7] = CreateChild(mid + new Vector3(extents.x, extents.y, -extents.z), size, borderCode & DensityOctreeBorderCode.TOP_BACK_RIGHT);
        
        children[0].Subdivide(terrainFunction, maxError, new float[] { cornerValues[0], samples[0], samples[12], samples[3], samples[4], samples[13], value, samples[16] });
        children[1].Subdivide(terrainFunction, maxError, new float[] { samples[0], cornerValues[1], samples[1], samples[12], samples[13], samples[5], samples[14], value });
        children[2].Subdivide(terrainFunction, maxError, new float[] { samples[12], samples[1], cornerValues[2], samples[2], value, samples[14], samples[6], samples[13] });
        children[3].Subdivide(terrainFunction, maxError, new float[] { samples[3], samples[12], samples[2], cornerValues[3], samples[16], value, samples[15], samples[7] });
        children[4].Subdivide(terrainFunction, maxError, new float[] { samples[4], samples[13], value, samples[16], cornerValues[4], samples[8], samples[17], samples[11] });
        children[5].Subdivide(terrainFunction, maxError, new float[] { samples[13], samples[5], samples[14], value, samples[8], cornerValues[5], samples[9], samples[17] });
        children[6].Subdivide(terrainFunction, maxError, new float[] { value, samples[14], samples[6], samples[15], samples[17], samples[9], cornerValues[6], samples[10] });
        children[7].Subdivide(terrainFunction, maxError, new float[] { samples[16], value, samples[15], samples[7], samples[11], samples[17], samples[10], cornerValues[7] });
    }









    public Vector3 GetPos000()
    {
        return bounds.min;
    }
    public Vector3 GetPos100()
    {
        return new Vector3(bounds.center.x, bounds.min.y, bounds.min.z);
    }
    public Vector3 GetPos200()
    {
        return new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
    }
    public Vector3 GetPos010()
    {
        return new Vector3(bounds.min.x, bounds.center.y, bounds.min.z);
    }
    public Vector3 GetPos110()
    {
        return new Vector3(bounds.center.x, bounds.center.y, bounds.min.z);
    }
    public Vector3 GetPos210()
    {
        return new Vector3(bounds.max.x, bounds.center.y, bounds.min.z);
    }
    public Vector3 GetPos020()
    {
        return new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
    }
    public Vector3 GetPos120()
    {
        return new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
    }
    public Vector3 GetPos220()
    {
        return new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
    }


    public Vector3 GetPos001()
    {
        return new Vector3(bounds.min.x, bounds.min.y, bounds.center.z);
    }
    public Vector3 GetPos101()
    {
        return new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
    }
    public Vector3 GetPos201()
    {
        return new Vector3(bounds.max.x, bounds.min.y, bounds.center.z);
    }
    public Vector3 GetPos011()
    {
        return new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);
    }
    public Vector3 GetPos111()
    {
        return bounds.center;
    }
    public Vector3 GetPos211()
    {
        return new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
    }
    public Vector3 GetPos021()
    {
        return new Vector3(bounds.min.x, bounds.max.y, bounds.center.z);
    }
    public Vector3 GetPos121()
    {
        return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
    }
    public Vector3 GetPos221()
    {
        return new Vector3(bounds.max.x, bounds.max.y, bounds.center.z);
    }


    public Vector3 GetPos002()
    {
        return new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
    }
    public Vector3 GetPos102()
    {
        return new Vector3(bounds.center.x, bounds.min.y, bounds.max.z);
    }
    public Vector3 GetPos202()
    {
        return new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
    }
    public Vector3 GetPos012()
    {
        return new Vector3(bounds.min.x, bounds.center.y, bounds.max.z);
    }
    public Vector3 GetPos112()
    {
        return new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
    }
    public Vector3 GetPos212()
    {
        return new Vector3(bounds.max.x, bounds.center.y, bounds.max.z);
    }
    public Vector3 GetPos022()
    {
        return new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    }
    public Vector3 GetPos122()
    {
        return new Vector3(bounds.center.x, bounds.max.y, bounds.max.z);
    }
    public Vector3 GetPos222()
    {
        return bounds.max;
    }

}
 */