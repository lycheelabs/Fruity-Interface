using LycheeLabs.FruityInterface.Elements;
using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(TextBoxNode))]
    public class TextBoxNodeEditor : Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, restrictSize: true); 
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