using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Stores a locked viewport position and screen offset. Resolves to current screen coordinates
    /// dynamically, so the position adapts when the screen is resized. Call PinWorld() to
    /// reverse-lock the current world position as a WorldAnchor.
    /// Created via WorldAnchor.PinScreen() or ScreenAnchor.Lerp().
    /// </summary>
    public struct ScreenAnchor {

        public static ScreenAnchor Lerp(ScreenAnchor a, ScreenAnchor b, float tween, Vector2 screenOffset = default) {
            var lerpScreen = Vector3.Lerp(a.viewportPosition, b.viewportPosition, tween);
            var lerpOffset = Vector2.Lerp(a.offset, b.offset, tween);
            return new ScreenAnchor {
                viewportPosition = lerpScreen,
                offset = lerpOffset + screenOffset
            };
        }

        private Vector3 viewportPosition;
        private Vector2 offset;

        internal ScreenAnchor(Vector3 screenPosition, Vector2 offset) {
            viewportPosition = new Vector3(
                screenPosition.x / Mathf.Max(Screen.width, 1),
                screenPosition.y / Mathf.Max(Screen.height, 1),
                screenPosition.z
            );
            this.offset = offset;
        }

        public WorldAnchor PinWorld() => PinWorld(FruityUI.UICamera);

        public WorldAnchor PinWorld(Camera camera) {
            var adjusted = ScreenToAdjusted();
            var worldPos = camera.ScreenToWorldPoint(adjusted);
            return new WorldAnchor(worldPos.IntersectWithWorldPlane());
        }

        public Vector3 WorldVector() => WorldVector(FruityUI.UICamera);

        public Vector3 WorldVector(Camera camera) {
            var adjusted = ScreenToAdjusted();
            return camera.ScreenToWorldPoint(adjusted).IntersectWithWorldPlane();
        }

        public Vector3 ScreenVector() {
            var canvasVector = RawScreenVector() * FruityUI.ScreenBounds.UIScaling;
            return canvasVector + (Vector3)(-FruityUI.ScreenBounds.WindowCanvasSize / 2f);
        }

        public Vector3 RawScreenVector() {
            var screenPosition = new Vector3(
                viewportPosition.x * Screen.width,
                viewportPosition.y * Screen.height,
                viewportPosition.z
            );
            return screenPosition + (Vector3)offset / FruityUI.ScreenBounds.UIScaling;
        }

        public Vector3 RawViewportVector() {
            var rawVector = RawScreenVector();
            return new Vector2(rawVector.x / Screen.width, rawVector.y / Screen.height);
        }

        private Vector3 ScreenToAdjusted() {
            var screenVector = ScreenVector();
            return (screenVector / FruityUI.ScreenBounds.UIScaling) + new Vector3(Screen.width, Screen.height) / 2f;
        }

    }

}
