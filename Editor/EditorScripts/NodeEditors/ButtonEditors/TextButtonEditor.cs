using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(TextButton))]
    public class TextButtonEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var isDriven = FruityEditorDrawer.LayoutIsDriven(serializedObject);

            FruityEditorDrawer.DrawConfigProperties(serializedObject, DrivenConfig, FreeConfig);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabConfig);
        }

        private static void DrivenConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("maxWidth"));
            EditorGUILayout.PropertyField(so.FindProperty("height"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
            EditorGUILayout.PropertyField(so.FindProperty("cropWidth"));
            EditorGUILayout.PropertyField(so.FindProperty("fontHeightScaling"));
        }

        private static void FreeConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("iconSprite"));
        }

        private static void PrefabConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("BackingImage"));
            EditorGUILayout.PropertyField(so.FindProperty("ButtonText"));
            EditorGUILayout.PropertyField(so.FindProperty("IconImage"));
            EditorGUILayout.PropertyField(so.FindProperty("BoxCollider"));
        }

    }

}