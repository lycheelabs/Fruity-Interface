using System;
using UnityEditor;
using UnityEngine;

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
        EditorGUILayout.PropertyField(so.FindProperty("inputParent"));
        EditorGUILayout.PropertyField(so.FindProperty("ignoresInterfaceLock"));
        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
    }

    public static void DrawLayoutProperties (SerializedObject so, bool restrictSize = false, bool restrictPadding = false) {
        if (restrictSize && restrictPadding) return;
        
        so.Update();
        EditorGUILayout.LabelField("UI Layout", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (restrictSize) {
            /*EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(so.FindProperty("LayoutSizePixels"));
            EditorGUILayout.LabelField("(Size is driven from elsewhere)", EditorStyles.miniLabel);
            EditorGUI.EndDisabledGroup();*/
        } else {
            EditorGUILayout.PropertyField(so.FindProperty("LayoutSizePixels"));
        }

        if (restrictPadding) {
            /*EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
            EditorGUILayout.LabelField("(Padding is driven from elsewhere)", EditorStyles.miniLabel);
            EditorGUI.EndDisabledGroup();*/
        } else {
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
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

}