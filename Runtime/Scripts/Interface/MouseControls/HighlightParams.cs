using UnityEngine;
using System.Collections;
using UnityEditor.Rendering;

namespace LycheeLabs.FruityInterface {

    public struct HighlightParams {

        public static readonly HighlightParams blank = new HighlightParams(null, Vector3.zero, MouseButton.None);

        public MouseButton HeldButton;
        public Vector3 MouseWorldPosition;
        public MouseTarget Target;

        public HighlightParams (MouseTarget target, Vector3 mouseWorldPosition, MouseButton heldButton) {
            Target = target;
            MouseWorldPosition = mouseWorldPosition;
            HeldButton = heldButton;
        }

        public Vector2 MouseUIPosition => FruityUI.WorldPointToScreenPoint(MouseWorldPosition);

    }

}