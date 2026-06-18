using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(ToggleSwitch))]
    public class ToggleSwitchEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawAdditionalProperties(serializedObject, "Config", ConfigProps);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabProps);
        }

        private static void ConfigProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("OnSprite"));
            EditorGUILayout.PropertyField(so.FindProperty("OffSprite"));
            EditorGUILayout.PropertyField(so.FindProperty("OnColor"));
            EditorGUILayout.PropertyField(so.FindProperty("OffColor"));
            EditorGUILayout.PropertyField(so.FindProperty("backingColor"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        private static void PrefabProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("Backing"));
            EditorGUILayout.PropertyField(so.FindProperty("Border"));
            EditorGUILayout.PropertyField(so.FindProperty("Dot"));
            EditorGUILayout.PropertyField(so.FindProperty("Icon"));
            EditorGUILayout.PropertyField(so.FindProperty("Animator"));
        }

    }

}
