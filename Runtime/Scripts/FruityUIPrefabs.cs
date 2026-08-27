using LycheeLabs.FruityInterface.Elements;
using LycheeLabs.FruityAssetLoader;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Contains a set of prefabs used for basic UI setup.
    /// </summary>
    public sealed class FruityUIPrefabs : AssetCatalog {

        private const string PREFABS_PATH = "FruityPrefabs/";

        // ----------------------------------------------------------------

        private const string CANVAS_FOLDER = "CanvasPrefabs/";
        private const string PROMPT_FOLDER = "PromptPrefabs/";

        // ----------------------------------------------------------------

        public static readonly Prefab<CanvasNode> Canvas = LoadPrefab<CanvasNode>(CANVAS_FOLDER, "CanvasNode");
        public static readonly Prefab<FullscreenLetterboxNode> FullscreenLetterbox = LoadPrefab<FullscreenLetterboxNode>(CANVAS_FOLDER, "FullscreenLetterboxNode");
        public static readonly Prefab<FullscreenShadowNode> FullscreenShadow = LoadPrefab<FullscreenShadowNode>(CANVAS_FOLDER, "FullscreenShadowNode");
        public static readonly Prefab<FullscreenButtonNode> FullscreenButton = LoadPrefab<FullscreenButtonNode>(CANVAS_FOLDER, "FullscreenButtonNode");
        public static readonly Prefab<TextTooltip> SimpleTooltip = LoadPrefab<TextTooltip>(PROMPT_FOLDER, "TextTooltip");

        private static Prefab<T> LoadPrefab<T>(string folder, string file) where T : Component {
            return new Prefab<T>(Load<GameObject>(PREFABS_PATH + folder, file));
        }

        public static GameObject NewCanvasNode () {
            return Canvas.InstantiateGameObject();
        }

        public static GameObject NewFullscreenLetterbox () {
            return FullscreenLetterbox.InstantiateGameObject();
        }

        public static GameObject NewFullscreenShadow () {
            return FullscreenShadow.InstantiateGameObject();
        }

        public static GameObject NewFullscreenButton () {
            return FullscreenButton.InstantiateGameObject();
        }

        public static GameObject NewSimpleTooltip () {
            return SimpleTooltip.InstantiateGameObject();
        }

    }

}
