Shader "Water/WaterSurface" {
    Properties { 
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725) 
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749) 

		_DepthMaxDistance("Depth Maximum Distance", Float) = 1 

		_FoamColor("Foam Color", Color) = (1,1,1,1) 

		_SurfaceNoise("Surface Noise", 2D) = "white" {} 
		_SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0) 
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777 

		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}	 
		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27 

		_FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
		_FoamMinDistance("Foam Minimum Distance", Float) = 0.04		

		_RefractionShallowIntensity("Refraction Shallow", Range(0, 1)) = 0.2
		_RefractionDeepIntensity("Refraction Deep", Range(0, 1)) = 0.8
    }
    SubShader {
		Tags {
			"RenderType"="Transparent"
			"Queue" = "Transparent"
		}
        Cull off
		GrabPass{

		}
        Pass {
			// Transparent "normal" blending.
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

            CGPROGRAM 
			#define SMOOTHSTEP_AA 0.01

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" 

			float4 alphaBlend(float4 top, float4 bottom) {
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

            struct appdata {
                float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : SV_POSITION;	
				float2 noiseUV : TEXCOORD0;
				float2 distortUV : TEXCOORD1;
				float4 screenPosition : TEXCOORD2;
				float3 viewNormal : NORMAL;
            };
			sampler2D _GrabTexture;

            sampler2D _CausticTex;
            float4 _CausticTile;

			sampler2D _SurfaceNoise;
			float4 _SurfaceNoise_ST;
			float4 _SurfaceNoise_TexelSize;

			sampler2D _SurfaceDistortion;
			float4 _SurfaceDistortion_ST;
			float4 _SurfaceDistortion_TexelSize;

            v2f vert (appdata v) {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex);
				o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion); 
				o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
				o.viewNormal = COMPUTE_VIEW_NORMAL;

                return o;
            }

			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float4 _FoamColor;

			float _DepthMaxDistance;
			float _FoamMaxDistance;
			float _FoamMinDistance;
			float _SurfaceNoiseCutoff;
			float _SurfaceDistortionAmount;

		    float _RefractionShallowIntensity;
			float _RefractionDeepIntensity;

			float2 _SurfaceNoiseScroll;

			sampler2D _CameraDepthTexture;
			float4 _CameraDepthTexture_TexelSize;

			sampler2D _CameraNormalsTexture;
 
            float4 frag (v2f i) : SV_Target {  
				float4 uv = i.screenPosition;
				
				float a = (tex2D(_SurfaceDistortion, UNITY_PROJ_COORD(i.distortUV)).xy * 2 - 1); 
				

				float refractionSpeed = 100;
				float off = cos((_Time.x * refractionSpeed) + i.vertex.y * 32.0) ;

				float4 offset = float4( sin((_Time.x * refractionSpeed) + i.screenPosition.y * 32.0) * 0.1, 0, 0, 0);
				offset = float4(off, off, off, off);
				float4 refraction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(uv + offset));

				//get the linear depth
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.screenPosition).r);  

				float depthDifference = depth - i.screenPosition.w; 

				//get the deep areas in white, with the rest in normal
				float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);

				//create a colour based on the depth
				float4 waterColor = lerp(
				lerp(_DepthGradientShallow, refraction, 1 -_RefractionShallowIntensity), 
				lerp(_DepthGradientDeep, refraction, 1 - _RefractionDeepIntensity), waterDepthDifference); 


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
				
				float d = lerp(_SurfaceDistortionAmount * 7, distortedWater, foamDepthDifference);
				float4 surfaceNoiseColor = _FoamColor + (_FoamColor * d); 
				surfaceNoiseColor.a *= surfaceNoise;  


				half4 Colour = alphaBlend(surfaceNoiseColor, waterColor);
				return Colour;
            }
            ENDCG
        }
    }
}
 
