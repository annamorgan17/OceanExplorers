#ifndef TERRAIN_INCLUDED
#define TERRAIN_INCLUDED

struct Input {
    float3 worldNormal;
    float3 worldPos;
};
        
sampler2D _MainTex;

const static int maxLayerCount = 8;
const static float epsilon = 1E-4;
float minHeight; float maxHeight;

int layerCount;
float3 baseColours[maxLayerCount];
float baseStartHeights[maxLayerCount];
float baseBlends[maxLayerCount];
float baseColourStrength[maxLayerCount];
float baseTextureScale[maxLayerCount];
        

sampler2D _SurfaceNoise;
float4 _SurfaceNoise_ST; 
 
sampler2D _SurfaceDistortion;
float4 _SurfaceDistortion_ST;
    	
float _SurfaceNoiseCutoff;
float _SurfaceDistortionAmount;
float2 _SurfaceNoiseScroll;
fixed4 _WaterTint;

half _Smoothness; 
half _Metallic;

float _WaterHeight;
float _Scale;

UNITY_DECLARE_TEX2DARRAY(baseTextures);
UNITY_INSTANCING_BUFFER_START(Props) 
UNITY_INSTANCING_BUFFER_END(Props)

float inverseLerp(float a, float b, float value) {
	return saturate((value-a)/(b-a));
}
float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
	float3 scaledWorldPos = worldPos / scale;
	float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
	float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
	float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
	return xProjection + yProjection + zProjection;
}

float3 samplerTexture(sampler2D tex, float3 worldPos, float2 offset, float scale, float3 worldNormal) {
    float3 scaledWorldPos = worldPos / scale;  
    return tex2D(tex, float2(scaledWorldPos.x, scaledWorldPos.z) + offset) * worldNormal.y;
}

void surf (Input IN, inout SurfaceOutputStandard o) {
	float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
	float3 blendAxes = abs(IN.worldNormal);
	blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

	for (int i = 0; i < layerCount; i ++) {
		float drawStrength = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);

		float3 baseColour = baseColours[i] * baseColourStrength[i];
		float3 textureColour = triplanar(IN.worldPos, baseTextureScale[i], blendAxes, i) * (1-baseColourStrength[i]);

		o.Albedo = o.Albedo * (1-drawStrength) + (baseColour+textureColour) * drawStrength;

        if (IN.worldPos.y < _WaterHeight) {   
        //calculate how far we are away from water
        float DistanceFromWater = IN.worldPos.y / 50; 
  
        //get the cords to sample with time
        float2 scrollingUV = float2((_Time.y * _SurfaceNoiseScroll.x) , (_Time.y * _SurfaceNoiseScroll.y)); 
        float2 distortUV = TRANSFORM_TEX(scrollingUV, _SurfaceDistortion); 
        //get the surface noise onto a triplane
        float3 caustics = samplerTexture(_SurfaceNoise, IN.worldPos, scrollingUV, _Scale, abs(IN.worldNormal)); 

        float2 c = UNITY_PROJ_COORD(distortUV).xy * 2 - 1;
        float2 distortSample = samplerTexture(_SurfaceDistortion, IN.worldPos, c , _Scale, abs(IN.worldNormal)) * _SurfaceDistortionAmount; 

        float3 causticsDistortion = samplerTexture(_SurfaceDistortion, IN.worldPos, scrollingUV + distortSample, 100, abs(IN.worldNormal).r);

        //add in the caustics with a transition into the water
        o.Albedo += (caustics);  
        }  
	}
    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
}
#endif