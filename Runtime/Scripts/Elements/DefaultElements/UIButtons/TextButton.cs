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

        private BoxCollider _boxCollider;
        public BoxCollider BoxCollider {
            get {
                _boxCollider = _boxCollider ?? GetComponent<BoxCollider>();
                return _boxCollider;
            }
        }

        [SerializeField] public ButtonLayoutStyle layoutDriver = null;
        [SerializeField] private float height = 50;
        [SerializeField] private float maxWidth = 200;
        [SerializeField] private bool cropWidth = false;
        [SerializeField] private Sprite iconSprite = null;
        [Range(0f, 2f)] [SerializeField] private float fontHeightScaling = 1f;

        public void OnValidate () {
            // Default layout driver
            if (layoutDriver == null && transform.parent != null) {
                layoutDriver = transform.parent.GetComponent<ButtonLayoutStyle>();
            }
            RefreshLayout();
        }

        public void SetText (string text) {
            ButtonText.text = text;
            RefreshLayout();
        }

        public void SetWidth (float maxWidth, bool crop = false) {
            this.maxWidth = maxWidth;
            this.cropWidth = crop;
            RefreshLayout();
        }

        public void SetHeight (float height) {
            this.height = height;
            SetFontScale(fontHeightScaling);
            RefreshLayout();
        }

        public void SetFontScale (float heightScale) {
            this.fontHeightScaling = heightScale;
            RefreshLayout();
        }

        // -----------------------------------------------

        private void RefreshLayout () {

            // Override sizes
            if (layoutDriver != null) {
                maxWidth = layoutDriver.Width;
                height = layoutDriver.Height;
                cropWidth = layoutDriver.CropWidth;
                LayoutPaddingPixels = layoutDriver.LayoutPadding;
                fontHeightScaling = layoutDriver.FontHeightScaling;
            }

            // Udpate size
            ApplySizeDeferred();

            // Update icon
            var hasIcon = iconSprite != null;
            IconImage.sprite = iconSprite;
            IconImage.gameObject.SetActive(hasIcon);


        }

        protected override void ApplySize () {
            if (this != null) {
                
                // Calculate sizes
                var hasIcon = iconSprite != null;
                var textWidth = ButtonText.preferredWidth;
                if (hasIcon) textWidth += height;
                var width = Mathf.Min(maxWidth, textWidth + 32);
                if (!cropWidth) width = maxWidth;
                var size = new Vector2(width, height);

                // Apply sizes
                rectTransform.sizeDelta = size;
                BoxCollider.size = size;
                LayoutSizePixels = size;
                IconImage.rectTransform.sizeDelta = size * 0.8f;
                IconImage.rectTransform.localPosition = new Vector3(size.y * 0.5f, 0);

                ButtonText.fontSizeMax = height * 0.7f * fontHeightScaling;
                ButtonText.fontSizeMin = height * 0.4f * fontHeightScaling;
                if (hasIcon) {
                    ButtonText.rectTransform.offsetMin = new Vector2(height + 4, 5);
                } else {
                    ButtonText.rectTransform.offsetMin = new Vector2(16, 5);
                }

            }
        }

    }

}