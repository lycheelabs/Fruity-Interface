using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a UIButton that passes behaviour to a ToggleButtonEffect component.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ToggleButton : ButtonNode, ClickTarget {

        public TextMeshProUGUI ButtonText;
        public RectTransform ButtonBounds;
        public Image ButtonIcon;
        public Sprite OnSprite;
        public Sprite OffSprite;

        private BoxCollider _boxCollider = null;
        public BoxCollider BoxCollider {
            get {
                _boxCollider = _boxCollider ?? GetComponent<BoxCollider>();
                return _boxCollider;
            }
        }

        public void Initialise() {
            _boxCollider = GetComponent<BoxCollider>();
        }

        public void Configure(string text, float height, float width) {
            ButtonText.text = text;
            ButtonText.fontSizeMax = height * 0.95f;
            ButtonText.fontSizeMin = height * 0.4f;
            ButtonText.rectTransform.offsetMin = new Vector2(height * 1.25f, 0);

            ButtonBounds.sizeDelta = new Vector2(height, height) * 0.9f;

            var size = new Vector2(width, height);
            rectTransform.sizeDelta = size;
            LayoutSizePixels = size;

            BoxCollider.size = new Vector3(height, height, 0);
            BoxCollider.center = new Vector3(-width / 2f + height / 2f, -1);

            AnimationScaling = 2;
        }

        public void OnValidate() {
            if (ButtonIcon) ButtonIcon.sprite = OnSprite;
        }

        // ------------------------------------------------------------------

        private ToggleButtonEffect _effect;
        public ToggleButtonEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<ToggleButtonEffect>();
                return _effect;
            }
        }

        protected sealed override void OnUpdate() {
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

        public sealed override void MouseClick(ClickParams clickParams) {
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
        }

    }

}