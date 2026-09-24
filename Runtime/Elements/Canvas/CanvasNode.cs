using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasNode : InterfaceNode {

        public static CanvasNode Spawn (string name, Camera camera, float planeDistance = 1f) {
            var instance = FruityUIPrefabs.Canvas.Instantiate();
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
            var targetCamera = camera ?? Camera.main;
            canvas.worldCamera = targetCamera;
            canvas.planeDistance = targetCamera.nearClipPlane + planeDistance;
            return this;
        }

        public CanvasNode Reconfigure (float nearPlaneDist, int interfaceLayer, string sortingLayer, int sortingOrder = 0) {
            canvas.planeDistance = canvas.worldCamera.nearClipPlane + nearPlaneDist;
            return SetInterfaceLayer(interfaceLayer)
                .SetSorting(sortingLayer, sortingOrder);
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
            contents.sizeDelta = FruityUI.ScreenBounds.BoxedCanvasSize;
            contents.localScale = Vector3.one / FruityUI.ScreenBounds.UIScaling;
        }

    }
    
}
