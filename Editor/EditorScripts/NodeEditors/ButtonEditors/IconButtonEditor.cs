using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(IconButton))]
    public class IconButtonEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var isDriven = FruityEditorDrawer.LayoutIsDriven(serializedObject);

            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawConfigProperties(serializedObject, DrivenConfig, FreeConfig);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabConfig);
        }

        private static void DrivenConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("size"));
            EditorGUILayout.PropertyField(so.FindProperty("colliderPadding"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
        }

        private static void FreeConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("sprite"));
        }

        private static void PrefabConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("ButtonImage"));
            EditorGUILayout.PropertyField(so.FindProperty("BoxCollider"));
        }

    }

}