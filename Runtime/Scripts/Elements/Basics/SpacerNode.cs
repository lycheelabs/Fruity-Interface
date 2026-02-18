using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class SpacerNode : LayoutNode {

        [SerializeField] private float Size = 50;

        private void OnValidate () {
            Size = Mathf.Max(Size, 0);
            LayoutSizePixels = new Vector2(Size, Size);
            LayoutPaddingPixels = default;
        }

        public void SetSize (float newSize) {
            Size = Mathf.Max(newSize, 0);
            LayoutSizePixels = new Vector2(Size, Size);
        }

    }

}