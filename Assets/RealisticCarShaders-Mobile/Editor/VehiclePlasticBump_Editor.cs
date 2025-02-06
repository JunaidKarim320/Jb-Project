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

public class VehiclePlasticBump_Editor : ShaderGUI
{
    private static string _logoImagePath = "Assets/RealisticCarShaders-Mobile/Editor/logoPlasticBump.png";
    MaterialProperty _Color;
    MaterialProperty _MainTex;
    MaterialProperty _DiffuseBumpMap;
    MaterialProperty _DiffuseUVScale;
    MaterialProperty _ShininessIntensity;
    MaterialProperty _ShininessScale;

    private MaterialEditor materialEditor;
    private MaterialProperty[] materialProperties;
    private Material _material;
    private Color customUIColor;

    private bool firstApply = true,
        ReflectionUVFold, BodyUVFold, DecalsUVFold, DiffuseBump;

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
        _ShininessIntensity = FindProperty("_ShininessIntensity", materialProperties);
        _ShininessScale = FindProperty("_ShininessScale", materialProperties);
        // toggle
        if (_material.IsKeywordEnabled("Bumped_Diffuse"))
            DiffuseBump = true;
        else
            DiffuseBump = false;
    }

    void ShowProperties()
    {
        GUI.backgroundColor = customUIColor;
        EditorGUILayout.Space();

        // body settings
        EditorGUILayout.HelpBox("Body", MessageType.None);
        EditorGUILayout.Space();
        DiffuseBump = EditorGUILayout.Toggle("Bump Map", DiffuseBump);
        materialEditor.ShaderProperty(_Color, "Plastic Color");
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
        EditorGUILayout.HelpBox("Plastic", MessageType.None);
        EditorGUILayout.Space();
        materialEditor.ShaderProperty(_ShininessIntensity, "Plastic Shininess Intensity");
        materialEditor.ShaderProperty(_ShininessScale, "Plastic Shininess Scale");
        EditorGUILayout.Space();

        // render queue
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        materialEditor.RenderQueueField();
        EditorGUILayout.Space();
    }
}