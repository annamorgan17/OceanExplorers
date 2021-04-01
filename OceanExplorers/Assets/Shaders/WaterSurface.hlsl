#ifndef WATERSURFACE_INCLUDED
#define WATERSURFACE_INCLUDED

//include any helper functions 

//define structures
struct Input {
	float2 reflection;
};

//attribute data
struct appdata {
    float4 vertex : POSITION;   //Vertex Position
	float4 uv : TEXCOORD0;      //UV
	float3 normal : NORMAL;     //Normal
};

//vertex to fragment (vertex output data)
struct v2f {
    float4 vertex : SV_POSITION;	//position in clip space
	float2 noiseUV : TEXCOORD0;     //UVs of noise
	float2 distortUV : TEXCOORD1;   //UVs of distortion
	float4 screenPosition : TEXCOORD2;  //Position on screen
	float3 viewNormal : NORMAL;     //View normal Position
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

/*
TEXTURE2D(_CameraDepthTexture); 
SAMPLER(sampler_CameraDepthTexture); 
float4 _CameraDepthTexture_ST;
*/

TEXTURE2D(_CameraNormalsTexture); SAMPLER(sampler_CameraNormalsTexture); float4 _CameraNormalsTexture_ST;

float4 alphaBlend(float4 top, float4 bottom) {
	float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
	float alpha = top.a + bottom.a * (1 - top.a);

	return float4(color, alpha);
}

v2f vert (appdata v) {
    v2f o;

    //o.vertex = UnityObjectToClipPos(v.vertex);
	o.screenPosition = ComputeScreenPos(o.vertex);
	o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion); 
	o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
	o.viewNormal = COMPUTE_VIEW_NORMAL;

    return o;
}


float4 frag (v2f i) : SV_Target {  

	#pragma region Refraction
    /*
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
        float Rdepth = LOAD_TEXTURE2D_LOD(_CameraDepthTexture, screenPos, 0).r;
		float depth = LinearEyeDepth(Rdepth);  
		float depthDifference = depth - i.screenPosition.w; 
		//get the deep areas in white, with the rest in normal
		float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);
		//create a colour based on the depth
		float4 waterColor = lerp( lerp(_DepthGradientShallow, refraction, 1 -_RefractionShallowIntensity), lerp(_DepthGradientDeep, refraction, 1 - _RefractionDeepIntensity), waterDepthDifference); 
		waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference); 
        float3 existingNormal = tex2Dproj(_CameraNormalsTexture, i.screenPosition); 
		float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, saturate(dot(existingNormal, i.viewNormal)));
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