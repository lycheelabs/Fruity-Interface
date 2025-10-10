using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a ClickButton using the TextButton prefab.
    /// </summary>
    public class TextButton : ClickButton {

        public float FontScale = 1f;

        public static TextButton SpawnDefault () {
            return FruityUIPrefabs.NewTextButton().GetComponent<TextButton>();
        }

        public Image BackingImage;
        public TextMeshProUGUI ButtonText;
        public Image ButtonIcon;

        private BoxCollider _boxCollider;
        public BoxCollider BoxCollider {
            get {
                _boxCollider = _boxCollider ?? GetComponent<BoxCollider>();
                return _boxCollider;
            }
        }

        public void Configure(string text, bool crop = false, Sprite icon = null) {
            Configure(text, LayoutSizePixels.y, LayoutSizePixels.x, crop, icon);
        }

        public void Configure (string text, float height, float maxWidth, bool crop = false, Sprite icon = null) {
            var hasIcon = icon != null;

            ButtonText.text = text;
            ButtonIcon.sprite = icon;
            ButtonIcon.gameObject.SetActive (hasIcon);

            ButtonText.fontSizeMax = height * 0.7f * FontScale;
            ButtonText.fontSizeMin = height * 0.4f * FontScale;

            ButtonIcon.rectTransform.sizeDelta = new Vector2(height, height) * 0.8f;
            ButtonIcon.rectTransform.localPosition = new Vector3(height * 0.5f, 0);

            var textWidth = ButtonText.preferredWidth;
            if (hasIcon) textWidth += height;
            var width = Mathf.Min(maxWidth, textWidth + 32);
            if (!crop) width = maxWidth;

            if (hasIcon) {
                ButtonText.rectTransform.offsetMin = new Vector2(height + 4, 5);
            } else {
                ButtonText.rectTransform.offsetMin = new Vector2(16, 5);
            }

            var size = new Vector2(width, height);
            rectTransform.sizeDelta = size;
            BoxCollider.size = size;
            LayoutSizePixels = size;
        }

    }

}