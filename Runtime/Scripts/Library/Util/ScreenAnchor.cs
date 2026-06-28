using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Stores a locked screen position and screen offset. Resolves to world coordinates dynamically
    /// via a camera each time. Call PinWorld() to reverse-lock the current world position as a WorldAnchor.
    /// Created via WorldAnchor.PinScreen() or ScreenAnchor.Lerp().
    /// </summary>
    public struct ScreenAnchor {

        public static ScreenAnchor Lerp(ScreenAnchor a, ScreenAnchor b, float tween, Vector2 screenOffset = default) {
            var lerpScreen = Vector3.Lerp(a.screenPosition, b.screenPosition, tween);
            var lerpOffset = Vector2.Lerp(a.offset, b.offset, tween);
            return new ScreenAnchor(lerpScreen, lerpOffset + screenOffset);
        }

        private Vector3 screenPosition;
        private Vector2 offset;

        internal ScreenAnchor(Vector3 screenPosition, Vector2 offset) {
            this.screenPosition = screenPosition;
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
