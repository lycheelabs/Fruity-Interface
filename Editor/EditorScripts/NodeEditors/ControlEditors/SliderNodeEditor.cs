using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(SliderNode))]
    public class SliderNodeEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawAdditionalProperties(serializedObject, "Config", ConfigProps);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabProps);
        }

        private static void ConfigProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("width"));
            EditorGUILayout.PropertyField(so.FindProperty("Color"));
            EditorGUILayout.PropertyField(so.FindProperty("showValue"));
            EditorGUILayout.PropertyField(so.FindProperty("valueIsPercentage"));
            EditorGUILayout.PropertyField(so.FindProperty("valueTextWidth"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        private static void PrefabProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("bounds"));
            EditorGUILayout.PropertyField(so.FindProperty("barBounds"));
            EditorGUILayout.PropertyField(so.FindProperty("barCollider"));
            EditorGUILayout.PropertyField(so.FindProperty("bar"));
            EditorGUILayout.PropertyField(so.FindProperty("handle"));
            EditorGUILayout.PropertyField(so.FindProperty("barFill"));
            EditorGUILayout.PropertyField(so.FindProperty("counterBounds"));
            EditorGUILayout.PropertyField(so.FindProperty("counter"));
        }

    }

}
