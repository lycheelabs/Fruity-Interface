using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(SimpleContainerNode))]
    public class SimpleContainerNodeEditor : Editor {

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

            EditorGUILayout.PropertyField(so.FindProperty("ContentsNode"));

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}