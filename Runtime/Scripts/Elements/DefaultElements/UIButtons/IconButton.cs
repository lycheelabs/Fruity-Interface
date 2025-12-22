using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a ClickButton using the IconButton prefab.
    /// </summary>
    public class IconButton : ClickButton {

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
            RefreshLayoutDeferred();
        }

        private void OnEnable () {
            RefreshLayoutDeferred();
        }

        public void ConfigureSprite(Sprite sprite) {
            this.sprite = sprite;
            ButtonImage.sprite = sprite;
            RefreshLayoutDeferred();
        }

        public void ConfigureSize(float size) {
            this.size = new Vector2(size, size);
            RefreshLayoutDeferred();
        }

        public void ConfigureSize(Vector2 size) {
            this.size = size;
            RefreshLayoutDeferred();
        }

        protected override void RefreshLayout () {
            rectTransform.sizeDelta = size;
            ButtonImage.rectTransform.sizeDelta = size;
            LayoutSizePixels = size;
            BoxCollider.size = size + new Vector2(colliderPadding, colliderPadding);
        }

    }

}