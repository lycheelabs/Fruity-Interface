using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public interface FullscreenButtonCallbacks {
		void OnFullscreenClick (ClickParams clickParams);
    }

	[RequireComponent(typeof(BoxCollider))]
    public class FullscreenButtonNode : InterfaceNode, ClickTarget {

		public static FullscreenButtonNode Spawn (InterfaceNode parent, FullscreenButtonCallbacks callbacks) {
			var instance = FruityUIPrefabs.NewFullscreenButton().GetComponent<FullscreenButtonNode>();
			instance.transform.SetParent(parent?.transform, false);
			instance.InputParentOverride = parent;
			instance.Callbacks = callbacks;
			return instance;
		}

		// --------------------------------------------------------

		private FullscreenButtonCallbacks Callbacks;
		private new BoxCollider collider;

        private void Awake () {
			collider = GetComponent<BoxCollider>();
		}

		private void Update () {
			var canvasSize = ScreenBounds.BoxedCanvasSize;
			collider.size = new Vector3(canvasSize.x, canvasSize.y, 1);
		}

        public void ApplyMouseClick (ClickParams clickParams) {
			Callbacks?.OnFullscreenClick(clickParams);
        }

        public void UpdateMouseHover (bool firstFrame, HoverParams highlightParams) {}
        public void EndMouseHover () {}

        public void Disable () {
			collider.enabled = false;
		}

    }

}