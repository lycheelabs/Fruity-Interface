using UnityEngine;

namespace LycheeLabs.FruityInterface  {
    public struct MousePress {

        public bool isPressed;
        public bool pressIsClick;
        public bool pressIsDrag;
        
        public MouseTarget target;
        public MouseButton button;

        public Vector3 pressPosition;

        public void StartClick(MouseTarget pressedTarget, MouseButton pressedButton) {
            isPressed = true;
            target = pressedTarget;
            pressIsClick = true;
            button = pressedButton;
        }

        public void StartDrag(MouseTarget pressedTarget, MouseButton pressedButton) {
            isPressed = true;
            target = pressedTarget;
            pressIsDrag = true;
            button = pressedButton;
        }

        public void ReleaseClick() {
            pressIsClick = false;
        }

        public void Clear() {
            isPressed = false;
            target = null;
            pressIsClick = false;
            pressIsDrag = false;
            target = null;
            button = MouseButton.None;
        }

    }
}