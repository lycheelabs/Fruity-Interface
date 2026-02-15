using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Parameters passed to MouseTarget.MouseHovering().
    /// Contains information about the current hover state.
    /// </summary>
    public struct HighlightParams {

        public static readonly HighlightParams blank = new HighlightParams(null, null, Vector3.zero, MouseButton.None);

        /// <summary>The mouse button currently held, or None.</summary>
        public MouseButton HeldButton;
        
        /// <summary>Current mouse position projected onto the world plane.</summary>
        public Vector3 MouseWorldPosition;
        
        /// <summary>
        /// The target receiving this highlight event.
        /// During a drag, this is the dragged target.
        /// </summary>
        public MouseTarget Target;
        
        /// <summary>The InterfaceNode associated with the target, if any.</summary>
        public InterfaceNode Node;

        public HighlightParams(MouseTarget target, InterfaceNode node, Vector3 mouseWorldPosition, MouseButton heldButton) {
            Target = target;
            Node = node;
            MouseWorldPosition = mouseWorldPosition;
            HeldButton = heldButton;
        }

        /// <summary>Current mouse position in screen/UI coordinates.</summary>
        public Vector2 MouseUIPosition => FruityUI.WorldPointToScreenPoint(MouseWorldPosition);

    }

}