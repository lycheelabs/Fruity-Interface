using UnityEngine;

namespace LycheeLabs.FruityInterface  {

    /// <summary>
    /// Tracks the state of an active mouse press (click or drag).
    /// </summary>
    public struct MousePress {

        /// <summary>
        /// Maximum duration for a click to be considered "short" and convert to pickup mode.
        /// </summary>
        public const float MAX_CLICK_DURATION = 0.4f;

        /// <summary>
        /// Divisor for calculating the minimum drag distance threshold.
        /// </summary>
        private const float DRAG_DISTANCE_DIVISOR = 20f;

        // Press state
        public bool isPressed;
        public bool pressIsClick;
        public bool pressIsDrag;

        // Drag-specific state
        public bool isPickUpDrag;
        public DragTarget.DragMode dragMode;

        // Timing
        public int pressStartFrame;
        public float pressStartTime;
        
        // Target and button
        public MouseTarget target;
        public MouseButton button;

        // Position tracking
        public Vector3 pressWorldPosition;
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

        public void StartDrag(MouseTarget pressedTarget, MouseButton pressedButton, DragTarget.DragMode mode, Vector3 worldPosition, Vector2 screenPosition) {
            isPressed = true;
            target = pressedTarget;
            pressIsDrag = true;
            button = pressedButton;
            dragMode = mode;
            // Only PickUpOnly starts as latched pickup
            // DragOrPickUp starts as normal drag, may convert to pickup on mouse-up if it was a short click
            isPickUpDrag = (mode == DragTarget.DragMode.PickUpOnly);
            pressStartFrame = Time.frameCount;
            pressStartTime = Time.unscaledTime;
            pressWorldPosition = worldPosition;
            pressScreenPosition = screenPosition;
        }

        /// <summary>
        /// Convert a DragOrPickUp drag into pickup mode (called when mouse released after a short click).
        /// </summary>
        public void ConvertToPickUp() {
            isPickUpDrag = true;
        }

        /// <summary>
        /// Determines if the drag was a "real" drag (held long enough or moved far enough)
        /// versus a short click that should convert to pickup mode.
        /// </summary>
        public bool WasRealDrag(Vector2 currentScreenPosition) {
            var duration = Time.unscaledTime - pressStartTime;
            var distance = (currentScreenPosition - pressScreenPosition).magnitude;
            var threshold = ScreenBounds.BoxedCanvasSize.y / DRAG_DISTANCE_DIVISOR;
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
            pressWorldPosition = Vector3.zero;
            pressScreenPosition = Vector2.zero;
        }

    }
}