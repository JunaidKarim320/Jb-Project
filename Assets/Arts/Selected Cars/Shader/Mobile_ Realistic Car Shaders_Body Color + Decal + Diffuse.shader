Shader "Mobile/ Realistic Car Shaders/Body Color + Decal + Diffuse" {
	Properties {
		_Color ("Vehicle Color", Vector) = (1,1,1,1)
		_MainTex ("Diffuse", 2D) = "white" {}
		_DiffuseUVScale ("Diffuse UV Scale", Range(1, 100)) = 1
		_DiffuseBumpMap ("Diffuse Bumpmap", 2D) = "bump" {}
		_RenderedTexture ("Rendered Texture", 2D) = "white" {}
		_DecalColor ("Decal Color", Vector) = (1,1,1,1)
		_Decal ("Decal", 2D) = "white" {}
		_DecalTransparency ("Decal Transparency", Range(0.1, 1)) = 1
		_DecalReflection ("Decal Reflection", Range(0, 1)) = 0.5
		_DecalUVScale ("Decal UV Scale", Range(1, 50)) = 1
		_Cube ("Reflection Cubemap", Cube) = "white" {}
		_RefIntensity ("Reflection Intensity", Range(0, 2)) = 0
		_RefVisibility ("Reflection Visibility Scale", Range(0.1, 2)) = 0.1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Standard"
	//CustomEditor "SkrilStudio.VehicleDecal_Editor"
}