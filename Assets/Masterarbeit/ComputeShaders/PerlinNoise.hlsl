#ifndef PERLIN_NOISE
#define PERLIN_NOISE

#define DIV_256 0.00390625f

static const uint PERLIN_PERMUTATIONS[512] =
{
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
};

static const float3 PERLIN_GRADIENTS[] = 
{ 
    1,  1,  0,
   -1,  1,  0,
    1, -1,  0,
   -1, -1,  0,
    1,  0,  1,
   -1,  0,  1,
    1,  0, -1,
   -1,  0, -1,
    0,  1,  1,
    0, -1,  1,
    0,  1, -1,
    0, -1, -1,
    1,  1,  0,
    0, -1,  1,
   -1,  1,  0,
    0, -1, -1 
};

float2 Perlin_Fade(float2 t)
{
    return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
}
float3 Perlin_Fade(float3 t)
{
    return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
}

float Perlin_Gradient(uint hash, float2 position)
{
    return dot(position, PERLIN_GRADIENTS[hash & 3].xy);
}
float Perlin_Gradient(uint hash, float3 position)
{
    return dot(position, PERLIN_GRADIENTS[hash & 15]);
}

float PerlinNoise(float3 position)
{
    float3 cubePos = floor(position);
    position -= cubePos;
    cubePos -= floor(cubePos * DIV_256) * 256.0f;
    uint A = PERLIN_PERMUTATIONS[cubePos.x] + cubePos.y;
    uint AA = PERLIN_PERMUTATIONS[A] + cubePos.z;
    uint AB = PERLIN_PERMUTATIONS[A + 1] + cubePos.z;
    uint B = PERLIN_PERMUTATIONS[cubePos.x + 1] + cubePos.y;
    uint BA = PERLIN_PERMUTATIONS[B] + cubePos.z;
    uint BB = PERLIN_PERMUTATIONS[B + 1] + cubePos.z;
    
    float3 lerpValues = Perlin_Fade(position);
    float a = 1.0 - lerpValues.y;
    float b = 1.0 - lerpValues.z;
    float c = lerpValues.y * lerpValues.z;
    float d = a * b;
    b *= lerpValues.y;
    a *= lerpValues.z;
    float4 l = float4(d, b, a, c);
    
    float4 v0 = float4(Perlin_Gradient(PERLIN_PERMUTATIONS[AA],     position),
                       Perlin_Gradient(PERLIN_PERMUTATIONS[AB],     position - float3(0.0f, 1.0f, 0.0f)),
                       Perlin_Gradient(PERLIN_PERMUTATIONS[AA + 1], position - float3(0.0f, 0.0f, 1.0f)),
                       Perlin_Gradient(PERLIN_PERMUTATIONS[AB + 1], position - float3(0.0f, 1.0f, 1.0f)));
    float p0 = dot(v0, l);
    
    float4 v1 = float4(Perlin_Gradient(PERLIN_PERMUTATIONS[BA],     position - float3(1.0f, 0.0f, 0.0f)),
                       Perlin_Gradient(PERLIN_PERMUTATIONS[BB],     position - float3(1.0f, 1.0f, 0.0f)),
                       Perlin_Gradient(PERLIN_PERMUTATIONS[BA + 1], position - float3(1.0f, 0.0f, 1.0f)),
                       Perlin_Gradient(PERLIN_PERMUTATIONS[BB + 1], position - float3(1.0f, 1.0f, 1.0f)));
    float p1 = dot(v1, l);
    
    return lerp(p0, p1, lerpValues.x);
}

//float p0 = (Perlin_Gradient(PERLIN_PERMUTATIONS[AA], position) * d + Perlin_Gradient(PERLIN_PERMUTATIONS[AB], position - float3(0.0, 1.0, 0.0)) * b + Perlin_Gradient(PERLIN_PERMUTATIONS[AA + 1], position - float3(0.0, 0.0, 1.0)) * a + Perlin_Gradient(PERLIN_PERMUTATIONS[AB + 1], position - float3(0.0, 1.0, 1.0)) * c) * (1.0 - lerpValues.x);
//float p1 = Perlin_Gradient(PERLIN_PERMUTATIONS[BA + 1], position - float3(1.0, 0.0, 1.0)) * a + Perlin_Gradient(PERLIN_PERMUTATIONS[BB + 1], position - float3(1.0, 1.0, 1.0)) * c) * lerpValues.x;


float PerlinNoise(float2 position)
{
    float2 positionFloor = floor(position);
    float2 positionFrac = position - positionFloor;
    uint2 gridPos = positionFloor - floor(positionFloor * DIV_256) * 256.0f;
    
    float2 lerpValues = Perlin_Fade(positionFrac);
    
    uint A = PERLIN_PERMUTATIONS[gridPos.x] + gridPos.y;
    uint B = PERLIN_PERMUTATIONS[gridPos.x + 1] + gridPos.y;
    
    float x0 = lerp(Perlin_Gradient(PERLIN_PERMUTATIONS[A], positionFrac), Perlin_Gradient(PERLIN_PERMUTATIONS[B], positionFrac - float2(1.0f, 0.0f)), lerpValues.x);
    float x1 = lerp(Perlin_Gradient(PERLIN_PERMUTATIONS[A + 1], positionFrac - float2(0.0f, 1.0f)), Perlin_Gradient(PERLIN_PERMUTATIONS[B + 1], positionFrac - float2(1.0f, 1.0f)), lerpValues.x);
    return lerp(x0, x1, lerpValues.y);
}

#endif