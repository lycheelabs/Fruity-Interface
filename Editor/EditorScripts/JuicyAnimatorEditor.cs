using UnityEditor;

namespace LycheeLabs.FruityInterface.Animation {

    [CustomEditor(typeof(JuicyAnimator))]
    public class JuicyAnimatorEditor : Editor {

        SerializedProperty BasePosition;
        SerializedProperty BaseRotation;
        SerializedProperty BaseScale;

        SerializedProperty SpeedScaling;
        SerializedProperty TimeScaling;

        void OnEnable() {
            BasePosition = serializedObject.FindProperty("basePosition");
            BaseRotation = serializedObject.FindProperty("baseRotation");
            BaseScale = serializedObject.FindProperty("baseScale");

            SpeedScaling = serializedObject.FindProperty("speedScaling");
            TimeScaling = serializedObject.FindProperty("useUnscaledTime");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(BasePosition);
            EditorGUILayout.PropertyField(BaseRotation);
            EditorGUILayout.PropertyField(BaseScale);

            EditorGUILayout.PropertyField(SpeedScaling);
            EditorGUILayout.PropertyField(TimeScaling);
            serializedObject.ApplyModifiedProperties();
        }

    }

}