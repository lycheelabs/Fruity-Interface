using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a ClickButton using the ImageButton prefab.
    /// </summary>
    public class ImageButton : ClickButton {

        public static ImageButton SpawnDefault () {
            return FruityUIPrefabs.NewImageButton().GetComponent<ImageButton>();
        }

        public Image ButtonImage;

        private BoxCollider boxCollider;
        public BoxCollider BoxCollider {
            get {
                boxCollider = boxCollider ?? GetComponent<BoxCollider>();
                return boxCollider;
            }
        }

        public void ConfigureSprite(Sprite sprite) {
            ButtonImage.sprite = sprite;
        }

        public void ConfigureSize(float size) {
            var rectSize = new Vector2(size, size);
            ConfigureSize(rectSize, rectSize);
        }

        public void ConfigureSize(Vector2 rectSize) {
            ConfigureSize(rectSize, rectSize);
        }

        public void ConfigureSize (Vector2 imageSize, Vector2 colliderSize) {
            rectTransform.sizeDelta = imageSize;
            ButtonImage.rectTransform.sizeDelta = imageSize;
            LayoutSizePixels = imageSize;
            BoxCollider.size = colliderSize;
        }

    }

}