using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(TooltipBubble))]
    public class TooltipBubbleEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var contentsNode = serializedObject.FindProperty("contentsNode").objectReferenceValue;

            FruityEditorDrawer.DrawAdditionalProperties(serializedObject, "Config", ConfigProps);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, sizeIsDriven: contentsNode != null);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabProps);
        }

        private static void ConfigProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("contentsNode"));
            EditorGUILayout.PropertyField(so.FindProperty("arrowOffset"));
            EditorGUILayout.PropertyField(so.FindProperty("arrowLength"));
            EditorGUILayout.PropertyField(so.FindProperty("arrowClearance"));
            EditorGUILayout.PropertyField(so.FindProperty("lerpSpeed"));
            EditorGUILayout.PropertyField(so.FindProperty("MinimumSize"));
        }

        private static void PrefabProps (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("root"));
            EditorGUILayout.PropertyField(so.FindProperty("backing"));
            EditorGUILayout.PropertyField(so.FindProperty("arrow"));
        }

    }

}
