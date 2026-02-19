using UnityEditor;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(ControlLayoutStyle))]
    public class ControlLayoutStyleEditor : Editor {

        public override void OnInspectorGUI () {
            serializedObject.Update();
            var driver = serializedObject.FindProperty("ParentDriver").objectReferenceValue;
            var isDriven = (driver as MonoBehaviour)?.isActiveAndEnabled == true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Nickname"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ParentDriver"));
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginDisabledGroup(isDriven);
            
            EditorGUILayout.LabelField("Dimensions", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("layoutPadding"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("Text Buttons", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cropWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fontHeightScaling"));
            EditorGUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

    }

}