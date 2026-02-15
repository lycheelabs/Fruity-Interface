using UnityEngine;

namespace LycheeLabs.FruityInterface  {
    public struct MousePress {

        public bool isPressed;
        public bool pressIsClick;
        public bool pressIsDrag;

        public bool isLatchedDrag;
        public DragTarget.DragMode dragMode;
        public int pressStartFrame;
        
        public MouseTarget target;
        public MouseButton button;

        public Vector3 pressPosition;

        public void StartClick(MouseTarget pressedTarget, MouseButton pressedButton) {
            isPressed = true;
            target = pressedTarget;
            pressIsClick = true;
            button = pressedButton;
            dragMode = DragTarget.DragMode.Disabled;
            isLatchedDrag = false;
            pressStartFrame = Time.frameCount;
        }

        public void StartDrag(MouseTarget pressedTarget, MouseButton pressedButton, DragTarget.DragMode mode) {
            isPressed = true;
            target = pressedTarget;
            pressIsDrag = true;
            button = pressedButton;
            dragMode = mode;
            isLatchedDrag = (mode == DragTarget.DragMode.Grab);
            pressStartFrame = Time.frameCount;
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
            isLatchedDrag = false;
            pressStartFrame = 0;
        }

    }
}