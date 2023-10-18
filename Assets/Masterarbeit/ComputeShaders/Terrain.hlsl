#ifndef TERRAIN
#define TERRAIN
#include "PerlinNoise.hlsl"

#define PI 3.14159265359f


float3 drillRayStart;
float3 drillRayEnd;
float3 drillRayData; // x = min radius, y = max radius, z = strength
bool drillRayEnabled;

float terrainWidthFactor;

float GetDistanceToDrillRay(float3 position)
{
    float3 startToEnd = drillRayEnd - drillRayStart;
    float3 startToPosition = position - drillRayStart;
    float3 endToPosition = position - drillRayEnd;
    
    float dotAtStart = dot(startToEnd, startToPosition);
    float dotAtEnd = dot(startToEnd, endToPosition);
    
    
    return dotAtStart < 0.0f ? length(startToPosition) : (dotAtEnd > 0.0f ? length(endToPosition) : length(cross(startToEnd, startToPosition)) / length(startToEnd));
}

float GetDrillRayValue(float3 position)
{
    if (!drillRayEnabled)
    {
        return 0.0f;
    }
    
    float drillRayFactor = 1.0f - saturate((GetDistanceToDrillRay(position) - drillRayData.x) / (drillRayData.y - drillRayData.x));
    //return drillRayFactor * (-drillRayData.z + PerlinNoise(position * 0.1f) * 20.0f);
    return drillRayFactor * -drillRayData.z;
}


float GetTerrainValue(float3 position)
{
    ////Higher than max height? => outside
    //if (position.y > 110.0f) { return -10.0f; }
    ////Lower than min height? => inside
    //if (position.y < 2.0f) { return 10.0f; }
    
    //Combine 2 octaves of noise
    float noise = PerlinNoise(position * 0.001f) + PerlinNoise(position * 0.0023f) * 1.36f;
    
    //Create terrain base
    float pathValue = 1.0f - pow(1.0f - abs(noise * 0.42372881355f), terrainWidthFactor);
    
    
    float baseValue = saturate((pathValue - 0.02f) * 5.56f); //Adjust terrain steepness with thresholds
    //base *= (base > 0.95f) * 9.0f + 1.0f; //Boost base if it is at upper limit => removes holes stuff in terrain
    //float base = base * 2.0f - 1.0f;
    //base += sign((int) base);
    //base *= 0.7f + 0.3f * saturate(110.0f - position.y);
    
    //Add 3 more octaves for overall terrain detail
    noise += PerlinNoise(position * 0.039142f) * 0.5f;
    noise += PerlinNoise(position * 0.0871f) * 0.25f;
    noise += PerlinNoise(position * 0.16337f) * 0.125f;
    
    float terrainValue = lerp(baseValue * 2.0f - 1.0f, noise, pow(pathValue, 0.4f)); //.3
    //terrainValue += (baseValue > 0.8f) * 50.0f; //(terrainBase01 > 0.8f) * 20.0f //Boost base if it is at upper limit => removes holes stuff in terrain
    //terrainValue -= (0.0f < terrainValue && terrainValue < 0.1f) * 2.0f * terrainValue; //Remove small flying objects
    
    
    //Get terrain base at 2D ground level
    float3 groundPos = float3(position.x, 10.0f, position.z);
    float groundValue = PerlinNoise(groundPos * 0.001f) + PerlinNoise(groundPos * 0.0023f) * 1.36f;
    groundValue = 1.0f - pow(1.0f - abs(groundValue * 0.42372881355f), terrainWidthFactor);
    groundValue = (saturate(groundValue * 10.0f) * 10.0f - position.y) * 0.1f;
    groundValue += noise;
    
    terrainValue = lerp(groundValue, terrainValue, pow(saturate(position.y * 0.1f), 7.0f));
    
    terrainValue += (baseValue > 0.8f) * 50.0f; //(terrainBase01 > 0.8f) * 20.0f //Boost base if it is at upper limit => removes holes stuff in terrain
    terrainValue -= (0.0f < terrainValue && terrainValue < 0.1f) * 2.0f * terrainValue; //Remove small flying objects
    
    terrainValue += (position.y > 110.0f) * -1000.0f + (position.y < 2.0f) * 1000.0f;
    return terrainValue + GetDrillRayValue(position);
}

