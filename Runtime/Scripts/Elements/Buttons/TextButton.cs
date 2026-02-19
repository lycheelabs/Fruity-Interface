using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a ClickButton using the TextButton prefab.
    /// </summary>
    public class TextButton : ClickButton {

        public Image BackingImage;
        public TextMeshProUGUI ButtonText;
        public Image IconImage;
        public BoxCollider BoxCollider;
        public ButtonLayoutStyle layoutDriver;

        [SerializeField] private float height = 50;
        [SerializeField] private float maxWidth = 200;
        [SerializeField] private bool cropWidth = false;
        [SerializeField] private Sprite iconSprite = null;

        [Range(0f, 2f)] [SerializeField] private float fontHeightScaling = 1f;
        [Range(-1f, 1f)][SerializeField] private float fontHeightShift = 0f;

        public void OnValidate () {
            // Default layout driver
            if (layoutDriver == null && transform.parent != null) {
                layoutDriver = transform.parent.GetComponent<ButtonLayoutStyle>();
            }
            RefreshLayoutDeferred();
        }

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

        // -----------------------------------------------

        protected override void RefreshLayout() {

            // Override sizes
            if (layoutDriver != null && layoutDriver.isActiveAndEnabled) {
                maxWidth = layoutDriver.Width;
                height = layoutDriver.Height;
                cropWidth = layoutDriver.CropWidth;
                LayoutPaddingPixels = layoutDriver.LayoutPadding;
                fontHeightScaling = layoutDriver.FontHeightScaling;
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