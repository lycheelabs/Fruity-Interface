using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Image))]
    public class FullscreenShadowNode : InterfaceNode {

		public static FullscreenShadowNode Instantiate (Transform parent) {
			var instance = FruityUIPrefabs.NewFullscreenShadow().GetComponent<FullscreenShadowNode>();
			instance.transform.SetParent(parent, false);
			return instance;
		}

		public float TargetAlpha = 0.85f;

		private Image shadow;
		private bool active;
		private float tween = 0;

        private void Awake () {
			shadow = GetComponent<Image>();
		}

		public void SetShadowActive(bool active) {
			this.active = active;
		}

		private void Update () {
			if (active) {
				tween = Mathf.Min(tween + Time.deltaTime * 8, TargetAlpha);
			} else {
				tween = Mathf.Max(tween - Time.deltaTime * 8, 0);
			}
			tween = tween.MoveTowards(active, 8);

			shadow.color = new Color(0, 0, 0, tween * TargetAlpha);
            shadow.enabled = (tween > 0);
		}

	}

}