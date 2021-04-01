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

		_ReflectionTex("Reflection Texture", 2D) = "white" {}
    }
	HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

    ENDHLSL

    SubShader {
		Tags {
			"RenderPipeline" = "HDRenderPipeline"
			"RenderType"="Transparent"
			"Queue" = "Transparent"
		}
        Cull off
		GrabPass{ }

        Pass {
			Name "ForwardLit"
			Tags {"LightMode" = "UniversalForward"}

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			//start our pass
            HLSLPROGRAM 
			#pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag 
 
			//call the fragment and vertex inside of this file
            #include "WaterSurface.hlsl" 

			//end the pass
            ENDHLSL
        }
    }
}
 
