//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

Shader "Mobile/ Realistic Car Shaders/Plastic"
{
	Properties
	{
		// plastic color
		_Color("Plastic Color", Color) = (1, 1, 1, 1)

		// texture
		_MainTex("Diffuse", 2D) = "white" {}
	    _DiffuseBumpMap("Diffuse Bumpmap", 2D) = "bump" {}
	    _DiffuseUVScale("Diffuse UV Scale", Range(1, 100)) = 1

	    // shininess settings
	    _ShininessIntensity("Plastic Shininess Intensity", Range(0.0, 4.0)) = 0
	    _ShininessScale("Plastic Shininess Scale", Range(0.0, 20.0)) = 0
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 2.0
        #pragma multi_compile Bumped_Diffuse_Off Bumped_Diffuse
		struct Input
		{
		    float2 uv_MainTex;
		    float3 viewDir;
		    INTERNAL_DATA
		};
		sampler2D _MainTex;
		sampler2D _DiffuseBumpMap;
		float4 _Color;
		float _DiffuseUVScale;
	    float _ShininessIntensity;
	    float _ShininessScale;
	    void surf(Input IN, inout SurfaceOutput s)
	    {
            #if Bumped_Diffuse
			s.Normal = normalize(float3(0, 0, 1) + UnpackNormal(tex2D(_DiffuseBumpMap, IN.uv_MainTex*_DiffuseUVScale)));
            #else
			s.Normal = normalize(float3(0, 0, 1));
            #endif
		    // texture
		    float4 BodyTexture = tex2D(_MainTex, IN.uv_MainTex*_DiffuseUVScale);
		    float bodySpecularMask = BodyTexture.a;
		    float4 BodyDiffuse = (_Color * (1 - bodySpecularMask) * BodyTexture) + (_Color*BodyTexture*bodySpecularMask);

		    // reflection	
		    float shininess = _ShininessIntensity* pow(abs(dot(normalize(IN.viewDir), s.Normal)),_ShininessScale);

		    // combine everything
		    s.Albedo = BodyDiffuse * bodySpecularMask + shininess + _Color*(1-bodySpecularMask);
	    }
	    ENDCG
	}
	FallBack "Standard"
	CustomEditor "VehiclePlasticBump_Editor"
}
