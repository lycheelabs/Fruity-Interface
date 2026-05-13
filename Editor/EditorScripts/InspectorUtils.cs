using UnityEditor;
using UnityEngine;

namespace LycheeLabs.FruityHelpers {

    public static class InspectorUtils {

        // ── Styles ────────────────────────────────────────────────────────────

        private static GUIStyle largeTextStyle;
        public static GUIStyle LargeTextStyle {
            get {
                if (largeTextStyle == null) {
                    largeTextStyle = new GUIStyle(EditorStyles.wordWrappedLabel) {
                        fontSize = 11,
                        fontStyle = FontStyle.Normal
                    };
                }
                return largeTextStyle;
            }
        }

        private static GUIStyle labelStyle;
        public static GUIStyle LabelStyle {
            get {
                if (labelStyle == null) {
                    labelStyle = new GUIStyle(EditorStyles.boldLabel) {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold
                    };
                }
                return labelStyle;
            }
        }

        // ── Layout helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Draws a thin horizontal separator line, matching the Unity Inspector style.
        /// </summary>
        public static void DrawLine () {
            EditorGUILayout.Space(2);
            var rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            EditorGUILayout.Space(2);
        }

    }

}