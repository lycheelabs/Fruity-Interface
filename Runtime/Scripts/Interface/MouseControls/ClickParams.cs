using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

    public struct ClickParams {

        public static readonly ClickParams blank = new ClickParams(null, Vector2.zero, MouseButton.None);

        private MouseButton clickButton;
        private Vector3 mouseWorldPosition;
        private ClickTarget target;
        public float HeldDuration;

        public ClickParams (ClickTarget target, Vector3 mouseWorldPosition, MouseButton clickButton) {
            this.target = target;
            this.mouseWorldPosition = mouseWorldPosition;
            this.clickButton = clickButton;
            this.HeldDuration = 0;
        }

        public Vector3 MouseWorldPosition => mouseWorldPosition;
        public Vector2 MouseUIPosition => FruityUI.WorldPointToScreenPoint(mouseWorldPosition);

        public MouseButton ClickButton => clickButton; 
        public ClickTarget Target => target; 

    }

}