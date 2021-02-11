// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Water/Caustics"
{
    Properties
    {
		_SurfaceNoise("Surface Noise", 2D) = "white" {} 
		_SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0) 
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777 

		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}	 
		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27 

        _WaterHeight("Water Height", Range(0,20)) = 10
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5  
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200  
        CGPROGRAM
        #define SMOOTHSTEP_AA 0.01
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float2 uv_MainTex; 
            float3 worldPos;
            float3 worldNormal;
        };

        sampler2D _SurfaceNoise;
        float4 _SurfaceNoise_ST; 
 
 		sampler2D _SurfaceDistortion;
		float4 _SurfaceDistortion_ST;
    	
        float _SurfaceNoiseCutoff;
		float _SurfaceDistortionAmount;
        float2 _SurfaceNoiseScroll;
        fixed4 _WaterTint;

        sampler2D _MainTex; 
        
        half _Smoothness;
        half _Specular;
        half _Metallic;
        fixed4 _Color;
        fixed3 caustics;
        float _WaterHeight;

        sampler2D _CameraDepthTexture;
		sampler2D _CameraNormalsTexture;

        UNITY_INSTANCING_BUFFER_START(Props) 
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 localPos = IN.worldPos;
            float2 UV;
            if(abs(IN.worldNormal.x)>0.5) {
                UV = IN.worldPos.yz; // side 
            } else if(abs(IN.worldNormal.z)>0.5) {
                UV = IN.worldPos.xy; // front 
            } else {
                UV = IN.worldPos.xz; // top 
            }
            // Albedo comes from a texture tinted by color
            float3 worlduv = (IN.worldPos / 10);
            fixed4 c = tex2D (_MainTex, worlduv.xy) * _Color;
            o.Albedo = c.rgb;
        
            // Caustics sampling
            fixed2 uv = IN.uv_MainTex * _SurfaceNoise_ST.xy + _SurfaceNoise_ST.zw;

            if (localPos.y < _WaterHeight) {   

                float DistanceFromWater = localPos.y / 50;
                float InvertedDistanceFromWater = -DistanceFromWater;
                float2 distortSample = (tex2D(_SurfaceDistortion, worlduv.xy).xy * 2 - 1) * (_SurfaceDistortionAmount * DistanceFromWater); 
                float2 noiseUV = float2((uv.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x,  (uv.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y); 
                float surfaceNoise = smoothstep(_SurfaceNoiseCutoff - SMOOTHSTEP_AA, _SurfaceNoiseCutoff + SMOOTHSTEP_AA, tex2D(_SurfaceNoise, worlduv.xy).r);  
                float3 tone = tex2D(_SurfaceNoise, noiseUV).rgb; 
                tone * surfaceNoise;
                o.Albedo += (tone * InvertedDistanceFromWater);   
            } else {
                caustics = tex2D(_MainTex, uv).rgb;
            }
             
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = c.a;
        }
        ENDCG
        Pass {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o; 
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
