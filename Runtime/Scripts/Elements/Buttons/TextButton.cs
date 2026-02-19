using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a button using the TextButton prefab.
    /// </summary>
    public class TextButton : ButtonNode {

        public Image BackingImage;
        public TextMeshProUGUI ButtonText;
        public Image IconImage;
        public BoxCollider BoxCollider;

        [SerializeField] private float height = 50;
        [SerializeField] private float maxWidth = 200;
        [SerializeField] private bool cropWidth = false;
        [SerializeField] private Sprite iconSprite = null;

        [Range(0f, 2f)] [SerializeField] private float fontHeightScaling = 1f;
        [Range(-1f, 1f)][SerializeField] private float fontHeightShift = 0f;

        private void OnEnable () {
            RefreshLayoutDeferred();
        }

        public void SetText (string text) {
            ButtonText.text = text;
            RefreshLayoutDeferred();
        }

        public void SetWidth (float maxWidth, bool crop = false) {
            this.maxWidth = maxWidth;
            this.cropWidth = crop;
            RefreshLayoutDeferred();
        }

        public void SetHeight (float height) {
            this.height = height;
            SetFontScale(fontHeightScaling);
            RefreshLayoutDeferred();
        }

        public void SetFontScale (float heightScale) {
            this.fontHeightScaling = heightScale;
            RefreshLayoutDeferred();
        }

        protected override void AnimateHover (float highlightTween, float heldTween) {
            /*var scaleShift = 0.03f * Tweens.EaseOutQuad(highlightTween) - 0.05f * Tweens.EaseOutQuad(heldTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.OverlayScale(Vector3.one * highlightScale * BaseScale);*/

            var scaleShift = 0.03f * Tweens.EaseOutQuad(highlightTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.OverlayScale(Vector3.one * highlightScale * BaseScale);

            var heldShift = new Vector3(0, -Tweens.EaseOutQuad(heldTween), 0) *10 * AnimationScaling;
            ButtonAnimator.OverlayPosition(heldShift * BaseScale);
        }

        protected override void AnimateClick () {
            ButtonAnimator.Squash(2.5f * AnimationScaling);
        }

        // ---------------------------------------------

        protected override void RefreshLayout() {

            // Override sizes
            if (LayoutDriver != null && LayoutDriver.isActiveAndEnabled) {
                maxWidth = LayoutDriver.width;
                height = LayoutDriver.height;
                cropWidth = LayoutDriver.cropWidth;
                LayoutPaddingPixels = LayoutDriver.layoutPadding;
                fontHeightScaling = LayoutDriver.fontHeightScaling;
            }

            // Update icon
            var hasIcon = iconSprite != null;
            IconImage.sprite = iconSprite;
            IconImage.gameObject.SetActive(hasIcon);

            // Calculate sizes
            var textWidth = ButtonText.preferredWidth;
            if (hasIcon) textWidth += height;
            var width = Mathf.Min(maxWidth, textWidth + 32);
            if (!cropWidth) width = maxWidth;
            var size = new Vector2(width, height);

            // Apply sizes
            LayoutSizePixels = size;
            rectTransform.sizeDelta = LayoutSizePixels;

            BoxCollider.size = LayoutSizePixels;
            IconImage.rectTransform.sizeDelta = new Vector2(height, height) * 0.8f;
            IconImage.rectTransform.localPosition = new Vector3(height * 0.5f, 0);

            ButtonText.fontSizeMax = height * 0.7f * fontHeightScaling;
            ButtonText.fontSizeMin = height * 0.4f * fontHeightScaling;

            var offsetMin = ButtonText.rectTransform.offsetMin;
            if (hasIcon) {
                ButtonText.rectTransform.offsetMin = new Vector2(height + 4, offsetMin.y);
            } else {
                ButtonText.rectTransform.offsetMin = new Vector2(16, offsetMin.y);
            }

        }

    }

}