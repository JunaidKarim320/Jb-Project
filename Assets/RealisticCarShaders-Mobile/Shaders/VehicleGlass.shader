//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

Shader "Mobile/ Realistic Car Shaders/Glass + Diffuse"
{
	Properties
	{
		// glass color
		_Color("Glass Color", Color) = (1, 1, 1, 1)

		// glass transparency
		_Transprnt("Glass Transparency", Range(0.05, 0.9)) = 0.5

		// glass texture for example: heater, decal, damage
		_MainTex("Diffuse", 2D) = "white" {}
	    _DiffuseUVScale("Diffuse UV Scale", Range(1, 100)) = 1
		_DiffuseBumpMap("Diffuse Bumpmap", 2D) = "bump" {}

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
        #pragma multi_compile Bumped_Diffuse_Off Bumped_Diffuse
		struct Input
		{
			float2 uv_MainTex;
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
		sampler2D _MainTex;
		sampler2D _DiffuseBumpMap;
		sampler2D _RenderedTexture;
		samplerCUBE _Cube;
		float4 _Color;
		float _DiffuseUVScale;
		float _Transprnt;
		float _RefIntensity;
		//uniform float4x4 _Rotation;
		void surf(Input IN, inout SurfaceOutput s)
		{
            #if Bumped_Diffuse
			s.Normal = normalize(float3(0, 0, 1) + UnpackNormal(tex2D(_DiffuseBumpMap, IN.uv_MainTex*_DiffuseUVScale)));
            #else
			s.Normal = normalize(float3(0, 0, 1));
            #endif

			// texture
			float3 worldVec = WorldReflectionVector(IN, s.Normal);
			float4 BodyTexture = tex2D(_MainTex, IN.uv_MainTex*_DiffuseUVScale);
			float4 _RenderedTxt = tex2D(_RenderedTexture, worldVec);
			float bodySpecularMask = BodyTexture.a;
			float4 BodyDiffuse = (bodySpecularMask * BodyTexture) + (BodyTexture * (1 - bodySpecularMask));

			// reflection
			float4 cubemapTexture = texCUBE(_Cube, worldVec/*mul(_Rotation, worldVec)*/); // user set cubemap
			float4 cubemapTexture2 = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldVec); // unity assigned cubemap

			// transparency
			float4 opacit = _Color.a * _Transprnt;
			// both
            #if Both_T
			float4 reflectionResult = _RefIntensity * cubemapTexture * (_RenderedTxt - (-1 * cubemapTexture.a));
			s.Emission = reflectionResult * (1 - bodySpecularMask) + BodyDiffuse * bodySpecularMask;
			s.Alpha = opacit + reflectionResult + bodySpecularMask;
            #endif
			// remdered texture only
            #if Rendered_Texture
			float4 reflectionResult = _RefIntensity * _RenderedTxt;
			s.Emission = reflectionResult * (1 - bodySpecularMask) + BodyDiffuse * bodySpecularMask;
			s.Alpha = opacit + reflectionResult + bodySpecularMask;
            #endif
			// cubemap only
            #if Cubemap_T
			float4 reflectionResult = _RefIntensity * cubemapTexture;
			s.Emission = reflectionResult * (1 - bodySpecularMask) + BodyDiffuse * bodySpecularMask;
			s.Alpha = opacit + reflectionResult + bodySpecularMask;
            #endif
			// assigned cubemap only
            #if Cubemap_Assigned
			float4 reflectionResult = _RefIntensity * cubemapTexture2;
			s.Emission = reflectionResult * (1 - bodySpecularMask) + BodyDiffuse * bodySpecularMask;
			s.Alpha = opacit + reflectionResult + bodySpecularMask;
            #endif
			// reflection is off
            #if Off_T
			s.Alpha = opacit + bodySpecularMask;
            #endif	

			// combine everything
			s.Albedo = _Color*(1 - bodySpecularMask) + BodyDiffuse* bodySpecularMask;
		}
		ENDCG
	}
	FallBack "Standard"
    CustomEditor "VehicleGlass_Editor"
}
