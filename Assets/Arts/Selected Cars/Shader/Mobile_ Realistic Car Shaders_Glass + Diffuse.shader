Shader "Mobile/ Realistic Car Shaders/Glass + Diffuse" {
	Properties {
		_Color ("Glass Color", Vector) = (1,1,1,1)
		_Transprnt ("Glass Transparency", Range(0.05, 0.9)) = 0.5
		_MainTex ("Diffuse", 2D) = "white" {}
		_DiffuseUVScale ("Diffuse UV Scale", Range(1, 100)) = 1
		_DiffuseBumpMap ("Diffuse Bumpmap", 2D) = "bump" {}
		_Cube ("Reflection Cubemap", Cube) = "white" {}
		_RefIntensity ("Reflection Intensity", Range(0, 2)) = 0
		_RenderedTexture ("Rendered Texture", 2D) = "white" {}
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
	//CustomEditor "SkrilStudio.VehicleGlass_Editor"
}