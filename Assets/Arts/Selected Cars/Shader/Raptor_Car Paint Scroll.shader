Shader "Raptor/Car Paint Scroll" {
	Properties {
		_DetailColor ("Detail Color", Vector) = (1,1,1,1)
		_DiffuseColor ("Paint Color", Vector) = (1,1,1,1)
		_DiffuseMapColorAndAlpha ("Texture Color", Vector) = (1,1,1,1)
		_DiffuseMap ("Diffuse Map", 2D) = "white" {}
		_MatCapLookup ("MatCap Lookup", 2D) = "white" {}
		_ReflectionColor ("Reflection Color", Vector) = (1,1,1,1)
		_ReflectionMap ("Reflection Map", Cube) = "" {}
		_ReflectionStrength ("Reflection Strength", Range(0, 1)) = 0.5
		_EnvSide ("Env Side Texture", 2D) = "black" {}
		_EnvTop ("Env Top Texture", 2D) = "black" {}
		_EnvTimer ("Env Timer", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}