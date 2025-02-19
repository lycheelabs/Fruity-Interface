using UnityEngine;
using System.Collections;
using UnityEditor.Rendering;

namespace LycheeLabs.FruityInterface {

    public struct HighlightParams {

        public static readonly HighlightParams blank = new HighlightParams(null, null, Vector3.zero, MouseButton.None);

        private Camera camera;
        public MouseButton HeldButton;
        public Vector3 MouseWorldPosition;
        public MouseTarget Target;

        public HighlightParams (Camera camera, MouseTarget target, Vector3 mouseWorldPosition, MouseButton heldButton) {
            this.camera = camera;
            Target = target;
            MouseWorldPosition = mouseWorldPosition;
            HeldButton = heldButton;
        }

        public Vector2 MouseUIPosition => new ScreenPosition(MouseWorldPosition).ScreenVector(camera);

    }

}