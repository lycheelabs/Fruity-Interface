using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Canvas))]
    public sealed class UICanvas : InterfaceNode {

        public static UICanvas Spawn (string name, string layerName, int order, float planeDistance, Camera camera = null) {
            var instance = FruityUIPrefabs.NewUICanvas().GetComponent<UICanvas>();
            instance.Setup(name, camera, layerName, order, planeDistance);
            return instance;
        }

        public Canvas canvas;
        public RectTransform contents;
        protected override Transform AttachTarget => contents.transform;

        public void Setup (string name, Camera camera, string layerName, int order, float planeDistance) {
            canvas.name = "Canvas-" + name;
            canvas.worldCamera = camera ?? Camera.main;
            canvas.sortingLayerName = layerName;
            canvas.sortingOrder = order;
            canvas.planeDistance = planeDistance;
        }

        public UICanvas SetLayer (string newLayerName) {
            canvas.sortingLayerName = newLayerName;
            return this;
        }

        public UICanvas AddLetterbox () {
            UILetterbox.Instantiate(canvas.transform);
            return this;
        }

        void Update () {
            contents.sizeDelta = InterfaceConfig.BoxedCanvasSize;
            contents.localScale = Vector3.one / InterfaceConfig.UIScaling;
        }

    }
    
}