using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public static class FruityUIPrefabs {

        private const string PREFABS_PATH = "FruityPrefabs/";

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

        // ----------------------------------------------------------------

        private static readonly GameObject canvas = Load(ROOT_FOLDER, "UICanvas");
        private static readonly GameObject shadow = Load(ROOT_FOLDER, "UIShadow");
        private static readonly GameObject letterbox = Load(ROOT_FOLDER, "UILetterbox");
        private static readonly GameObject fullscreenButton = Load(ROOT_FOLDER, "UIFullscreenButton");

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

    }

}