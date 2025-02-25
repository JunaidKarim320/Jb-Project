Shader "QuantumTheory/Mobile/Architecture" {
	Properties {
		_MainTex ("Diffuse", 2D) = "white" {}
		_Reflection ("CubeMap Reflection", Cube) = "black" {}
		_ReflectionMask ("Masks - Cube(R), Spec(G), Gloss(B)", 2D) = "black" {}
		_ReflectionTint ("Reflection Tint", Vector) = (1,1,1,1)
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