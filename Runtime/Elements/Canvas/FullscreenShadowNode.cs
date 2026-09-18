using LycheeLabs.FruityInterface.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Image))]
    public class FullscreenShadowNode : InterfaceNode {

		public static FullscreenShadowNode Instantiate (Transform parent) {
            var instance = FruityUIPrefabs.FullscreenShadow.Instantiate();
			instance.transform.SetParent(parent, false);
			return instance;
		}

		public float TargetAlpha = 0.85f;

		private Image shadow;
		private bool active;
		private float tween;

		public bool IsActive => tween > 0;

        private void Awake () {
			shadow = GetComponent<Image>();
		}

		public void Show() {
			active = true;
		}

		public void Hide(bool immediate = false) {
			active = false;
			if (immediate) {
				tween = 0;
				ApplyVisualState();
			}
		}

		public void SetShadowActive(bool active) {
			if (active) Show();
			else Hide();
		}

		private void Update () {
			tween = tween.MoveTowardsUnscaled(active, 8);
			ApplyVisualState();
		}

		private void ApplyVisualState() {
			shadow.color = new Color(0, 0, 0, tween * TargetAlpha);
			shadow.enabled = tween > 0;
		}

	}

}
