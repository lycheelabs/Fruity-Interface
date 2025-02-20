using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

    public struct ClickParams {

        public static readonly ClickParams blank = new ClickParams(null, null, Vector2.zero, MouseButton.None);

        private Camera camera;
        private MouseButton clickButton;
        private Vector3 mouseWorldPosition;
        private ClickTarget target;
        public float HeldDuration;

        public ClickParams (Camera camera, ClickTarget target, Vector3 mouseWorldPosition, MouseButton clickButton) {
            this.camera = camera;
            this.target = target;
            this.mouseWorldPosition = mouseWorldPosition;
            this.clickButton = clickButton;
            this.HeldDuration = 0;
        }

        public Vector3 MouseWorldPosition => mouseWorldPosition;
        public Vector2 MouseUIPosition => new ScreenPosition(mouseWorldPosition).ScreenVector(camera);

        public MouseButton ClickButton => clickButton; 
        public ClickTarget Target => target; 

    }

}