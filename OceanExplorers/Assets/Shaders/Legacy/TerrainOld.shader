Shader "Terrain/MeshShader" {
    Properties { 
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM 
        #pragma surface surf Standard fullforwardshadows 
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float3 worldNormal;
            float3 worldPos;
        };
        const static int maxLayerCount = 8;
        const static float epsilon = 1E-4;
        float minHeight;
        float maxHeight;

        int layerCount;
        float3 baseColours[maxLayerCount];
        float baseStartHeights[maxLayerCount];
        float baseBlends[maxLayerCount];
        float baseColourStrength[maxLayerCount];
        float baseTextureScale[maxLayerCount];
        
        UNITY_DECLARE_TEX2DARRAY(baseTextures);
        UNITY_INSTANCING_BUFFER_START(Props) 
        UNITY_INSTANCING_BUFFER_END(Props)

        float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}
//
        float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
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
			}

		
		}
        ENDCG
    }
    FallBack "Diffuse"
}
