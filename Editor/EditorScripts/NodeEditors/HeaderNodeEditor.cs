using UnityEditor;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(HeaderNode))]
    public class HeaderNodeEditor : UnityEditor.Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawAdditionalProperties(serializedObject, "Config", DrawConfigProperties);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, DrawPrefabProperties);
        }

        private static void DrawConfigProperties (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("textScale"), new GUIContent("Text Scale"));
        }

        private static void DrawPrefabProperties (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("text"));
        }

    }

}
