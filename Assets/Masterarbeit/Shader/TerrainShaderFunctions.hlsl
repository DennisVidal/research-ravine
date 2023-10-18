float3x2 GetTriplanarUVs(float3 position)
{
    float3x2 uv;
    uv[0] = position.zy;
    uv[1] = position.xz;
    uv[2] = position.xy;
    return uv;
}

float4 GetBlendValues(float3 normal, float blendExponent)
{
    float4 blendValues;
    blendValues.xyz = abs(normal);
    blendValues.yw = float2(normal.y > 0.0f, normal.y <= 0.0f) * blendValues.y;
    blendValues = pow(blendValues, blendExponent);
    return blendValues / dot(blendValues, 1.0f);
}

float4 BlendColors(float4 colorX, float4 colorY, float4 colorZ, float4 colorNegY, float4 blendValues)
{
    return colorX * blendValues.x + colorY * blendValues.y + colorZ * blendValues.z + colorNegY * blendValues.w;
}

float4 BlendColors(float4x4 colors, float4 blendValues)
{
    return BlendColors(colors[0], colors[1], colors[2], colors[3], blendValues);
}

float3 BlendNormals(float3 nX, float3 nY, float3 nZ, float3 nNegY, float4 blendValues, float3 worldNormal)
{
    nX = float3(nX.xy + worldNormal.zy, abs(nX.z) * worldNormal.x);
    nY = float3(nY.xy + worldNormal.xz, abs(nY.z) * worldNormal.y);
    nZ = float3(nZ.xy + worldNormal.xy, abs(nZ.z) * worldNormal.z);
    nNegY = float3(nNegY.xy + worldNormal.xz, abs(nNegY.z) * worldNormal.y);

    return normalize(nX.zyx * blendValues.x + nY.xzy * blendValues.y + nZ.xyz * blendValues.z + nNegY.xzy * blendValues.w);
}

float3 BlendNormals(float4x3 normals, float4 blendValues, float3 worldNormal)
{
    return BlendNormals(normals[0], normals[1], normals[2], normals[3], blendValues, worldNormal);
}


float4 ScrambleTexture(Texture2DArray<float4> textures, float textureIndex, float2 uv, float variation, float textureblendFactor, SamplerState samplerState)
{
    float index = variation * 8.0f;
    float i = floor(index);
    float f = frac(index);
    
    float i2 = i + 1.0f;
    float4 offsets = sin(float4(3.0f, 7.0f, 3.0f, 7.0f) * float4(i, i, i2, i2));
    
    float2 duvdx = ddx(uv);
    float2 duvdy = ddy(uv);
    
    //Might not work well for normals
    float4 colorA = lerp(textures.SampleGrad(samplerState, float3(uv + offsets.xy, textureIndex), duvdx, duvdy),
                         textures.SampleGrad(samplerState, float3(uv + offsets.xy, textureIndex + 4), duvdx, duvdy),
                         textureblendFactor);
    
    float4 colorB = lerp(textures.SampleGrad(samplerState, float3(uv + offsets.zw, textureIndex), duvdx, duvdy), 
                         textures.SampleGrad(samplerState, float3(uv + offsets.zw, textureIndex + 4), duvdx, duvdy),
                         textureblendFactor);
    
    //float4 colorA = textures.SampleGrad(samplerState, float3(uv + offsets.xy, textureIndex), duvdx, duvdy);
    //float4 colorB = textures.SampleGrad(samplerState, float3(uv + offsets.zw, textureIndex), duvdx, duvdy);
    float lerpValue = smoothstep(0.2f, 0.8f, f - 0.1f * dot(colorA - colorB, 1.0f));
    return lerp(colorA, colorB, lerpValue);
}


void SampleTextures_float(float3 worldPosition, float3 worldNormal, out float4 color, out float3 normal, out float ambientOcclusion, out float smoothness, out float metallic)
{
    float3x2 uv = GetTriplanarUVs(worldPosition);
    
    float4 blendValues = GetBlendValues(worldNormal, triplanarBlendSharpness);
    
    SamplerState samplerState = SamplerState_Linear_Repeat;
    
    float4 variations;
    variations.x = variationTexture.Sample(samplerState, uv[0] * variationTextureScale).x;
    variations.yw = variationTexture.Sample(samplerState, uv[1] * variationTextureScale).yw;
    variations.z = variationTexture.Sample(samplerState, uv[2] * variationTextureScale).z;
    //variations.w = variationTexture.Sample(samplerState, uv[1] * variationTextureScale).w;
    
    
    float4 textureblendFactors;
    textureblendFactors.x = blendingFactorTexture.Sample(samplerState, uv[0] * blendingFactorScale).x;
    textureblendFactors.yw = blendingFactorTexture.Sample(samplerState, uv[1] * blendingFactorScale).yw;
    textureblendFactors.z = blendingFactorTexture.Sample(samplerState, uv[2] * blendingFactorScale).z;
    //textureblendFactors.w = blendingFactorTexture.Sample(samplerState, uv[1] * blendingFactorScale).w;
    
    float4x4 colors;
    colors[0] = ScrambleTexture(terrainTextures, 0, uv[0], variations.x, textureblendFactors.x, samplerState);
    colors[1] = ScrambleTexture(terrainTextures, 1, uv[1], variations.y, textureblendFactors.y, samplerState);
    colors[2] = ScrambleTexture(terrainTextures, 2, uv[2], variations.z, textureblendFactors.z, samplerState);
    colors[3] = ScrambleTexture(terrainTextures, 3, uv[1], variations.w, textureblendFactors.w, samplerState);
    color = BlendColors(colors, blendValues);
    
    float4x3 normals;
    normals[0] = UnpackNormal(ScrambleTexture(terrainNormals, 0, uv[0], variations.x, textureblendFactors.x, samplerState));
    normals[1] = UnpackNormal(ScrambleTexture(terrainNormals, 1, uv[1], variations.y, textureblendFactors.y, samplerState));
    normals[2] = UnpackNormal(ScrambleTexture(terrainNormals, 2, uv[2], variations.z, textureblendFactors.z, samplerState));
    normals[3] = UnpackNormal(ScrambleTexture(terrainNormals, 3, uv[1], variations.w, textureblendFactors.w, samplerState));
    normal = BlendNormals(normals, blendValues, worldNormal);
    
    float4x4 otherInfos;
    otherInfos[0] = ScrambleTexture(terrainOther, 0, uv[0], variations.x, textureblendFactors.x, samplerState);
    otherInfos[1] = ScrambleTexture(terrainOther, 1, uv[1], variations.y, textureblendFactors.y, samplerState);
    otherInfos[2] = ScrambleTexture(terrainOther, 2, uv[2], variations.z, textureblendFactors.z, samplerState);
    otherInfos[3] = ScrambleTexture(terrainOther, 3, uv[1], variations.w, textureblendFactors.w, samplerState);
    float4 otherInfo = BlendColors(otherInfos, blendValues) * float4(ambientOcclusionStrength, smoothnessStrength, metallicStrength, 1.0f);
    ambientOcclusion = otherInfo.x;
    smoothness = otherInfo.y;
    metallic = otherInfo.z;
}