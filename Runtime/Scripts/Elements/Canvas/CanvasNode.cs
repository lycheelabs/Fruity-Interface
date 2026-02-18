using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasNode : InterfaceNode {

        public static CanvasNode Spawn (string name, string layerName, int order, float planeDistance, Camera camera = null) {
            var instance = FruityUIPrefabs.NewCanvasNode().GetComponent<CanvasNode>();
            instance.Setup(name, camera, layerName, order, planeDistance);
            return instance;
        }

        // -------------------------------------------------

        public Canvas canvas;
        public RectTransform contents;
        protected override Transform AttachTarget => contents.transform;

        public void Setup (string name, Camera camera, string layerName, int order, float planeDistance) {
            canvas.name = "Canvas-" + name;
            canvas.worldCamera = camera ?? Camera.main;
            canvas.sortingLayerName = layerName;
            canvas.sortingOrder = order;
            canvas.planeDistance = planeDistance + 5;
        }

        public CanvasNode SetLayer (string newLayerName) {
            canvas.sortingLayerName = newLayerName;
            return this;
        }

        public CanvasNode AddLetterbox () {
            FullscreenLetterboxNode.Instantiate(canvas.transform);
            return this;
        }

        private void Update () {
            contents.sizeDelta = ScreenBounds.BoxedCanvasSize;
            contents.localScale = Vector3.one / ScreenBounds.UIScaling;
        }

    }
    
}