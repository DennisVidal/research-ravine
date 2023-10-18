using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunctionCPU
{
    public static float PI = 3.14159265359f;
    public static float DIV_256 = 0.00390625f;

    static int[] PERLIN_PERMUTATIONS = new int[512]
    {
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    };

    static Vector3[] PERLIN_GRADIENTS = new Vector3[]
    {
        new Vector3( 1.0f,  1.0f,  0.0f),
        new Vector3(-1.0f,  1.0f,  0.0f),
        new Vector3( 1.0f, -1.0f,  0.0f),
        new Vector3(-1.0f, -1.0f,  0.0f),
        new Vector3( 1.0f,  0.0f,  1.0f),
        new Vector3(-1.0f,  0.0f,  1.0f),
        new Vector3( 1.0f,  0.0f, -1.0f),
        new Vector3(-1.0f,  0.0f, -1.0f),
        new Vector3( 0.0f,  1.0f,  1.0f),
        new Vector3( 0.0f, -1.0f,  1.0f),
        new Vector3( 0.0f,  1.0f, -1.0f),
        new Vector3( 0.0f, -1.0f, -1.0f),
        new Vector3( 1.0f,  1.0f,  0.0f),
        new Vector3( 0.0f, -1.0f,  1.0f),
        new Vector3(-1.0f,  1.0f,  0.0f),
        new Vector3( 0.0f, -1.0f, -1.0f)
    };


    public static Vector3 Perlin_Fade(Vector3 t)
    {
        float x = t.x;
        float y = t.y;
        float z = t.z;

        float fadeX = x * x * x * (x * (x * 6.0f - 15.0f) + 10.0f);
        float fadeY = y * y * y * (y * (y * 6.0f - 15.0f) + 10.0f);
        float fadeZ = z * z * z * (z * (z * 6.0f - 15.0f) + 10.0f);

        return new Vector3(fadeX, fadeY, fadeZ);
    }
    public static float Perlin_Gradient(int hash, Vector3 pos)
    {
        return Vector3.Dot(pos, PERLIN_GRADIENTS[hash & 15]);
    }

    public static float PerlinNoise(Vector3 position)
    {
        Vector3 cubePos = new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z));
        position -= cubePos;
        cubePos -= new Vector3(Mathf.Floor(cubePos.x / 256), Mathf.Floor(cubePos.y / 256), Mathf.Floor(cubePos.z / 256))*256;

        Vector3 lerpValues = Perlin_Fade(position);

        int A =  PERLIN_PERMUTATIONS[(int)cubePos.x] + (int)cubePos.y;
        int AA = PERLIN_PERMUTATIONS[A] + (int)cubePos.z;
        int AB = PERLIN_PERMUTATIONS[A + 1] + (int)cubePos.z;
        int B =  PERLIN_PERMUTATIONS[(int)cubePos.x + 1] + (int)cubePos.y;
        int BA = PERLIN_PERMUTATIONS[B] + (int)cubePos.z;
        int BB = PERLIN_PERMUTATIONS[B + 1] + (int)cubePos.z;

        float a = 1.0f - lerpValues.y;
        float b = 1.0f - lerpValues.z;
        float c = lerpValues.y * lerpValues.z;
        float d = a * b;
        b *= lerpValues.y;
        a *= lerpValues.z;


        Vector4 l = new Vector4(d, b, a, c);

        Vector4 v0 = new Vector4(Perlin_Gradient(PERLIN_PERMUTATIONS[AA], position),
                           Perlin_Gradient(PERLIN_PERMUTATIONS[AB], position - new Vector3(0.0f, 1.0f, 0.0f)),
                           Perlin_Gradient(PERLIN_PERMUTATIONS[AA + 1], position - new Vector3(0.0f, 0.0f, 1.0f)),
                           Perlin_Gradient(PERLIN_PERMUTATIONS[AB + 1], position - new Vector3(0.0f, 1.0f, 1.0f)));
        float p0 = v0.x * l.x + v0.y * l.y + v0.z * l.z + v0.w * l.w;


        Vector4 v1 = new Vector4(Perlin_Gradient(PERLIN_PERMUTATIONS[BA], position - new Vector3(1.0f, 0.0f, 0.0f)),
                           Perlin_Gradient(PERLIN_PERMUTATIONS[BB], position - new Vector3(1.0f, 1.0f, 0.0f)),
                           Perlin_Gradient(PERLIN_PERMUTATIONS[BA + 1], position - new Vector3(1.0f, 0.0f, 1.0f)),
                           Perlin_Gradient(PERLIN_PERMUTATIONS[BB + 1], position - new Vector3(1.0f, 1.0f, 1.0f)));
        float p1 = v1.x * l.x + v1.y * l.y + v1.z * l.z + v1.w * l.w;

        return Mathf.Lerp(p0, p1, lerpValues.x);
    }


    public static float GetTerrainValue(Vector3 position)
    {
        float noise = PerlinNoise(position * 0.001f) + PerlinNoise(position * 0.0023f) * 1.36f;
        float pathValue = 1.0f - Mathf.Pow(1.0f - Mathf.Abs(noise * 0.42372881355f), GetTerrainWidthFactor());
        float baseValue = Mathf.Clamp01((pathValue - 0.02f) * 5.56f);
        noise += PerlinNoise(position * 0.039142f) * 0.5f;
        noise += PerlinNoise(position * 0.0871f) * 0.25f;
        noise += PerlinNoise(position * 0.16337f) * 0.125f;
        float terrainValue = Mathf.Lerp(baseValue * 2.0f - 1.0f, noise, Mathf.Pow(pathValue, 0.4f));
        Vector3 groundPos = new Vector3(position.x, 10.0f, position.z);
        float groundValue = PerlinNoise(groundPos * 0.001f) + PerlinNoise(groundPos * 0.0023f) * 1.36f;
        groundValue = 1.0f - Mathf.Pow(1.0f - Mathf.Abs(groundValue * 0.42372881355f), GetTerrainWidthFactor());
        groundValue = (Mathf.Clamp01(groundValue * 10.0f) * 10.0f - position.y) * 0.1f;
        groundValue += noise;
        terrainValue = Mathf.Lerp(groundValue, terrainValue, Mathf.Pow(Mathf.Clamp01(position.y * 0.1f), 7.0f));
        terrainValue += baseValue > 0.8f ? 50.0f : 0.0f;
        terrainValue = (0.0f < terrainValue && terrainValue < 0.1f)  ? -terrainValue : terrainValue;
        terrainValue += (position.y > 110.0f) ? -1000.0f : ((position.y < 2.0f) ? 1000.0f : 0.0f);
        return terrainValue;
    }


    //public static float Terrain_GetValue(Vector3 position)
    //{
    //    float terrainBase01 = (PerlinNoise(position * 0.001f) + PerlinNoise(position * 0.0023f) * 1.36f) * 0.42372881355f; // /2.36f
    //    terrainBase01 = 1.0f - Mathf.Pow(1.0f - Mathf.Abs(terrainBase01), GetTerrainWidthFactor());
    //
    //
    //    float groundHeight = 10.0f;
    //    float canyonHeight = 100.0f;
    //    if (position.y > groundHeight + canyonHeight)
    //    {
    //        return -10.0f;
    //    }
    //    else if (position.y < 2.0f)
    //    {
    //        return 10.0f;
    //    }
    //
    //
    //
    //    float noise = 0.0f;
    //    noise += PerlinNoise(position * 0.04f) * 0.5f;
    //    noise += PerlinNoise(position * 0.08f) * 0.25f;
    //    noise += PerlinNoise(position * 0.16f) * 0.125f;
    //
    //
    //    Vector3 groundSamplePos = new Vector3(position.x, groundHeight, position.z);
    //    float groundDensity = (PerlinNoise(groundSamplePos * 0.001f) + PerlinNoise(groundSamplePos * 0.0023f) * 1.36f) * 0.42372881355f;
    //    groundDensity = 1.0f - Mathf.Pow(1.0f - Mathf.Abs(groundDensity), GetTerrainWidthFactor());
    //
    //    float groundHeightOffset = (Mathf.Clamp01(groundDensity * 10.0f) * groundHeight - position.y) / groundHeight;
    //    groundHeightOffset += noise;
    //
    //    float lowerThreshold = 0.02f;
    //    float upperThreshold = 0.2f;
    //    float heightFactor = Mathf.Clamp01((terrainBase01 - lowerThreshold) / (upperThreshold - lowerThreshold));
    //
    //    if (heightFactor == 1.0f)
    //    {
    //        heightFactor *= 10.0f;
    //    }
    //
    //    float value = heightFactor * 2.0f - 1.0f;
    //    value += System.Math.Sign((int)value); //Unity's Mathf.Sign returns 1 when input is 0, hlsl's sign function would return 0
    //    value *= 0.7f + 0.3f * Mathf.Clamp01(canyonHeight - (position.y - groundHeight));
    //    
    //    
    //    value = Mathf.Lerp(value, noise, Mathf.Pow(terrainBase01, 0.3f));
    //    
    //    value += (terrainBase01 > 0.9f) ? 20.0f : 0.0f;
    //    
    //    if (0.0f < value && value < 0.05f)
    //    {
    //        value = -value;
    //    }
    //
    //    return Mathf.Lerp(groundHeightOffset, value, Mathf.Pow(Mathf.Clamp01(position.y / groundHeight), 7.0f));
    //}

    public static bool IsInTerrain(Vector3 position)
    {
        return GetTerrainValue(position) > 0.0f;
    }


    public static float GetTerrainWidthFactor()
    {
        return ProceduralWorld.Instance.GetTerrainWidthFactor();
    }
}
