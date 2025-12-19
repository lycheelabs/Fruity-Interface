using UnityEditor;

public static class InterfaceNodeDrawer {

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
        so.Update();
        EditorGUILayout.LabelField("UI Layout", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (restrictSize) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(so.FindProperty("LayoutSizePixels"));
            EditorGUILayout.LabelField("(Size is driven from elsewhere)", EditorStyles.miniLabel);
            EditorGUI.EndDisabledGroup();
        } else {
            EditorGUILayout.PropertyField(so.FindProperty("LayoutSizePixels"));
        }

        if (restrictPadding) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
            EditorGUILayout.LabelField("(Padding is driven from elsewhere)", EditorStyles.miniLabel);
            EditorGUI.EndDisabledGroup();
        } else {
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
    }

}