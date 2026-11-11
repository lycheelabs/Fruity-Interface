using UnityEditor;
using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(ButtonSelectorNode))]
    public class ButtonSelectorNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            DrawConfigProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject, sizeIsDriven: true,
                drivenSizeSource: "contents");
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(so.FindProperty("selectedIndex"));
            EditorGUILayout.PropertyField(so.FindProperty("selectedColor"));
            EditorGUILayout.PropertyField(so.FindProperty("unselectedColor"));
            FruityEditorDrawer.DrawSizeProperties(so.FindProperty("minimumSize"),
                widthLabel: "Minimum Width", heightLabel: "Minimum Height");

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}
