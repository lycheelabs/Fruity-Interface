using UnityEditor;
using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(TabbingContainerNode))]
    public class TabbingContainerNodeEditor : UnityEditor.Editor {

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

            EditorGUILayout.PropertyField(so.FindProperty("index"));
            FruityEditorDrawer.DrawSizeProperties(so.FindProperty("minimumSize"),
                widthLabel: "Minimum Width", heightLabel: "Minimum Height");

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}
