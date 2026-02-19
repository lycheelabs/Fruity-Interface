using UnityEditor;

namespace LycheeLabs.FruityInterface.Animation {

    [CustomEditor(typeof(JuicyGroup))]
    public class JuicyGroupEditor : Editor {

        SerializedProperty Children;
        SerializedProperty TimeScaling;

        void OnEnable() {
            Children = serializedObject.FindProperty("Children");
            TimeScaling = serializedObject.FindProperty("timeScaling");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(Children);
            EditorGUILayout.PropertyField(TimeScaling);
            serializedObject.ApplyModifiedProperties();
        }

    }

}