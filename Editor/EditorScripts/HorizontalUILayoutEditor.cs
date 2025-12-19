using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(HorizontalUILayout))]
    public class HorizontalUILayoutEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject, restrictSize: true);
            DrawConfigProperties(serializedObject);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("minimumSize"));

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}