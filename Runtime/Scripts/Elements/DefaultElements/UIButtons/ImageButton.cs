using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a ClickButton using the ImageButton prefab.
    /// </summary>
    public class ImageButton : ClickButton {

        public Image ButtonImage;

        private BoxCollider boxCollider;
        public BoxCollider BoxCollider {
            get {
                boxCollider = boxCollider ?? GetComponent<BoxCollider>();
                return boxCollider;
            }
        }

        [SerializeField] private Sprite sprite = null;
        [SerializeField] private Vector2 size = new Vector2(50, 50);
        [SerializeField] private float colliderPadding = 10;

        private void OnValidate () {
            RefreshLayout();
        }

        public void ConfigureSprite(Sprite sprite) {
            this.sprite = sprite;
            RefreshLayout();
        }

        public void ConfigureSize(float size) {
            this.size = new Vector2(size, size);
            RefreshLayout();
        }

        public void ConfigureSize(Vector2 size) {
            this.size = size;
            RefreshLayout();
        }

        public void RefreshLayout () {
            ButtonImage.sprite = sprite;
            ApplySizeDeferred();
        }

        protected override void ApplySize () {
            if (this != null) {
                rectTransform.sizeDelta = size;
                ButtonImage.rectTransform.sizeDelta = size;
                LayoutSizePixels = size;
                BoxCollider.size = size + new Vector2(colliderPadding, colliderPadding);
            }
        }

    }

}