using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Stores a world position and screen offset. Resolves to screen coordinates dynamically
    /// via a camera each time. Call PinScreen() to lock the current screen position as a ScreenAnchor.
    /// </summary>
    public struct WorldAnchor {

        public static WorldAnchor FromCamera(Vector2 screenOffset, Camera camera = null) {
            camera = camera ?? FruityUI.UICamera;
            var cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height) / 2f);
            return new WorldAnchor(cameraCenter, screenOffset);
        }

        private Vector3 position;
        private Vector2 offset;

        public WorldAnchor(Vector3 worldPosition) {
            position = worldPosition;
            offset = default;
        }

        public WorldAnchor(Vector3 worldPosition, Vector2 screenOffset) : this(worldPosition) {
            offset = screenOffset;
        }

        public ScreenAnchor PinScreen() => PinScreen(FruityUI.UICamera);

        public ScreenAnchor PinScreen(Camera camera) {
            return new ScreenAnchor(camera.WorldToScreenPoint(position), offset);
        }

        public Vector3 ScreenVector() => ScreenVector(FruityUI.UICamera);

        public Vector3 ScreenVector(Camera camera) {
            var canvasVector = RawScreenVector(camera) * ScreenBounds.UIScaling;
            var screenOffset = (Vector3)(-ScreenBounds.WindowCanvasSize / 2f);
            return canvasVector + screenOffset;
        }

        public Vector3 RawScreenVector() => RawScreenVector(FruityUI.UICamera);

        public Vector3 RawScreenVector(Camera camera) {
            if (camera == null) {
                Debug.LogWarning("Resolving a WorldAnchor requires a Camera");
                return default;
            }
            return camera.WorldToScreenPoint(position) + (Vector3)offset / ScreenBounds.UIScaling;
        }

        public Vector3 RawViewportVector() => RawViewportVector(FruityUI.UICamera);

        public Vector2 RawViewportVector(Camera camera) {
            var rawVector = RawScreenVector(camera);
            return new Vector2(rawVector.x / Screen.width, rawVector.y / Screen.height);
        }

        public Vector3 WorldVector() => WorldVector(FruityUI.UICamera);

        public Vector3 WorldVector(Camera camera) {
            var screenVector = ScreenVector(camera);
            var adjusted = (screenVector / ScreenBounds.UIScaling) + new Vector3(Screen.width, Screen.height) / 2f;
            return camera.ScreenToWorldPoint(adjusted).IntersectWithWorldPlane();
        }

    }

}
