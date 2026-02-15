using UnityEngine;

namespace LycheeLabs.FruityInterface  {
    public struct MousePress {

        // Clicks lasting longer than this will be counted as drags (for DragOrPickUp mode)
        public const float MAX_CLICK_DURATION = 0.4f;

        public bool isPressed;
        public bool pressIsClick;
        public bool pressIsDrag;

        public bool isPickUpDrag;
        public DragTarget.DragMode dragMode;
        public int pressStartFrame;
        public float pressStartTime;
        
        public MouseTarget target;
        public MouseButton button;

        public Vector3 pressPosition;
        public Vector2 pressScreenPosition;

        public void StartClick(MouseTarget pressedTarget, MouseButton pressedButton) {
            isPressed = true;
            target = pressedTarget;
            pressIsClick = true;
            button = pressedButton;
            dragMode = DragTarget.DragMode.Disabled;
            isPickUpDrag = false;
            pressStartFrame = Time.frameCount;
            pressStartTime = Time.unscaledTime;
        }

        public void StartDrag(MouseTarget pressedTarget, MouseButton pressedButton, DragTarget.DragMode mode, Vector2 screenPosition) {
            isPressed = true;
            target = pressedTarget;
            pressIsDrag = true;
            button = pressedButton;
            dragMode = mode;
            // Only PickUpOnly starts as latched
            // DragOrPickUp starts as normal drag, may convert to pickup on mouse-up
            isPickUpDrag = (mode == DragTarget.DragMode.PickUpOnly);
            pressStartFrame = Time.frameCount;
            pressStartTime = Time.unscaledTime;
            pressScreenPosition = screenPosition;
        }

        public void ConvertToPickUp() {
            isPickUpDrag = true;
        }

        public bool WasRealDrag(Vector2 currentScreenPosition) {
            var duration = Time.unscaledTime - pressStartTime;
            var distance = (currentScreenPosition - pressScreenPosition).magnitude;
            var threshold = ScreenBounds.BoxedCanvasSize.y / 20;
            return duration > MAX_CLICK_DURATION || distance > threshold;
        }

        public void ReleaseClick() {
            pressIsClick = false;
        }

        public void Clear() {
            isPressed = false;
            target = null;
            pressIsClick = false;
            pressIsDrag = false;
            button = MouseButton.None;
            dragMode = DragTarget.DragMode.Disabled;
            isPickUpDrag = false;
            pressStartFrame = 0;
            pressStartTime = 0;
            pressScreenPosition = Vector2.zero;
        }

    }
}