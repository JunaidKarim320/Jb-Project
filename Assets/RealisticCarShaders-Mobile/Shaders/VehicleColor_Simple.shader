//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

Shader "Mobile/ Realistic Car Shaders/Body Color Simple"
{
	Properties
	{
		// vehicle color
		_Color("Vehicle Color", Color) = (1, 1, 1, 1)

	    // reflection
	    _Cube("Reflection Cubemap", Cube) = "white" {}
	    _RefIntensity("Reflection Intensity", Range(0, 2)) = 0
		_RenderedTexture("Rendered Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 2.0
        #pragma multi_compile Rendered_Texture Cubemap_T Cubemap_Assigned Both_T Off_T
		struct Input
		{
			float3 worldRefl;
			INTERNAL_DATA
		};
		samplerCUBE _Cube;
		float4 _Color;
		sampler2D _RenderedTexture;
		float _RefIntensity;
		//uniform float4x4 _Rotation;
		void surf(Input IN, inout SurfaceOutput s)
		{
			s.Normal = normalize(float3(0, 0, 1));
			float3 worldVec = WorldReflectionVector(IN, s.Normal);
			float4 _RenderedTxt = tex2D(_RenderedTexture, worldVec);
			// reflection
			float4 cubemapTexture = texCUBE(_Cube, worldVec/*mul(_Rotation, worldVec)*/); // user set cubemap
			float4 cubemapTexture2 = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldVec); // unity assigned cubemap
			// both
            #if Both_T
			float4 reflectionResult = _RefIntensity * cubemapTexture * (_RenderedTxt - (-1 * cubemapTexture.a));
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
            #endif
			// remdered texture only
            #if Rendered_Texture
			float4 reflectionResult = _RefIntensity * _RenderedTxt;
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
            #endif
			// cubemap only
            #if Cubemap_T
			float4 reflectionResult = _RefIntensity * cubemapTexture;
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
            #endif
			// assigned cubemap only
            #if Cubemap_Assigned
			float4 reflectionResult = _RefIntensity * cubemapTexture2;
			s.Emission = reflectionResult * reflectionResult + reflectionResult * reflectionResult;
            #endif

			// combine everything
			s.Albedo = _Color;
		}
		ENDCG
	}
	FallBack "Standard"
	CustomEditor "VehicleSimple_Editor"
}
