using TMPro;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class SettingNode : ControlNode {

        // Prefab references
        public TextMeshProUGUI TextField;
        public RectTransform TextBounds;
        public LayoutNode ControlNode;
        public RectTransform ControlBounds;

        [SerializeField] private string text = "Setting text";
        [SerializeField] private float width = 200;
        [SerializeField] private float height = 50;
        [SerializeField][Range(0,1)] private float fontHeightScaling = 0.75f;
        [SerializeField][Range(0,1)] private float contentRatio = 0.75f;

        public void SetText (string text) {
            TextField.text = text;
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

            // Apply size to self
            LayoutSizePixels = new Vector2(width, height);
            rectTransform.sizeDelta = new Vector2(width, height);

            var textW = width * contentRatio;
            var controlW = width - textW;
            TextBounds.sizeDelta = new Vector2(textW, height);
            ControlBounds.sizeDelta = new Vector2(controlW, height);

            TextField.text = text;
            TextField.fontSizeMax = height * 0.75f * fontHeightScaling;
            TextField.fontSizeMin = height * 0.35f * fontHeightScaling;

            // Apply size to control
            if (ControlNode != null) {
                var controlSize = ControlNode.TotalSizePixels;
                var controlScaleX = controlW / controlSize.x;
                var controlScaleY = height / controlSize.y;
                var controlScale = Mathf.Min(controlScaleX, controlScaleY);
                ControlNode.transform.localScale = Vector3.one * controlScale * 0.9f;
            }

        }

        // ------------------------------------------------------------------

        private ToggleEffect _effect;
        public ToggleEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<ToggleEffect>();
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