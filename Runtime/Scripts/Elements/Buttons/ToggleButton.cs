using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    public class ToggleButton : LayoutNode, ClickTarget {

        // Config
        public Sprite OnSprite;
        public Sprite OffSprite;
        public Color OnColor = Color.yellowNice;
        public Color OffColor = Color.gray6;
        public Color backingColor = Color.gray2;

        // Prefab references
        public Image Backing;
        public Image Border;
        public Image Dot;
        public Image Icon;
        public JuicyAnimator Animator;

        // Animation
        private bool isActive;
        private float activeTween;

        private bool isHighlighted;
        private float highlightTween;

        private bool isPressed;
        private float pressTween;

        private void Awake() {
            Backing.color = backingColor;
            Icon.color = backingColor;
            JumpTo(true);
        }

        private void Update() {
            activeTween = activeTween.MoveTowardsUnscaled(isActive, 12);
            var color = Color.Lerp(OffColor, OnColor, activeTween);
            Border.color = color;
            Dot.color = color;
            Dot.transform.localPosition = new Vector3(36 * (activeTween - 0.5f), 0, 0);
    
            highlightTween = highlightTween.MoveTowardsUnscaled(isHighlighted, 10);
            pressTween = pressTween.MoveTowardsUnscaled(isPressed, 12);
            Animator.BaseScale = Vector3.one * (1 + 0.18f * Tweens.EaseInOutQuad(highlightTween));
            Animator.BasePosition = new Vector3(26 * pressTween * (activeTween - 0.5f), 0);

            Dot.transform.localScale = Vector3.one * (1 + 0.3f * pressTween);
        }

        public void JumpTo (bool active) {
            isActive = active;
            activeTween = active ? 1 : 0;
            Icon.sprite = active ? OnSprite : OffSprite;
            var color = active ? OnColor : OffColor;
            Border.color = color;
            Dot.color = color;
        }

        private void AnimateTo (bool active) {
            isActive = active;
            Icon.sprite = active ? OnSprite : OffSprite;
            var direction = new Vector3(isActive ? 1 : -1, 0, 0);
            Animator.Nudge(direction, 18f, 0.8f);
        }

        // ------------------------------------------------------------------

        public void ApplyMouseClick(ClickParams clickParams) {
            AnimateTo(!isActive);
        }

        public void UpdateMouseHover(bool isFirstFrame, HoverParams highlightParams) {
            isHighlighted = true;
            isPressed = highlightParams.PressButton == MouseButton.Left;
        }

        public void EndMouseHover() {
            isHighlighted = false;
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