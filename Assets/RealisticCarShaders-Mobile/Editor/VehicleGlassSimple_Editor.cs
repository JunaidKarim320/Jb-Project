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

public class VehicleGlassSimple_Editor : ShaderGUI
{
    private static string _logoImagePath = "Assets/RealisticCarShaders-Mobile/Editor/logoGlassSimple.png";
    MaterialProperty _Color;
    MaterialProperty _Transprnt;
    MaterialProperty _Cube;
    MaterialProperty _RenderedTexture;
    MaterialProperty _RefIntensity;

    public enum ReflectionType
    {
        RenderedTextureReflection,
        CubemapReflection,
        AssignedCubemapReflection,
        BothReflections,
        TurnedOff
    }
    public ReflectionType reflectionType;

    private MaterialEditor materialEditor;
    private MaterialProperty[] materialProperties;
    private Material _material;
    private Color customUIColor;

    private bool firstApply = true,
        ReflectionUVFold, BodyUVFold, DecalsUVFold;

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
        _Transprnt = FindProperty("_Transprnt", materialProperties);
        _Cube = FindProperty("_Cube", materialProperties);
        _RenderedTexture = FindProperty("_RenderedTexture", materialProperties);
        _RefIntensity = FindProperty("_RefIntensity", materialProperties);
        // enum
        if (_material.IsKeywordEnabled("Rendered_Texture"))
            reflectionType = ReflectionType.RenderedTextureReflection;
        if (_material.IsKeywordEnabled("Cubemap_T"))
            reflectionType = ReflectionType.CubemapReflection;
        if (_material.IsKeywordEnabled("Cubemap_Assigned"))
            reflectionType = ReflectionType.AssignedCubemapReflection;
        if (_material.IsKeywordEnabled("Both_T"))
            reflectionType = ReflectionType.BothReflections;
        if (_material.IsKeywordEnabled("Off_T"))
            reflectionType = ReflectionType.TurnedOff;
    }

    void ShowProperties()
    {
        GUI.backgroundColor = customUIColor;
        EditorGUILayout.Space();

        // reflection settings
        EditorGUILayout.HelpBox("Reflection", MessageType.None);
        EditorGUILayout.Space();
        reflectionType = (ReflectionType)EditorGUILayout.EnumPopup("Reflection Type", reflectionType);
        // enum
        if (reflectionType != ReflectionType.TurnedOff)
        {
            materialEditor.ShaderProperty(_RefIntensity, "Reflection Intensity");
        }
        else
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
            _material.EnableKeyword("Off_T");
        }
        if (reflectionType == ReflectionType.RenderedTextureReflection)
        {
            _material.EnableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
            _material.DisableKeyword("Off_T");
            materialEditor.TexturePropertySingleLine(new GUIContent("Rendered Texture"), _RenderedTexture);
        }
        if (reflectionType == ReflectionType.CubemapReflection)
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.EnableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
            _material.DisableKeyword("Off_T");
            materialEditor.TexturePropertySingleLine(new GUIContent("Reflection Cubemap"), _Cube);
        }
        if (reflectionType == ReflectionType.AssignedCubemapReflection)
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.EnableKeyword("Cubemap_Assigned");
            _material.DisableKeyword("Both_T");
            _material.DisableKeyword("Off_T");
        }
        if (reflectionType == ReflectionType.BothReflections)
        {
            _material.DisableKeyword("Rendered_Texture");
            _material.DisableKeyword("Cubemap_T");
            _material.DisableKeyword("Cubemap_Assigned");
            _material.EnableKeyword("Both_T");
            _material.DisableKeyword("Off_T");
            materialEditor.TexturePropertySingleLine(new GUIContent("Rendered Texture"), _RenderedTexture);
            materialEditor.TexturePropertySingleLine(new GUIContent("Reflection Cubemap"), _Cube);
        }
        EditorGUILayout.Space();

        // body settings
        EditorGUILayout.HelpBox("Body", MessageType.None);
        EditorGUILayout.Space();
        materialEditor.ShaderProperty(_Color, "Glass Color");
        materialEditor.ShaderProperty(_Transprnt, "Glass Transparency");
        EditorGUILayout.Space();

        // render queue
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        materialEditor.RenderQueueField();
        EditorGUILayout.Space();
    }
}