using UnityEditor;
using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface.Editor {

    [CustomEditor(typeof(SimpleContainerNode))]
    public class SimpleContainerNodeEditor : UnityEditor.Editor {

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
