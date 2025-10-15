using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public struct HighlightParams {

        public static readonly HighlightParams blank = new HighlightParams(null, null, Vector3.zero, MouseButton.None);

        public MouseButton HeldButton;
        public Vector3 MouseWorldPosition;
        public MouseTarget Target;
        public InterfaceNode Node;

        public HighlightParams (MouseTarget target, InterfaceNode node, Vector3 mouseWorldPosition, MouseButton heldButton) {
            Target = target;
            Node = node;
            MouseWorldPosition = mouseWorldPosition;
            HeldButton = heldButton;
        }

        public Vector2 MouseUIPosition => FruityUI.WorldPointToScreenPoint(MouseWorldPosition);

    }

}