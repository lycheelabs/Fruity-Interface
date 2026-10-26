using LycheeLabs.FruityInterface.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(Image))]
    public class ShadowBackgroundNode : BackgroundNode {

        private Image shadow;

        protected override bool WaitForFullTransition => false;

        protected override void OnInitialise() {
            shadow = GetComponent<Image>();
        }

        protected override void ApplyTween(float tween) {
            if (shadow == null) return;

            shadow.color = new Color(0, 0, 0, Tweens.EaseInOutQuad(tween) * maxEntry);
            shadow.enabled = tween > 0;
        }

    }

}
