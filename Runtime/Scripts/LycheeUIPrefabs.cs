using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public static class LycheeUIPrefabs {

        private const string PREFABS_PATH = "LycheeUIPrefabs/";

        public static void Initialise () { }

        public static GameObject Load (string filePath, string fileName) {
            var path = PREFABS_PATH + filePath + fileName;
            var gameObject = Resources.Load(path) as GameObject;
            if (gameObject == null) {
                throw new System.NullReferenceException("No prefab found at: " + path);
            }
            return gameObject;
        }

        // ----------------------------------------------------------------

        private const string ROOT_FOLDER = "";
        private const string BUTTON_FOLDER = "Buttons/";

        // ----------------------------------------------------------------

        private static readonly GameObject canvas = Load(ROOT_FOLDER, "UICanvas");
        private static readonly GameObject shadow = Load(ROOT_FOLDER, "UIShadow");
        private static readonly GameObject letterbox = Load(ROOT_FOLDER, "UILetterbox");
        private static readonly GameObject fullscreenButton = Load(ROOT_FOLDER, "UIFullscreenButton");
        private static readonly GameObject rect = Load(ROOT_FOLDER, "UIRect");
        private static readonly GameObject imageButton = Load(ROOT_FOLDER, "ImageButton");
        private static readonly GameObject textButton = Load(ROOT_FOLDER, "TextButton");
        private static readonly GameObject toggleButton = Load(ROOT_FOLDER, "ToggleButton");
        private static readonly GameObject verticalLayout = Load(ROOT_FOLDER, "VerticalLayout");
        private static readonly GameObject horizontalLayout = Load(ROOT_FOLDER, "HorizontalLayout");

        public static GameObject NewUICanvas () {
            return Object.Instantiate(canvas);
        }

        public static GameObject NewUIShadow () {
            return Object.Instantiate(shadow);
        }

        public static GameObject NewUILetterbox () {
            return Object.Instantiate(letterbox);
        }

        public static GameObject NewUIFullscreenButton () {
            return Object.Instantiate(fullscreenButton);
        }

        public static GameObject NewUIRect () {
            return Object.Instantiate(rect);
        }

        public static GameObject NewImageButton () {
            return Object.Instantiate(imageButton);
        }

        public static GameObject NewTextButton () {
            return Object.Instantiate(textButton);
        }

        public static GameObject NewToggleButton() {
            return Object.Instantiate(toggleButton);
        }

        public static GameObject NewVerticalLayout () {
            return Object.Instantiate(verticalLayout);
        }

        public static GameObject NewHorizontalLayout () {
            return Object.Instantiate(horizontalLayout);
        }

    }

}