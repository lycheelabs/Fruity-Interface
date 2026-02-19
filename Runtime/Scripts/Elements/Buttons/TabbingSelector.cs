using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public enum TabbingSelectorComponent {
        Main,
        LeftArrow,
        RightArrow
    }

    public class TabbingSelector : ControlNode, ControlLayoutDriver {

        public TextButton MainButton;
        public IconButton LeftArrow;
        public IconButton RightArrow;

        [SerializeField] private float width = 200;
        [SerializeField] private float height = 50;
        [SerializeField] private float fontHeightScaling = 1;
        [SerializeField] private float iconScaling = 1;
        [SerializeField] private float arrowMargin = 20;

        private float ButtonWidth => Mathf.Max(height, width - height * iconScaling * 2 - arrowMargin);

        public float DrivenWidth => ButtonWidth;
        public float DrivenHeight => height;
        public Vector2 DrivenLayoutPadding => default;
        public bool DrivenCropWidth => false;
        public float DrivenFontHeightScaling => fontHeightScaling;
        public float DrivenIconScale => iconScaling;
        public float DrivenInteriorMargins => arrowMargin;

        private TabbingSelectorEffect _effect;
        public TabbingSelectorEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<TabbingSelectorEffect>();
                return _effect;
            }
        }

        protected override void RefreshLayout () {

            // Override layout
            if (LayoutDriver != null && LayoutDriver.isActiveAndEnabled) {
                width = LayoutDriver.width;
                height = LayoutDriver.height;
                LayoutPaddingPixels = LayoutDriver.layoutPadding;
                fontHeightScaling = LayoutDriver.fontHeightScaling;
                iconScaling = LayoutDriver.iconScaling;
                arrowMargin = LayoutDriver.interiorMargins;
            }

            // Pass layout to children
            var childDriver = GetComponent<ControlLayoutStyle>();
            if (childDriver != null) {
                childDriver.width = ButtonWidth;
                childDriver.height = height;
                childDriver.layoutPadding = default;
                childDriver.fontHeightScaling = fontHeightScaling;
                childDriver.iconScaling = iconScaling;
                childDriver.interiorMargins = 0;

                LeftArrow.BasePosition = new Vector3(height * iconScaling / 2f, 0);
                RightArrow.BasePosition = new Vector3(-height * iconScaling / 2f, 0);

                MainButton.RefreshLayoutDeferred();
                LeftArrow.RefreshLayoutDeferred();
                RightArrow.RefreshLayoutDeferred();
            }

            // Apply size to self
            LayoutSizePixels = new Vector2(width, height);
            rectTransform.sizeDelta = new Vector2(width, height);

        }

        public void Activate (TabbingSelectorComponent type, MouseButton clickButton) {
            if (TryGetEffect != null) {
                TryGetEffect.Activate(type, clickButton);
            } else {
                Debug.LogWarning("TabbingSelectorEffect component not found on " + name);
            }
        }

        public void MouseOver (TabbingSelectorComponent type) {
            if (TryGetEffect != null) {
                TryGetEffect.MouseOver(type);
            }
        }

        public void UpdateLayout (TabbingSelectorComponent type) {
            //
        }

    }

}