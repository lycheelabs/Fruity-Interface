using LycheeLabs.InputTree.Elements;
using UnityEngine;

namespace LycheeLabs.InputTree {

    /// <summary>
    /// Contains the canvas prefabs needed for basic UI setup.
    /// </summary>
    public sealed class FruityUIPrefabs {

        private const string CANVAS_PATH = "FruityPrefabs/CanvasPrefabs/";

        // ----------------------------------------------------------------

        public static CanvasNode InstantiateCanvas () {
            return UnityEngine.Object.Instantiate(Resources.Load<CanvasNode>(CANVAS_PATH + "CanvasNode"));
        }

        public static FullscreenLetterboxNode InstantiateFullscreenLetterbox () {
            return UnityEngine.Object.Instantiate(Resources.Load<FullscreenLetterboxNode>(CANVAS_PATH + "FullscreenLetterboxNode"));
        }

        public static GameObject NewCanvasNode () {
            return InstantiateCanvas().gameObject;
        }

        public static GameObject NewFullscreenLetterbox () {
            return InstantiateFullscreenLetterbox().gameObject;
        }

    }

}
