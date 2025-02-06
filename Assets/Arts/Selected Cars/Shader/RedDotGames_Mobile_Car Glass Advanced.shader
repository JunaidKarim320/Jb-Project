Shader "RedDotGames/Mobile/Car Glass Advanced" {
	Properties {
		_Color ("Main Color (RGB)", Vector) = (1,1,1,1)
		_ReflectColor ("Reflection Color (RGB)", Vector) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Mask (A)", 2D) = "black" {}
		_Cube ("Reflection Cubemap (CUBE)", Cube) = "_Skybox" {}
		_FresnelPower ("Fresnel Power", Range(0.05, 5)) = 0.75
		_TintColor ("Tint Color (RGB)", Vector) = (1,1,1,1)
		_AlphaPower ("Alpha", Range(0, 2)) = 1
		_OpaqueReflection ("Opaque Reflection", Range(0, 1)) = 0
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
	Fallback "Reflective/VertexLit"
}