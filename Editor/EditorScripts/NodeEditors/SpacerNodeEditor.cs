using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(SpacerNode))]
    public class SpacerNodeEditor : Editor {

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, true, true);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
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