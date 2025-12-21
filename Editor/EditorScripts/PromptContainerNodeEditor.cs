using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(PromptContainerNode))]
    public class PromptContainerNodeEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject, restrictSize: true);
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