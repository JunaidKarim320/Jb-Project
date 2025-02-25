Shader "GRAY/Car Shader" {
	Properties {
		[HideInInspector] __dirty ("", Float) = 1
		_MainTex ("Main Tex", 2D) = "white" {}
		_Color ("Color", Vector) = (0,0,0,0)
		_DecalRight ("Decal Right", 2D) = "black" {}
		_DecalLeft ("Decal Left", 2D) = "black" {}
		_DecalFront ("Decal Front", 2D) = "black" {}
		_Metalic ("Metalic", Range(0, 1)) = 0.1362112
		_OuterColor ("Outer Color", Vector) = (0,0,0,0)
		_InnerColor ("Inner Color", Vector) = (0,0,0,0)
		_OuterGloss ("Outer Gloss", Range(0, 1)) = 0
		_InnerGloss ("Inner Gloss", Range(0, 1)) = 0
		_Highlight ("Highlight", Range(0, 1)) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Mobile/Diffuse"
}