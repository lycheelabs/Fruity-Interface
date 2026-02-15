using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Parameters passed to ClickTarget.MouseClick().
    /// Contains information about the click event.
    /// </summary>
    public struct ClickParams {

        public static readonly ClickParams blank = new ClickParams(null, Vector2.zero, MouseButton.None);

        private readonly ClickTarget target;
        private readonly Vector3 mouseWorldPosition;
        private readonly MouseButton clickButton;
        
        /// <summary>How long the mouse button was held before release (seconds).</summary>
        public float HeldDuration;

        public ClickParams(ClickTarget target, Vector3 mouseWorldPosition, MouseButton clickButton) {
            this.target = target;
            this.mouseWorldPosition = mouseWorldPosition;
            this.clickButton = clickButton;
            this.HeldDuration = 0;
        }

        /// <summary>The target that was clicked.</summary>
        public ClickTarget Target => target;
        
        /// <summary>Which mouse button was used for the click.</summary>
        public MouseButton ClickButton => clickButton;
        
        /// <summary>Mouse position projected onto the world plane when clicked.</summary>
        public Vector3 MouseWorldPosition => mouseWorldPosition;
        
        /// <summary>Mouse position in screen/UI coordinates when clicked.</summary>
        public Vector2 MouseUIPosition => FruityUI.WorldPointToScreenPoint(mouseWorldPosition);

    }

}