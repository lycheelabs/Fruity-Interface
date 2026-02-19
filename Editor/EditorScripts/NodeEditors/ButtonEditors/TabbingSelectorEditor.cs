using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(TabbingSelector))]
    public class TabbingSelectorEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            var isDriven = FruityEditorDrawer.LayoutIsDriven(serializedObject);

            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawConfigProperties(serializedObject, DrivenConfig, FreeConfig);
            FruityEditorDrawer.DrawPrefabProperties(serializedObject, ref PrefabFoldout, PrefabConfig);
        }

        private static void DrivenConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("width"));
            EditorGUILayout.PropertyField(so.FindProperty("height"));
            EditorGUILayout.PropertyField(so.FindProperty("LayoutPaddingPixels"));
            EditorGUILayout.PropertyField(so.FindProperty("fontHeightScaling"));
            EditorGUILayout.PropertyField(so.FindProperty("iconScaling"));
            EditorGUILayout.PropertyField(so.FindProperty("arrowMargin"));
        }

        private static void FreeConfig (SerializedObject so) {
            //
        }

        private static void PrefabConfig (SerializedObject so) {
            EditorGUILayout.PropertyField(so.FindProperty("MainButton"));
            EditorGUILayout.PropertyField(so.FindProperty("LeftArrow"));
            EditorGUILayout.PropertyField(so.FindProperty("RightArrow"));
        }

    } 

}