/*
//Old terrain function
float GetTerrainValue(float3 position)
{
    
   //return -position.y + 7.0f;
   float terrainBase01 = (PerlinNoise(position * 0.001f) + PerlinNoise(position * 0.0023f) * 1.36f) * 0.42372881355f; // /2.36f
   terrainBase01 = 1.0f - pow(1.0f - abs(terrainBase01), terrainWidthFactor); //7.06f;
   
   
   float groundHeight = 10.0f;
   float canyonHeight = 100.0f;
   
   if (position.y > groundHeight + canyonHeight)
   {
       return -10.0f;
   }
   else if (position.y < 2.0f)
   {
       return 10.0f;
   }
   
   
   
   float noise = 0.0f;
   noise += PerlinNoise(position * 0.04f) * 0.5f;
   noise += PerlinNoise(position * 0.08f) * 0.25f;
   noise += PerlinNoise(position * 0.16f) * 0.125f;
   
   
   float3 groundSamplePos = float3(position.x, groundHeight, position.z);
   float groundDensity = (PerlinNoise(groundSamplePos * 0.001f) + PerlinNoise(groundSamplePos * 0.0023f) * 1.36f) * 0.42372881355f;
   groundDensity = 1.0f - pow(1.0f - abs(groundDensity), terrainWidthFactor); //7.06f;
   
   //float groundHeightOffset = groundDensity * groundHeight - position.y;
   float groundHeightOffset = (saturate(groundDensity / 0.1f) * groundHeight - position.y) / groundHeight;
   //groundHeightOffset += sign(groundHeightOffset);
   groundHeightOffset += noise;
   
   float3 pos = float3(position.x, position.y * 0.5f, position.z);
   float lowerThreshold = 0.02f;
   float upperThreshold = 0.2f;
   float heightFactor = saturate((terrainBase01 - lowerThreshold) / (upperThreshold - lowerThreshold));
   
   if (heightFactor == 1.0f)
   {
       heightFactor *= 10.0f;
   }
   
   float value = heightFactor * 2.0f - 1.0f;
   value += sign((int) value);
   value *= 0.7f + 0.3f * saturate(canyonHeight - (position.y - groundHeight));
   
   
   value = lerp(value, noise, pow(terrainBase01, 0.3f));
   
   value += (terrainBase01 > 0.9f) * 20.0f;
   
   if (0.0f < value && value < 0.05f)
   {
       value = -value;
   }
   
   return GetDrillRayValue(position) + lerp(groundHeightOffset, value, pow(saturate(position.y / groundHeight), 7.0f));
    
}
*/
#endif












//float GetTerrainBase(float2 positionXZ, float powerFactor)
//{
//    
//    float noise = PerlinNoise(positionXZ * 0.001f);
//    noise += PerlinNoise(positionXZ * 0.0023f) * 1.36f;
//    noise /= 2.36f;
//    noise = abs(noise);
//    noise = pow(1.0f - noise, powerFactor);
//    return 1.0f - noise;
//    
//    //float lowerThreshold = 0.1f;
//    //float upperThreshold = 0.3f;
//    //return saturate((noise - lowerThreshold) / (upperThreshold - lowerThreshold));
//}
//float GetTerrainBase(float3 position, float powerFactor)
//{
//    
//    float noise = PerlinNoise(position * 0.001f);
//    noise += PerlinNoise(position * 0.0023f) * 1.36f;
//    noise /= 2.36f;
//    noise = abs(noise);
//    noise = pow(1.0f - noise, powerFactor);
//    return 1.0f - noise;
//}