Shader "Beffio/Car Paint Opaque (Simplified)" {
	Properties {
		_DiffuseColor ("Paint Color", Vector) = (1,1,1,1)
		_DiffuseMapColorAndAlpha ("Texture Color", Vector) = (1,1,1,1)
		_DiffuseMap ("Diffuse Map", 2D) = "white" {}
		_ReflectionColor ("Reflection Color", Vector) = (1,1,1,1)
		_ReflectionMap ("Reflection Map", Cube) = "" {}
		_ReflectionStrength ("Reflection Strength", Range(0, 1)) = 0.5
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