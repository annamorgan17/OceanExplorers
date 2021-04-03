#ifndef WATERSURFACE_INCLUDED
#define WATERSURFACE_INCLUDED

//include any helper functions  
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Shadow/ShadowSamplingTent.hlsl"

CBUFFER_START(UnityPerFrame)
	float4x4 unity_MatrixVP;
CBUFFER_END

CBUFFER_START(UnityPerDraw)
	float4x4 unity_ObjectToWorld;
	float4 unity_LightIndicesOffsetAndCount;
	float4 unity_4LightIndices0, unity_4LightIndices1;
CBUFFER_END

#define MAX_VISIBLE_LIGHTS 16

CBUFFER_START(_LightBuffer)
	float4 _VisibleLightColors[MAX_VISIBLE_LIGHTS];
	float4 _VisibleLightDirectionsOrPositions[MAX_VISIBLE_LIGHTS];
	float4 _VisibleLightAttenuations[MAX_VISIBLE_LIGHTS];
	float4 _VisibleLightSpotDirections[MAX_VISIBLE_LIGHTS];
CBUFFER_END

CBUFFER_START(_ShadowBuffer)
	float4x4 _WorldToShadowMatrices[MAX_VISIBLE_LIGHTS];
	float4 _ShadowData[MAX_VISIBLE_LIGHTS];
	float4 _ShadowMapSize;
CBUFFER_END
  
TEXTURE2D_SHADOW(_ShadowMap);
SAMPLER_CMP(sampler_ShadowMap);





//attribute data
struct appdata {
    float4 pos : POSITION;
	float3 normal : NORMAL;
	UNITY_VERTEX_INPUT_INSTANCE_ID

	float4 uv : TEXCOORD0;      //UV
	
};

//vertex to fragment (vertex output data)
struct v2f {
	float4 clipPos : SV_POSITION;
	float3 normal : TEXCOORD0;
	float3 worldPos : TEXCOORD1; 
	UNITY_VERTEX_INPUT_INSTANCE_ID
};







TEXTURE2D(_GrabTexture);
TEXTURE2D(_ReflectionTex);
TEXTURE2D(_CausticTex); SAMPLER(sampler_CausticTex); float4 _CausticTile_ST;
TEXTURE2D(_SurfaceNoise); SAMPLER(sampler_SurfaceNoise); float4 _SurfaceNoise_ST;
TEXTURE2D(_SurfaceDistortion); SAMPLER(sampler_SurfaceDistortion); float4 _SurfaceDistortion_ST; 
float4 _DepthGradientShallow; float4 _DepthGradientDeep; float _DepthMaxDistance;
float4 _FoamColor; float _FoamMaxDistance; float _FoamMinDistance;
float _SurfaceNoiseCutoff; float _SurfaceDistortionAmount;
float _RefractionShallowIntensity; float _RefractionDeepIntensity;
float2 _SurfaceNoiseScroll;
/* TEXTURE2D(_CameraDepthTexture);  SAMPLER(sampler_CameraDepthTexture);  float4 _CameraDepthTexture_ST; */
TEXTURE2D(_CameraNormalsTexture); SAMPLER(sampler_CameraNormalsTexture); float4 _CameraNormalsTexture_ST;
 
float4 alphaBlend(float4 top, float4 bottom) {
	float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
	float alpha = top.a + bottom.a * (1 - top.a);
	return float4(color, alpha);
}

v2f vert (appdata input) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	float4 worldPos = mul(UNITY_MATRIX_M, float4(input.pos.xyz, 1.0));
	o.clipPos = mul(unity_MatrixVP, worldPos);
	o.normal = mul((float3x3)UNITY_MATRIX_M, input.normal);
	o.worldPos = worldPos.xyz; 
	o.screenPosition = ComputeScreenPos (o.vertex);  
    return o;
}

float4 frag (v2f i) : SV_Target {  

	#pragma region Refraction
    /*
	make clip position

	float4(input.pos.xy, 0.0, 1.0);


		//object space to homogeneous clip space.
		//float4 clipPos = UnityObjectToClipPos(i.vertex);
		//From homogeneous clip space to normalized device space.
		clipPos.xy = (clipPos.xy / clipPos.w); 
		//Invert Y for reflection
        clipPos.y *= -1; 
		//To uv coordinates. 
		clipPos.xy = 0.5 * clipPos.xy + 0.5;
		float4 colll = tex2D(_ReflectionTex, clipPos.xy).rgba;
		float4 uv = i.screenPosition;
		float refractionSpeed = 100;
		float off = cos((_Time.x * refractionSpeed) + i.vertex.y * 32.0) ;
		float4 offset = float4( sin((_Time.x * refractionSpeed) + i.screenPosition.y * 32.0) * 0.1, 0, 0, 0);
		offset = float4(off, off, off, off);
		float4 refraction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(uv + offset));
        */
	#pragma endregion

	#pragma region Water
		//get the linear depth
        float Rdepth = LOAD_TEXTURE2D_LOD(_CameraDepthTexture, i.screenPosition,0).r;
		float depth = LinearEyeDepth(Rdepth);   
		float depthDifference = depth - i.screenPosition.w; 
		//get the deep areas in white, with the rest in normal
		float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);
		//create a colour based on the depth
		float4 waterColor = lerp( lerp(_DepthGradientShallow, refraction, 1 -_RefractionShallowIntensity), lerp(_DepthGradientDeep, refraction, 1 - _RefractionDeepIntensity), waterDepthDifference); 
		waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference); 
        float3 existingNormal = tex2Dproj(_CameraNormalsTexture, i.screenPosition); 
		float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, saturate(dot(existingNormal, i.vertex))); //was i.viewNormal
		//set the very edge of the water as normal, with the mid while
		float foamDepthDifference = saturate(depthDifference / foamDistance);
		float surfaceNoiseCutoff = foamDepthDifference * _SurfaceNoiseCutoff; 
		float distortedWater = lerp(_SurfaceDistortionAmount * 5, _SurfaceDistortionAmount, waterDepthDifference);
		float distortedFoam = lerp(_SurfaceDistortionAmount * 7, distortedWater, waterDepthDifference);
		float2 distortSample = (tex2D(_SurfaceDistortion, UNITY_PROJ_COORD(i.distortUV)).xy * 2 - 1) * distortedFoam; 
		float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
        float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, tex2D(_SurfaceNoise, noiseUV).r);  
        float4 surfaceNoiseColor = _FoamColor + (_FoamColor * lerp(_SurfaceDistortionAmount * 7, distortedWater, foamDepthDifference)); 
		surfaceNoiseColor.a *= surfaceNoise;   
	#pragma endregion

	half4 Colour = alphaBlend(surfaceNoiseColor, waterColor );
	return Colour;
}

#endif