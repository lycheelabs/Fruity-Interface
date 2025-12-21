using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(SpacerNode))]
    public class SpacerNodeEditor : Editor {

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject, true, true);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("Size"));

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}