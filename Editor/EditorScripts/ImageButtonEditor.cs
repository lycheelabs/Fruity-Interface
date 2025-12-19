using UnityEditor;

namespace LycheeLabs.FruityInterface.Elements {

    [CustomEditor(typeof(ImageButton))]
    public class ImageButtonEditor : Editor {

        public bool PrefabFoldout;

        public override void OnInspectorGUI () {
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject, restrictSize: true);
            DrawConfigProperties(serializedObject);
            DrawPrefabProperties(serializedObject, ref PrefabFoldout);
        }

        public static void DrawConfigProperties (SerializedObject so) {
            so.Update();
            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(so.FindProperty("sprite"));
            EditorGUILayout.PropertyField(so.FindProperty("size"));
            EditorGUILayout.PropertyField(so.FindProperty("colliderPadding"));
            EditorGUILayout.EndVertical();
            so.ApplyModifiedProperties();
        }

        public static void DrawPrefabProperties (SerializedObject so, ref bool foldOut) {
            foldOut = EditorGUILayout.Foldout(foldOut, "Prefab", true, EditorStyles.foldoutHeader);
            if (foldOut) {
                so.Update();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(so.FindProperty("ButtonImage"));
                EditorGUILayout.EndVertical();
                so.ApplyModifiedProperties();
            }
        }

    }

}