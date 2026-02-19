using TMPro;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a UIButton that passes behaviour to a ToggleButtonEffect component.
    /// </summary>
    public class ToggleField : ControlNode, ControlLayoutDriver {

        public IconButton ToggleButton;
        public RectTransform FieldBounds;
        public TextMeshProUGUI FieldText;

        public Sprite OnSprite;
        public Sprite OffSprite;

        [SerializeField] private float width = 200;
        [SerializeField] private float height = 50;
        [SerializeField] private float fontHeightScaling = 1;
        [SerializeField] private float fieldMargin = 20;

        private float FieldWidth => Mathf.Max(height, width - height - fieldMargin);

        public float DrivenWidth => FieldWidth;
        public float DrivenHeight => height;
        public Vector2 DrivenLayoutPadding => default;
        public bool DrivenCropWidth => false;
        public float DrivenFontHeightScaling => fontHeightScaling;

        public void SetText (string text) {
            FieldText.text = text;
            FieldText.rectTransform.offsetMin = new Vector2(height * 1.25f, 0);

            var size = new Vector2(width, height);
            rectTransform.sizeDelta = size;
            LayoutSizePixels = size;
        }

        //public new void OnValidate() {
            //ToggleButton.SetIcon(OnSprite);
        //}

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
                childDriver.width = FieldWidth;
                childDriver.height = height;
                childDriver.layoutPadding = default;
                childDriver.fontHeightScaling = fontHeightScaling;

                ToggleButton.BasePosition = new Vector3(height / 2f, 0);
                ToggleButton.RefreshLayoutDeferred();
            }

            // Apply size to self
            LayoutSizePixels = new Vector2(width, height);
            rectTransform.sizeDelta = new Vector2(width, height);
            FieldBounds.sizeDelta = new Vector2(FieldWidth, height);

            FieldText.fontSizeMax = height * 0.75f * fontHeightScaling;
            FieldText.fontSizeMin = height * 0.35f * fontHeightScaling;

        }

        // ------------------------------------------------------------------

        private ToggleButtonEffect _effect;
        public ToggleButtonEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<ToggleButtonEffect>();
                return _effect;
            }
        }

        /*protected sealed override void OnUpdate() {
            if (TryGetEffect != null) {
                ButtonIcon.sprite = TryGetEffect.IsToggledOn ? OnSprite : OffSprite;
            } else {
                ButtonIcon.sprite = OnSprite;
            }
        }

        protected override void OnHighlight(bool firstFrame, HoverParams highlightParams) {
            if (firstFrame && TryGetEffect != null) {
                TryGetEffect.MouseOver();
            }
        }

        protected override void OnDehighlight() {
            //
        }

        public sealed override void ApplyMouseClick(ClickParams clickParams) {
            ButtonAnimator.Squash();
            if (TryGetEffect != null && TryGetEffect.MouseButtonIsPermitted(clickParams.ClickButton)) {
                TryGetEffect.Toggle();
            }
        }

        bool ClickTarget.TryMouseUnclick(ClickParams clickParams) {
            return true;
        }

        public void SetEntered(float tween) {
            if (TryGetEffect != null) {
                Offset = (1 - tween) * TryGetEffect.EntryOffset;
            }
        }

        public void SetUpAs(bool value) {
            if (TryGetEffect != null) {
                TryGetEffect.SetUpAs(value);
            }
        }*/

    }

}