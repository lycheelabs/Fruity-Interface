using System;
using LycheeLabs.FruityInterface;
using LycheeLabs.FruityInterface.Elements;
using UnityEditor;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Editors {

public static class FruityEditorDrawer {

    public static bool LayoutIsDriven (SerializedObject serializedObject) {
        var driver = serializedObject.FindProperty("LayoutDriver").objectReferenceValue;
        return (driver as MonoBehaviour)?.isActiveAndEnabled == true;
    }

    public static void DrawConfigProperties (SerializedObject so, 
            Action<SerializedObject> drawDrivenProperties, 
            Action<SerializedObject> drawFreeProperties) {
        
        so.Update();
        EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(so.FindProperty("LayoutDriver"));
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (LayoutIsDriven(so)) {
            //EditorGUILayout.LabelField("(These properties are being driven)", EditorStyles.label);
            EditorGUI.BeginDisabledGroup(true);
            drawDrivenProperties?.Invoke(so);
            EditorGUI.EndDisabledGroup();
        } 
        else {
            drawDrivenProperties?.Invoke(so);
        }
        EditorGUILayout.EndVertical();
        drawFreeProperties?.Invoke(so);

        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
    }

    public static void DrawNodeTreeProperties (SerializedObject so) {
        so.Update();
        EditorGUILayout.LabelField("Node Tree", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(so.FindProperty("inputParentOverride"));
        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
    }

    public static void DrawLayoutProperties (SerializedObject so, bool restrictSize = false, bool restrictPadding = false,
            bool sizeIsDriven = false, string drivenSizeSource = "contents") {
        if (restrictSize && restrictPadding) return;
        
        so.Update();
        EditorGUILayout.LabelField("UI Layout", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (!restrictSize) {
            DrawSizeProperties(so.FindProperty("LayoutSizePixels"), sizeIsDriven, drivenSizeSource);
        }

        if (!restrictPadding) {
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"), new GUIContent("Padding"));
        }

        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
    }

    public static void DrawSizeProperties (SerializedProperty property, bool isDriven = false,
            string drivenSource = "contents", string widthLabel = "Width", string heightLabel = "Height") {
        DrawDrivenProperty(property.FindPropertyRelative("x"), isDriven,
            $"({widthLabel} is driven from {drivenSource})", widthLabel);
        DrawDrivenProperty(property.FindPropertyRelative("y"), isDriven,
            $"({heightLabel} is driven from {drivenSource})", heightLabel);
    }

    public static void DrawDrivenProperty (SerializedProperty property, bool isDriven, string warning,
            string label = null) {
        EditorGUI.BeginDisabledGroup(isDriven);
        EditorGUILayout.PropertyField(property, label == null ? GUIContent.none : new GUIContent(label));
        EditorGUI.EndDisabledGroup();

        if (isDriven) {
            EditorGUILayout.LabelField(warning, EditorStyles.miniLabel);
        }
    }

    public static void DrawPrefabProperties (SerializedObject so, ref bool foldOut,
            Action<SerializedObject> drawDrivenProperties) {

        foldOut = EditorGUILayout.Foldout(foldOut, "Prefab", true, EditorStyles.foldoutHeader);
        if (foldOut) {
            so.Update();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            drawDrivenProperties?.Invoke(so);
            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }
    }

    public static void DrawAdditionalProperties (SerializedObject so, string title, Action<SerializedObject> drawProperties) {
        so.Update();
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        drawProperties?.Invoke(so);
        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
    }

}

}
