using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(TextButton))]
    public class TextButtonEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var driver = serializedObject.FindProperty("layoutDriver").objectReferenceValue;

            DrawConfigProperties(serializedObject);
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject, 
                restrictSize: true, restrictPadding: driver != null);
            DrawPrefabProperties(serializedObject, ref PrefabFoldout);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("layoutDriver"));
            var disabled = so.FindProperty("layoutDriver").objectReferenceValue != null;

            EditorGUI.BeginDisabledGroup(disabled);
            EditorGUILayout.PropertyField(so.FindProperty("maxWidth"));
            EditorGUILayout.PropertyField(so.FindProperty("height"));
            EditorGUILayout.PropertyField(so.FindProperty("cropWidth"));
            EditorGUILayout.PropertyField(so.FindProperty("fontHeightScaling"));
            if (disabled) {
                EditorGUILayout.LabelField("(These properties are being driven)", EditorStyles.miniLabel);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(so.FindProperty("iconSprite"));
            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

        public static void DrawPrefabProperties (SerializedObject so, ref bool foldOut) {
            foldOut = EditorGUILayout.Foldout(foldOut, "Prefab", true, EditorStyles.foldoutHeader);
            if (foldOut) {
                so.Update();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(so.FindProperty("BackingImage"));
                EditorGUILayout.PropertyField(so.FindProperty("ButtonText"));
                EditorGUILayout.PropertyField(so.FindProperty("IconImage"));
                EditorGUILayout.EndVertical();
                so.ApplyModifiedProperties();
            }
        }

    }

}