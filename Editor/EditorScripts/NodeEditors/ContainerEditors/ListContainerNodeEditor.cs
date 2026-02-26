using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(ListContainerNode))]
    public class ListContainerNodeEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, restrictSize: true);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("Orientation"));
            EditorGUILayout.PropertyField(so.FindProperty("minimumSize"));

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}