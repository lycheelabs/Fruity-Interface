using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a button using the IconButton prefab.
    /// </summary>
    public class IconButton : ButtonNode {

        public Image ButtonImage;
        public BoxCollider BoxCollider;

        [SerializeField] private Sprite sprite = null;
        [SerializeField] private Vector2 size = new Vector2(50, 50);
        [SerializeField] private float colliderPadding = 10;
        [SerializeField] private float iconScaling = 1;

        private bool inputDisabled;

        private void OnValidate () {
            RefreshLayoutDeferred();
        }

        private void OnEnable () {
            RefreshLayoutDeferred();
        }

        protected override void AnimateHover (float highlightTween, float heldTween) {
            var scaleShift = 0.1f * Tweens.EaseOutQuad(highlightTween) - 0.14f * Tweens.EaseOutQuad(heldTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.OverlayScale(Vector3.one * highlightScale * BaseScale);
        }

        protected override void AnimateClick () {
            ButtonAnimator.Squash(6f * AnimationScaling);
        }

        public void ConfigureSprite(Sprite sprite) {
            this.sprite = sprite;
            ButtonImage.sprite = sprite;
            RefreshLayoutDeferred();
        }

        protected override void RefreshLayout () {            
            
            // Override sizes
            if (LayoutDriver != null && LayoutDriver.isActiveAndEnabled) {
                size = new Vector2(LayoutDriver.height, LayoutDriver.height);
                iconScaling = LayoutDriver.iconScaling;
                LayoutPaddingPixels = LayoutDriver.layoutPadding;
            }

            rectTransform.sizeDelta = size;
            ButtonImage.rectTransform.sizeDelta = size * iconScaling;
            LayoutSizePixels = size;
            BoxCollider.size = (size * iconScaling) + new Vector2(colliderPadding, colliderPadding);
        }

        public void SetInputDisabled (bool disabled) {
            inputDisabled = disabled;
        }

        public override bool InputIsDisabled => inputDisabled;

    }

}