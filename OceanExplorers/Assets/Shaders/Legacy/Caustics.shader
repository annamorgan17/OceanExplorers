

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
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5  
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _VertexColour("Colour by vertexs", Range(0,1)) = 0
    }
    SubShader
    {
        
        Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }
        LOD 200  
        CGPROGRAM
        #define SMOOTHSTEP_AA 0.01
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float2 uv_MainTex; 
            float2 uv_BumpMap;
            float3 worldPos;
            float3 worldNormal; INTERNAL_DATA
            float4 vertColor;
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
        sampler2D _BumpMap;

        half _Smoothness;
        half _Specular;
        half _Metallic;
        fixed4 _Color;
        fixed3 caustics;
        float _WaterHeight;

        int _VertexColour;

        sampler2D _CameraDepthTexture;
		sampler2D _CameraNormalsTexture;

        UNITY_INSTANCING_BUFFER_START(Props) 
        UNITY_INSTANCING_BUFFER_END(Props)
        void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertColor = v.color;
		}

        //get the texture on each axis, blend it by the rotation and combine together
        //triplanar projection
        float3 triplanar(sampler2D tex, float3 worldPos, float2 offset, float scale, float3 blendAxes) {
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = tex2D(tex, float2(scaledWorldPos.y, scaledWorldPos.z) + offset) * blendAxes.x;
			float3 yProjection = tex2D(tex, float2(scaledWorldPos.x, scaledWorldPos.z) + offset) * blendAxes.y;
			float3 zProjection = tex2D(tex, float2(scaledWorldPos.x, scaledWorldPos.y) + offset) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		} 
        //kinda triplanar still? y projection blended with clear. Provides a fade off
        float3 samplerTexture(sampler2D tex, float3 worldPos, float2 offset, float scale, float3 worldNormal) {
            float3 scaledWorldPos = worldPos / scale;  
            return tex2D(tex, float2(scaledWorldPos.x, scaledWorldPos.z) + offset) * worldNormal.y;
        }
        void surf (Input IN, inout SurfaceOutputStandard o) {  
            
            //switch to get main texture from vertex colours if specified
            if (_VertexColour == 1){
                o.Albedo = IN.vertColor.rgb;
            }else{ 
                //Apply surface texture and colour tint
                o.Albedo = triplanar(_MainTex, IN.worldPos, float2(0,0), 200, abs(IN.worldNormal)) * _Color; 
            }
        
            // Caustics sampling
            fixed2 uv = IN.uv_MainTex * _SurfaceNoise_ST.xy + _SurfaceNoise_ST.zw;

            if (IN.worldPos.y < _WaterHeight) {   
                //calculate how far we are away from water
                float DistanceFromWater = IN.worldPos.y / 50; 
  
                //get the cords to sample with time
                float2 scrollingUV = float2((_Time.y * _SurfaceNoiseScroll.x) , (_Time.y * _SurfaceNoiseScroll.y)); 
                float2 distortUV = TRANSFORM_TEX(scrollingUV, _SurfaceDistortion); 
                //get the surface noise onto a triplane
                float3 caustics = samplerTexture(_SurfaceNoise, IN.worldPos, scrollingUV, 100, abs(IN.worldNormal)); 

                float2 c = UNITY_PROJ_COORD(distortUV).xy * 2 - 1;
                float2 distortSample = samplerTexture(_SurfaceDistortion, IN.worldPos, c , 100, abs(IN.worldNormal)) * _SurfaceDistortionAmount; 

                float3 causticsDistortion = samplerTexture(_SurfaceDistortion, IN.worldPos, scrollingUV + distortSample, 100, abs(IN.worldNormal).r);

                //add in the caustics with a transition into the water
                o.Albedo += (caustics);  
            } 
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;  
            //o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Alpha = 1;
        }
        ENDCG
        
        GrabPass {"_MainTex2"}
        //normal pass (although does mess with albedo currently)
        //https://bgolus.medium.com/normal-mapping-for-a-triplanar-shader-10bf39dca05a#668e
        //https://github.com/bgolus/Normal-Mapping-for-a-Triplanar-Shader/blob/master/TriplanarSwizzle.shader
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM  
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // flip UVs horizontally to correct for back side projection
            #define TRIPLANAR_CORRECT_PROJECTED_U

            // offset UVs to prevent obvious mirroring
            // #define TRIPLANAR_UV_OFFSET

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
            };

            uniform sampler2D _MainTex2;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BumpMap;
            
            fixed4 _LightColor0;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // calculate triplanar blend
                half3 triblend = saturate(pow(i.worldNormal, 4));
                triblend /= max(dot(triblend, half3(1,1,1)), 0.0001);

                // preview blend
                // return fixed4(triblend.xyz, 1);

                // calculate triplanar uvs
                // applying texture scale and offset values ala TRANSFORM_TEX macro
                float scale = 0.05;
                float4 TextureScale = float4(scale,scale,scale,scale);
                float2 uvX = i.worldPos.zy * TextureScale.xy + TextureScale.zw;
                float2 uvY = i.worldPos.xz * TextureScale.xy + TextureScale.zw;
                float2 uvZ = i.worldPos.xy * TextureScale.xy + TextureScale.zw;

                // offset UVs to prevent obvious mirroring
            #if defined(TRIPLANAR_UV_OFFSET)
                uvY += 0.33;
                uvZ += 0.67;
            #endif

                // minor optimization of sign(). prevents return value of 0
                half3 axisSign = i.worldNormal < 0 ? -1 : 1;

                // flip UVs horizontally to correct for back side projection
            #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
                uvX.x *= axisSign.x;
                uvY.x *= axisSign.y;
                uvZ.x *= -axisSign.z;
            #endif

                // albedo textures
                fixed4 colX = tex2D(_MainTex2, uvX);
                fixed4 colY = tex2D(_MainTex2, uvY);
                fixed4 colZ = tex2D(_MainTex2, uvZ);
                fixed4 col = colX * triblend.x + colY * triblend.y + colZ * triblend.z;

                // tangent space normal maps
                half3 tnormalX = UnpackNormal(tex2D(_BumpMap, uvX));
                half3 tnormalY = UnpackNormal(tex2D(_BumpMap, uvY));
                half3 tnormalZ = UnpackNormal(tex2D(_BumpMap, uvZ));

                // flip normal maps' x axis to account for flipped UVs
            #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
                tnormalX.x *= axisSign.x;
                tnormalY.x *= axisSign.y;
                tnormalZ.x *= -axisSign.z;
            #endif

                // flip normal maps' z axis to account for world surface normal facing
                tnormalX.z *= axisSign.x;
                tnormalY.z *= axisSign.y;
                tnormalZ.z *= axisSign.z;

                // swizzle tangent normals to match world orientation and blend together
                half3 worldNormal = normalize(
                    tnormalX.zyx * triblend.x +
                    tnormalY.xzy * triblend.y +
                    tnormalZ.xyz * triblend.z
                    );

                // preview world normals
                // return fixed4(worldNormal * 0.5 + 0.5, 1);

                // calculate lighting
                half ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 ambient = ShadeSH9(half4(worldNormal, 1));
                half3 lighting = _LightColor0.rgb * ndotl + ambient;

                // preview directional lighting
                // return fixed4(ndotl.xxx, 1);

                return fixed4(lighting, 0.5);
            }
            ENDCG
        }
        
        //dddddddddd
    }
}





