using LycheeLabs.FruityInterface.Elements;
using UnityEditor;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(TextBoxNode))]
    public class TextBoxNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, restrictSize: true);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(so.FindProperty("Width"));
            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}
