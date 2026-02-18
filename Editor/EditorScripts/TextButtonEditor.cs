using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(TextButton))]
    public class TextButtonEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var driver = serializedObject.FindProperty("layoutDriver").objectReferenceValue;
            var isDriven = (driver as ButtonLayoutStyle)?.isActiveAndEnabled == true;

            DrawConfigProperties(serializedObject, isDriven);
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject, 
                restrictSize: true, restrictPadding: isDriven);
            DrawPrefabProperties(serializedObject, ref PrefabFoldout);
        }

        public static void DrawConfigProperties (SerializedObject so, bool isDriven) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("layoutDriver"));
            var disabled = isDriven;

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
                EditorGUILayout.PropertyField(so.FindProperty("BoxCollider"));
                EditorGUILayout.EndVertical();
                so.ApplyModifiedProperties();
            }
        }

    }

}