Shader "QuantumTheory/Mobile/Self-Illumin/DiffuseEmissive" {
	Properties {
		_MainTex ("Diffuse (rgb) Specular Mask(a)", 2D) = "white" {}
		_Illum ("Emission Mask (rgba)", 2D) = "white" {}
		_EmissionLM ("Emission Strength", Float) = 1
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
	Fallback "Mobile/VertexLit"
}