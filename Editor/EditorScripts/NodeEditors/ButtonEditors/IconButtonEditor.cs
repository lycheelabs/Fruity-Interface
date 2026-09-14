using UnityEditor;
using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(IconButton))]
    public class IconButtonEditor : UnityEditor.Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var isDriven = FruityEditorDrawer.LayoutIsDriven(serializedObject);

            FruityEditorDrawer.DrawConfigProperties(serializedObject, DrivenConfig, null);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabConfig);
        }

        private static void DrivenConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("size"));
            EditorGUILayout.PropertyField(so.FindProperty("colliderPadding"));
            EditorGUILayout.PropertyField(so.FindProperty("iconScaling"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        private static void PrefabConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("ButtonImage"));
            EditorGUILayout.PropertyField(so.FindProperty("BoxCollider"));
        }

    }

}
