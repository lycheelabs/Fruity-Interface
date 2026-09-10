#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(FruityInterfaceInputModule))]
    public sealed class FruityInterfaceInputModuleEditor : UnityEditor.Editor {

        private const string DEFAULT_ACTIONS_PATH =
            "Packages/com.unity.inputsystem/InputSystem/Runtime/Plugins/PlayerInput/DefaultInputActions.inputactions";

        private SerializedProperty actionsAsset;
        private SerializedProperty point;
        private SerializedProperty leftClick;
        private SerializedProperty rightClick;
        private SerializedProperty middleClick;
        private SerializedProperty scrollWheel;
        private InputActionReference[] availableReferences;
        private string[] availableNames;

        private void OnEnable() {
            actionsAsset = serializedObject.FindProperty(nameof(FruityInterfaceInputModule.actionsAsset));
            point = serializedObject.FindProperty(nameof(FruityInterfaceInputModule.point));
            leftClick = serializedObject.FindProperty(nameof(FruityInterfaceInputModule.leftClick));
            rightClick = serializedObject.FindProperty(nameof(FruityInterfaceInputModule.rightClick));
            middleClick = serializedObject.FindProperty(nameof(FruityInterfaceInputModule.middleClick));
            scrollWheel = serializedObject.FindProperty(nameof(FruityInterfaceInputModule.scrollWheel));
            AssignDefaultAssetIfNeeded();
            RefreshAvailableReferences();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(actionsAsset);

            if (serializedObject.ApplyModifiedProperties()) {
                ((FruityInterfaceInputModule)target).AssignActionsFromAsset();
                RefreshAvailableReferences();
                serializedObject.Update();
            }

            DrawActionPopup("Point", point);
            DrawActionPopup("Left Click", leftClick);
            DrawActionPopup("Right Click", rightClick);
            DrawActionPopup("Middle Click", middleClick);
            DrawActionPopup("Scroll Wheel", scrollWheel);

            EditorGUILayout.Space();
            DrawPropertiesExcluding(
                serializedObject,
                nameof(FruityInterfaceInputModule.actionsAsset),
                nameof(FruityInterfaceInputModule.point),
                nameof(FruityInterfaceInputModule.leftClick),
                nameof(FruityInterfaceInputModule.rightClick),
                nameof(FruityInterfaceInputModule.middleClick),
                nameof(FruityInterfaceInputModule.scrollWheel));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawActionPopup(string label, SerializedProperty property) {
            var current = property.objectReferenceValue as InputActionReference;
            var currentIndex = 0;
            if (current != null && availableReferences != null) {
                for (var i = 0; i < availableReferences.Length; i++) {
                    if (availableReferences[i].action == current.action) {
                        currentIndex = i + 1;
                        break;
                    }
                }
            }

            var selectedIndex = EditorGUILayout.Popup(label, currentIndex, availableNames ?? new[] { "None" });
            if (selectedIndex != currentIndex) {
                property.objectReferenceValue = selectedIndex == 0
                    ? null
                    : availableReferences[selectedIndex - 1];
            }
        }

        private void RefreshAvailableReferences() {
            var module = (FruityInterfaceInputModule)target;
            if (module.actionsAsset == null) {
                availableReferences = null;
                availableNames = new[] { "None" };
                return;
            }

            var path = AssetDatabase.GetAssetPath(module.actionsAsset);
            availableReferences = string.IsNullOrEmpty(path)
                ? null
                : AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<InputActionReference>()
                    .Where(reference => reference.action != null)
                    .OrderBy(reference => reference.name)
                    .ToArray();
            availableNames = new[] { "None" }.Concat(
                availableReferences?.Select(GetDisplayName) ?? Enumerable.Empty<string>())
                .ToArray();
        }

        private static string GetDisplayName(InputActionReference reference) {
            var action = reference.action;
            var mapName = action.actionMap != null ? action.actionMap.name + "\uFF0F" : string.Empty;
            return mapName + action.name;
        }

        private void AssignDefaultAssetIfNeeded() {
            var module = (FruityInterfaceInputModule)target;
            if (module.actionsAsset != null) return;

            var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(DEFAULT_ACTIONS_PATH);
            if (asset == null) return;

            module.actionsAsset = asset;
            module.AssignActionsFromAsset();
            EditorUtility.SetDirty(module);
        }

    }

}
#endif
