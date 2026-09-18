using UnityEditor;
using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(ListContainerNode))]
    public class ListContainerNodeEditor : UnityEditor.Editor {

        public bool PrefabFoldout;

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

            EditorGUILayout.PropertyField(so.FindProperty("Orientation"));
            FruityEditorDrawer.DrawSizeProperties(so.FindProperty("minimumSize"),
                widthLabel: "Minimum Width", heightLabel: "Minimum Height");

            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

    }

}
