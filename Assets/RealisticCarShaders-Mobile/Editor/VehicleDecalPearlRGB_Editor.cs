//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VehicleDecalPearlRGB_Editor : ShaderGUI
{
    private static string _logoImagePath = "Assets/RealisticCarShaders-Mobile/Editor/logoPearlRGB.png";
    MaterialProperty _Color;
    MaterialProperty _MainTex;
    MaterialProperty _DiffuseBumpMap;
    MaterialProperty _DiffuseUVScale;
    MaterialProperty _DecalColor;
    MaterialProperty _Decal;
    MaterialProperty _DecalTransparency;
    MaterialProperty _DecalReflection;
    MaterialProperty _DecalUVScale;
    MaterialProperty _PearlescentColor;
    MaterialProperty _MainTexPearl;
    MaterialProperty _PearlBumpMap;
    MaterialProperty _PearlUVScale;
    MaterialProperty _Cube;
    MaterialProperty _RenderedTexture;
    MaterialProperty _RefIntensity;
    MaterialProperty _RefVisibility;
    MaterialProperty _MetalBrightnessIntensity;
    MaterialProperty _ShininessIntensity;
    MaterialProperty _ShininessScale;

    public enum ReflectionType
    {
        RenderedTextureReflection,
        CubemapReflection,
        AssignedCubemapReflection,
        BothReflections
    }
    public ReflectionType reflectionType;

    private MaterialEditor materialEditor;
    private MaterialProperty[] materialProperties;
    private Material _material;
    private Color customUIColor;

    private bool firstApply = true,
        ReflectionUVFold, BodyUVFold, DecalsUVFold, PearlUVFold, DiffuseBump, PearlBump;

    // custom logo
    Texture texLogo = AssetDatabase.LoadAssetAtPath<Texture>(_logoImagePath);
    public Rect logoRect;

    public override void OnGUI(MaterialEditor _materialEditor, MaterialProperty[] _materialProperties)
    {
        EditorGUI.BeginChangeCheck();
        if (firstApply)
        {
            materialEditor = _materialEditor;
            firstApply = false;
            customUIColor = new Color(.6f, .6f, .6f);
        }
        materialProperties = _materialProperties;
        _material = materialEditor.target as Material;
        if (!materialEditor.isVisible)
            return;
        GetValues();

        logoRect.height = texLogo.height;
        logoRect.width = texLogo.width;
        GUILayout.BeginHorizontal();
        GUILayout.Space((EditorGUIUtility.currentViewWidth - texLogo.width - 10f) / 2f);
        GUILayout.Label(texLogo, GUILayout.Width(logoRect.width - 25f), GUILayout.Height(logoRect.height));
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;

        // material preview
        EditorGUILayout.BeginVertical(GUI.skin.textArea);
        materialEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), EditorStyles.toolbar);
        GUI.backgroundColor = customUIColor;
        EditorGUILayout.EndVertical();

        // material preview buttons
        GUILayout.BeginHorizontal(GUI.skin.button);
        materialEditor.DefaultPreviewSettingsGUI();
        GUI.backgroundColor = customUIColor;
        GUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(.7f, .7f, .7f);

        EditorGUILayout.BeginVertical(GUI.skin.button);
        ShowProperties();
        EditorGUILayout.EndVertical();
    }

    void GetValues()
    {
        _Color = FindProperty("_Color", materialProperties);
        _MainTex = FindProperty("_MainTex", materialProperties);
        _DiffuseBumpMap = FindProperty("_DiffuseBumpMap", materialProperties);
        _DiffuseUVScale = FindProperty("_DiffuseUVScale", materialProperties);
        _DecalColor = FindProperty("_DecalColor", materialProperties);
        _Decal = FindProperty("_Decal", materialProperties);
        _DecalTransparency = FindProperty("_DecalTransparency", materialProperties);
        _DecalReflection = FindProperty("_DecalReflection", materialProperties);
        _DecalUVScale = FindProperty("_DecalUVScale", materialProperties);
        _PearlescentColor = FindProperty("_PearlescentColor", materialProperties);
        _MainTexPearl = FindProperty("_MainTexPearl", materialProperties);
        _PearlBumpMap = FindProperty("_PearlBumpMap", materialProperties);
        _PearlUVScale = FindProperty("_PearlUVScale", materialProperties);
        _Cube = FindProperty("_Cube", materialProperties);
        _RenderedTexture = FindProperty("_RenderedTexture", materialProperties);
        _RefIntensity = FindProperty("_RefIntensity", materialProperties);
        _RefVisibility = FindProperty("_RefVisibility", materialProperties);
        _MetalBrightnessIntensity = FindProperty("_MetalBrightnessIntensity", materialProperties);
        _ShininessIntensity = FindProperty("_ShininessIntensity", materialProperties);
        _ShininessScale = FindProperty("_ShininessScale", materialProperties);
        // enum
        if (_material.IsKeywordEnabled("Rendered_Texture"))
            reflectionType = ReflectionType.RenderedTextureReflection;
        if (_material.IsKeywordEnabled("Cubemap_T"))
            reflectionType = ReflectionType.CubemapReflection;
        if (_material.IsKeywordEnabled("Cubemap_Assigned"))
            reflectionType = ReflectionType.AssignedCubemapReflection;
        if (_material.IsKeywordEnabled("Both_T"))
            reflectionType = ReflectionType.BothReflections;
        // toggle
        if (_material.IsKeywordEnabled("Bumped_Diffuse"))
            DiffuseBump = true;
        else
            DiffuseBump = false;
        if (_material.IsKeywordEnabled("Bumped_Pearl"))
            PearlBump = true;
        else
            PearlBump = false;
    }

    void ShowProperties()
    {
        GUI.backgroundColor = customUIColor;
        EditorGUILayout.Space();

        // reflection settings
        EditorGUILayout.HelpBox("Reflection", MessageType.None);
        EditorGUILayout.Space();
        reflectionType = (ReflectionType)EditorGUILayout.EnumPopup("Reflection Type", reflectionType);
        
        materialEditor.ShaderProperty(_RefIntensity, "Reflection Intensity");
        materialEditor.ShaderProperty(_RefVisibility, "Reflection Holographic Scale");
        materialEditor.ShaderProperty(_MetalBrightnessIntensity, "Metal Brightness Intensity");
        // enum
        if (reflectionType == ReflectionType.RenderedTextureReflection)
        {
            _material.EnableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
            materialEditor.TexturePropertySingleLine(new GUIContent("Rendered Texture"), _RenderedTexture);
        }
        if (reflectionType == ReflectionType.CubemapReflection)
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.EnableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
            materialEditor.TexturePropertySingleLine(new GUIContent("Reflection Cubemap"), _Cube);
        }
        if (reflectionType == ReflectionType.AssignedCubemapReflection)
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.EnableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
        }
        if (reflectionType == ReflectionType.BothReflections)
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.EnableKeyword("Both_T");
            materialEditor.TexturePropertySingleLine(new GUIContent("Rendered Texture"), _RenderedTexture);
            materialEditor.TexturePropertySingleLine(new GUIContent("Reflection Cubemap"), _Cube);
        }
        EditorGUILayout.Space();

        // body settings
        EditorGUILayout.HelpBox("Body", MessageType.None);
        EditorGUILayout.Space();
        DiffuseBump = EditorGUILayout.Toggle("Bump Map", DiffuseBump);
        materialEditor.ShaderProperty(_Color, "Vehicle Color");
        materialEditor.TexturePropertySingleLine(new GUIContent("Diffuse Texture"), _MainTex);
        if (DiffuseBump)
        {
            _material.EnableKeyword("Bumped_Diffuse");
            materialEditor.TexturePropertySingleLine(new GUIContent("Diffuse Bump Map"), _DiffuseBumpMap);
        }
        else
        {
            _material.DisableKeyword("Bumped_Diffuse");
        }

        materialEditor.ShaderProperty(_DiffuseUVScale, "Diffuse UV Scale");
        BodyUVFold = EditorGUILayout.Foldout(BodyUVFold, "Diffuse UV");
        if (BodyUVFold)
            materialEditor.TextureScaleOffsetProperty(_MainTex);
        EditorGUILayout.Space();

        // pearlescent settings
        EditorGUILayout.HelpBox("Pearlescent / Flakes / Carbon", MessageType.None);
        EditorGUILayout.Space();
        PearlBump = EditorGUILayout.Toggle("Bump Map", PearlBump);
        materialEditor.ShaderProperty(_PearlescentColor, "Color");
        materialEditor.ShaderProperty(_ShininessIntensity, "Intensity");
        materialEditor.ShaderProperty(_ShininessScale, "Visibility Scale");
        materialEditor.TexturePropertySingleLine(new GUIContent("Flakes/Carbon Texture"), _MainTexPearl);
        if (PearlBump)
        {
            _material.EnableKeyword("Bumped_Pearl");
            materialEditor.TexturePropertySingleLine(new GUIContent("Flakes/Carbon Bump Map"), _PearlBumpMap);
        }
        else
        {
            _material.DisableKeyword("Bumped_Pearl");
        }
        materialEditor.ShaderProperty(_PearlUVScale, "Texture UV Scale");
        PearlUVFold = EditorGUILayout.Foldout(PearlUVFold, "Diffuse UV");
        if (PearlUVFold)
            materialEditor.TextureScaleOffsetProperty(_MainTexPearl);
        EditorGUILayout.Space();

        // decals settings
        EditorGUILayout.HelpBox("Decals", MessageType.None);
        EditorGUILayout.Space();
        materialEditor.ShaderProperty(_DecalColor, "Decal Color");
        materialEditor.TexturePropertySingleLine(new GUIContent("Decal Texture"), _Decal);
        materialEditor.ShaderProperty(_DecalTransparency, "Decal Transparency");
        materialEditor.ShaderProperty(_DecalReflection, "Decal Reflection");
        materialEditor.ShaderProperty(_DecalUVScale, "Decal UV Scale");
        DecalsUVFold = EditorGUILayout.Foldout(DecalsUVFold, "Decal UV");
        if (DecalsUVFold)
            materialEditor.TextureScaleOffsetProperty(_Decal);

        // render queue
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        materialEditor.RenderQueueField();
        EditorGUILayout.Space();
    }
}