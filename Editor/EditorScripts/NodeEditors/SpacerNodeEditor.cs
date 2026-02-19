using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(SpacerNode))]
    public class SpacerNodeEditor : Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, true, true);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("Size"));
            EditorGUILayout.PropertyField(so.FindProperty("Orientation"));

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}