#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class NavAreaMask
{
    public int mask;
}

[CustomPropertyDrawer(typeof(NavAreaMask))]
public class NavAreaMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
        var dropdownRect = new Rect(position.x, position.y, position.width, position.height);
        
        var navMask = property.FindPropertyRelative("mask");
        navMask.intValue = EditorGUI.MaskField(dropdownRect, navMask.intValue, GameObjectUtility.GetNavMeshAreaNames());
        
        EditorGUI.EndProperty();
    }
}
#endif
