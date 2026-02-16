using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Parameters passed to MouseTarget.MouseHovering().
    /// Contains information about the current hover state.
    /// </summary>
    public struct HoverParams {

        public static readonly HoverParams blank = new HoverParams(null, Vector3.zero, MouseButton.None);

        /// <summary>
        /// The mouse button for an active press that originated on this target, or None.
        /// This is None if the user pressed elsewhere then hovered over this target.
        /// Use this to detect if the user is holding down a button on your UI element.
        /// </summary>
        public MouseButton PressButton;
        
        /// <summary>Current mouse position projected onto the world plane.</summary>
        public Vector3 MouseWorldPosition;
        
        /// <summary>The InterfaceNode associated with the target, if any.</summary>
        public InterfaceNode Node;

        public HoverParams(InterfaceNode node, Vector3 mouseWorldPosition, MouseButton pressButton) {
            Node = node;
            MouseWorldPosition = mouseWorldPosition;
            PressButton = pressButton;
        }

        /// <summary>Current mouse position in screen/UI coordinates.</summary>
        public Vector2 MouseUIPosition => FruityUI.WorldPointToScreenPoint(MouseWorldPosition);

    }

}