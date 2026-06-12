using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasNode : InterfaceNode {

        public static CanvasNode Spawn (string name, Camera camera, float planeDistance) {
            var instance = FruityUIPrefabs.NewCanvasNode().GetComponent<CanvasNode>();
            instance.name = "Canvas-" + name;
            instance.SetCamera(camera, planeDistance);
            return instance;
        }

        // -------------------------------------------------

        public Canvas canvas;
        public InterfaceLayer layer;
        public RectTransform contents;
        protected override Transform AttachTarget => contents.transform;

        public CanvasNode SetCamera (Camera camera, float planeDistance) {
            canvas.worldCamera = camera ?? Camera.main;
            canvas.planeDistance = planeDistance + 5;
            return this;
        }

        public CanvasNode SetInterfaceLayer (int interfaceLayer) {
            layer.Layer = interfaceLayer;
            return this;
        }

        public CanvasNode SetSorting (string sortingLayerName, int sortingOrder = 0) {
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
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