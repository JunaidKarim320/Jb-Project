//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

Shader "Mobile/ Realistic Car Shaders/Glass + Diffuse + Decal"
{
	Properties
	{
		// glass color
		_Color("Glass Color", Color) = (1, 1, 1, 1)

		// glass transparency
		_Transprnt("Glass Transparency", Range(0.05, 0.9)) = 0.5

		// glass texture for example: heater, decal, damage
		_MainTex("Diffuse", 2D) = "white" {}
	    _DiffuseBumpMap("Diffuse Bumpmap", 2D) = "bump" {}
	    _DiffuseUVScale("Diffuse UV Scale", Range(1, 100)) = 1

		// decal
		_DecalColor("Decal Color", Color) = (1, 1, 1,1)
		_Decal("Decal", 2D) = "white" {}
	    _DecalTransparency("Decal Transparency", Range(0.1, 1)) = 1
		_DecalReflection("Decal Reflection", Range(0, 1)) = 0.5
		_DecalUVScale("Decal UV Scale", Range(1, 50)) = 1

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
		float2 uv_Decal;
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
	sampler2D _Decal;
	samplerCUBE _Cube;
	float4 _Color;
	float4 _DecalColor;
	float _DiffuseUVScale;
	float _DecalTransparency;
	float _DecalReflection;
	float _DecalUVScale;
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
		float4 DecalTexture = tex2D(_Decal, IN.uv_Decal*_DecalUVScale);
		float4 _RenderedTxt = tex2D(_RenderedTexture, worldVec);
		float bodySpecularMask = BodyTexture.a;
		float decalSpecularMask = DecalTexture.a*_DecalTransparency;
		float4 BodyDiffuse = (bodySpecularMask * BodyTexture) + (BodyTexture * (1 - bodySpecularMask));
		float4 DecalDiffuse = ((_DecalColor * decalSpecularMask) * DecalTexture) + (DecalTexture * (1 - decalSpecularMask));

		// reflection
		float4 cubemapTexture = texCUBE(_Cube, worldVec/*mul(_Rotation, worldVec)*/); // user set cubemap
		float4 cubemapTexture2 = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldVec); // unity assigned cubemap

		// transparency
		float4 opacit = _Color.a * _Transprnt;
		// both
        #if Both_T
		float4 reflectionResult = _RefIntensity * cubemapTexture * (_RenderedTxt - (-1 * cubemapTexture.a));
		s.Emission = reflectionResult * (1 - bodySpecularMask) * (1 - decalSpecularMask) + reflectionResult*decalSpecularMask*_DecalReflection + BodyDiffuse * bodySpecularMask* (1 - decalSpecularMask);
		s.Alpha = opacit *(1 - decalSpecularMask) + reflectionResult + bodySpecularMask* (1 - decalSpecularMask) + DecalTexture *decalSpecularMask;
        #endif
		// remdered texture only
        #if Rendered_Texture
		float4 reflectionResult = _RefIntensity * _RenderedTxt;
		s.Emission = reflectionResult * (1 - bodySpecularMask) * (1 - decalSpecularMask) + reflectionResult*decalSpecularMask*_DecalReflection + BodyDiffuse * bodySpecularMask*(1 - decalSpecularMask);
		s.Alpha = opacit *(1 - decalSpecularMask) + reflectionResult + bodySpecularMask* (1 - decalSpecularMask) + DecalTexture *decalSpecularMask;
        #endif
		// cubemap only
        #if Cubemap_T
		float4 reflectionResult = _RefIntensity * cubemapTexture;
		s.Emission = reflectionResult * (1 - bodySpecularMask) * (1 - decalSpecularMask) + reflectionResult*decalSpecularMask*_DecalReflection + BodyDiffuse * bodySpecularMask* (1 - decalSpecularMask);
		s.Alpha = opacit *(1 - decalSpecularMask) + reflectionResult + bodySpecularMask* (1 - decalSpecularMask) + DecalTexture *decalSpecularMask;
        #endif
		// assigned cubemap only
        #if Cubemap_Assigned
		float4 reflectionResult = _RefIntensity * cubemapTexture2;
		s.Emission = reflectionResult * (1 - bodySpecularMask) * (1 - decalSpecularMask) + reflectionResult * decalSpecularMask * _DecalReflection + BodyDiffuse * bodySpecularMask * (1 - decalSpecularMask);
		s.Alpha = opacit * (1 - decalSpecularMask) + reflectionResult + bodySpecularMask * (1 - decalSpecularMask) + DecalTexture * decalSpecularMask;
        #endif
		// reflection is off
        #if Off_T
		s.Alpha = opacit *(1 - decalSpecularMask) + bodySpecularMask* (1 - decalSpecularMask) + DecalTexture *decalSpecularMask;
        #endif	

		// combine everything
		s.Albedo = _Color*(1 - bodySpecularMask)* (1 - decalSpecularMask) + BodyDiffuse* bodySpecularMask* (1 - decalSpecularMask) + DecalDiffuse * decalSpecularMask;
	}
	ENDCG
	}
	FallBack "Standard"
	CustomEditor "VehicleGlassDecalBump_Editor"
}