using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(SettingNode))]
    public class SettingNodeEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawAdditionalProperties(serializedObject, "Label", LabelProps);
            FruityEditorDrawer.DrawConfigProperties(serializedObject, DrivenConfig, null);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabProps);
        }

        private static void LabelProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("text"));
        }

        private static void DrivenConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("width"));
            EditorGUILayout.PropertyField(so.FindProperty("height"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
            EditorGUILayout.PropertyField(so.FindProperty("fontHeightScaling"));
            EditorGUILayout.PropertyField(so.FindProperty("contentRatio"));
        }

        private static void PrefabProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("TextField"));
            EditorGUILayout.PropertyField(so.FindProperty("TextBounds"));
            EditorGUILayout.PropertyField(so.FindProperty("ControlNode"));
            EditorGUILayout.PropertyField(so.FindProperty("ControlBounds"));
        }

    }

}
