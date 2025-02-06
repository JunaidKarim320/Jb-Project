//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

Shader "Mobile/ Realistic Car Shaders/Glass Simple"
{
	Properties
	{
		// glass color
		_Color("Glass Color", Color) = (1, 1, 1, 1)

		// glass transparency
		_Transprnt("Glass Transparency", Range(0.05, 0.9)) = 0.5

		// reflection
	    _Cube("Reflection Cubemap", Cube) = "white" {}
	    _RefIntensity("Reflection Intensity", Range(0, 2)) = 0
		_RenderedTexture("Rendered Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		LOD 200
		CGPROGRAM
		#pragma surface surf BlinnPhong alpha
		#pragma target 2.0	
        #pragma multi_compile Rendered_Texture Cubemap_T Cubemap_Assigned Both_T Off_T
		struct Input
		{
			float3 worldRefl;
			INTERNAL_DATA
		};
		// transparency
		struct v2f
		{
			float2 uv : TEXCOORD1;
		};
		v2f vert(inout appdata_full v)
		{
			v2f o;
			return o;
		}
		samplerCUBE _Cube;
		float4 _Color;
		float _RefIntensity;
		sampler2D _RenderedTexture;
		float _Transprnt;
		//uniform float4x4 _Rotation;
		void surf(Input IN, inout SurfaceOutput s)
		{
			s.Normal = normalize(float3(0, 0, 1));

			// reflection
			float3 worldVec = WorldReflectionVector(IN, s.Normal);
			float4 cubemapTexture = texCUBE(_Cube, worldVec/*mul(_Rotation, worldVec)*/); // user set cubemap
			float4 cubemapTexture2 = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldVec); // unity assigned cubemap
			float4 _RenderedTxt = tex2D(_RenderedTexture, worldVec);
			// transparency
			float4 opacit = _Color.a * _Transprnt;
			// both
            #if Both_T
			float4 reflectionResult = _RefIntensity * cubemapTexture * (_RenderedTxt - (-1 * cubemapTexture.a));
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
			s.Alpha = opacit + reflectionResult;
            #endif
			// remdered texture only
            #if Rendered_Texture
			float4 reflectionResult = _RefIntensity * _RenderedTxt;
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
			s.Alpha = opacit + reflectionResult;
            #endif
			// cubemap only
            #if Cubemap_T
			float4 reflectionResult = _RefIntensity * cubemapTexture;
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
			s.Alpha = opacit + reflectionResult;
            #endif
			// assigned cubemap only
            #if Cubemap_Assigned
			float4 reflectionResult = _RefIntensity * cubemapTexture2;
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
			s.Alpha = opacit + reflectionResult;
            #endif
			// reflection is off
            #if Off_T
			s.Alpha = opacit;
            #endif	

			s.Albedo = _Color;
			
		}
		ENDCG
	}
	FallBack "Standard"
	CustomEditor "VehicleGlassSimple_Editor"
}
