using UnityEditor;
using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(ListContainerNode))]
    public class ListContainerNodeEditor : UnityEditor.Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, sizeIsDriven: true);
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
