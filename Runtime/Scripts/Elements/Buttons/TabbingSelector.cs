using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class TabbingSelector : ControlNode, ControlLayoutDriver {

        public enum Component {
            Main,
            LeftArrow,
            RightArrow
        }

        public TextButton MainButton;
        public IconButton LeftArrow;
        public IconButton RightArrow;

        [SerializeField] private float width = 200;
        [SerializeField] private float height = 50;
        [SerializeField] private float fontHeightScaling = 1;
        [SerializeField] private float arrowMargin = 20;

        private float ButtonWidth => Mathf.Max(height, width - height * 2 - arrowMargin);

        public float DrivenWidth => ButtonWidth;
        public float DrivenHeight => height;
        public Vector2 DrivenLayoutPadding => default;
        public bool DrivenCropWidth => false;
        public float DrivenFontHeightScaling => fontHeightScaling;

        protected override void RefreshLayout () {

            // Override layout
            if (LayoutDriver != null && LayoutDriver.isActiveAndEnabled) {
                width = LayoutDriver.width;
                height = LayoutDriver.height;
                LayoutPaddingPixels = LayoutDriver.layoutPadding;
                fontHeightScaling = LayoutDriver.fontHeightScaling;
            }

            // Pass layout to children
            var childDriver = GetComponent<ControlLayoutStyle>();
            if (childDriver != null) {
                childDriver.width = ButtonWidth;
                childDriver.height = height;
                childDriver.layoutPadding = default;
                childDriver.fontHeightScaling = fontHeightScaling;

                LeftArrow.BasePosition = new Vector3(height / 2f, 0);
                RightArrow.BasePosition = new Vector3(-height / 2f, 0);

                MainButton.RefreshLayoutDeferred();
                LeftArrow.RefreshLayoutDeferred();
                RightArrow.RefreshLayoutDeferred();
            }

            // Apply size to self
            LayoutSizePixels = new Vector2(width, height);
            rectTransform.sizeDelta = new Vector2(width, height);

        }

        public void Activate (Component type, MouseButton clickButton) {
            //
        }

        public void MouseOver (Component type) {
            //
        }

        public void UpdateLayout (Component type) {
            //
        }

    }

}