using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public interface FullscreenButtonCallbacks {
		void OnFullscreenClick (ClickParams clickParams);
    }

	[RequireComponent(typeof(BoxCollider))]
    public class UIFullscreenButton : InterfaceNode, ClickTarget {

		public static UIFullscreenButton Spawn (InterfaceNode parent, FullscreenButtonCallbacks callbacks) {
			var instance = FruityUIPrefabs.NewUIFullscreenButton().GetComponent<UIFullscreenButton>();
			instance.transform.SetParent(parent?.transform, false);
			instance.InputParent = parent;
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

        public void MouseClick (ClickParams clickParams) {
			Callbacks?.OnFullscreenClick(clickParams);
        }

        public void MouseHovering (bool firstFrame, HighlightParams highlightParams) {}
        public void MouseHoverEnd () {}

        public void Disable () {
			collider.enabled = false;
		}

    }

}