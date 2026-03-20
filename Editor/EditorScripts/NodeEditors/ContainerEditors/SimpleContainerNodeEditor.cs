using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(SimpleContainerNode))]
    public class SimpleContainerNodeEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawAdditionalProperties(serializedObject, "UI Layout", GeneralConfig);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabConfig);
        }

        private static void GeneralConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("LayoutContents"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        private static void PrefabConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("BoxCollider"));
        }

    }

}