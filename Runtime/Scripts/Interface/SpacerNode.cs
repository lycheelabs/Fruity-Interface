using UnityEditor;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class SpacerNode : LayoutNode {

        public float Size = 50;

        private void OnValidate () {
            Size = Mathf.Max(Size, 0);
            LayoutSizePixels = new Vector2(Size, Size);
            LayoutPaddingPixels = default;
            ApplySizeDeferred();
        }

    }

}