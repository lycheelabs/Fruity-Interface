using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Contains a set of prefabs used for basic UI setup.
    /// </summary>
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
        private const string CANVAS_FOLDER = "CanvasPrefabs/";

        // ----------------------------------------------------------------

        private static readonly GameObject canvas = Load(CANVAS_FOLDER, "CanvasNode");
        
        private static readonly GameObject fullscreenLetterbox = Load(CANVAS_FOLDER, "FullscreenLetterboxNode");
        private static readonly GameObject fullscreenShadow = Load(CANVAS_FOLDER, "FullscreenShadowNode");
        private static readonly GameObject fullscreenButton = Load(CANVAS_FOLDER, "FullscreenButtonNode");

        public static GameObject NewCanvasNode () {
            return Object.Instantiate(canvas);
        }

        public static GameObject NewFullscreenLetterbox () {
            return Object.Instantiate(fullscreenLetterbox);
        }

        public static GameObject NewFullscreenShadow () {
            return Object.Instantiate(fullscreenShadow);
        }

        public static GameObject NewFullscreenButton () {
            return Object.Instantiate(fullscreenButton);
        }

    }

}