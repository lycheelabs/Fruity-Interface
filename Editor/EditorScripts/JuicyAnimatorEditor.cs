using UnityEditor;

namespace LycheeLabs.FruityInterface.Animation {

    [CustomEditor(typeof(JuicyAnimator))]
    public class JuicyAnimatorEditor : Editor {

        SerializedProperty BasePosition;
        SerializedProperty BaseRotation;
        SerializedProperty BaseScale;
        SerializedProperty TimeScaling;

        void OnEnable() {
            BasePosition = serializedObject.FindProperty("basePosition");
            BaseRotation = serializedObject.FindProperty("baseRotation");
            BaseScale = serializedObject.FindProperty("baseScale");
            TimeScaling = serializedObject.FindProperty("timeScaling");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(BasePosition);
            EditorGUILayout.PropertyField(BaseRotation);
            EditorGUILayout.PropertyField(BaseScale);
            EditorGUILayout.PropertyField(TimeScaling);
            serializedObject.ApplyModifiedProperties();
        }

    }

}