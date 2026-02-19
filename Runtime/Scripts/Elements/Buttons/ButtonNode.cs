using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary> Abstract implementation of a simple button. </summary>
    public abstract class ButtonNode : ControlNode, ClickTarget, EnteringElement {

        public float BaseScale = 1f;
        public float AnimationScaling = 1f;

        public bool IsHighlighted { get; protected set; }
        private float highlightTween;

        public bool IsHeld { get; private set; }
        private float heldTween;

        public JuicyAnimator ButtonAnimator;

        private Vector3 basePosition;
        public Vector3 BasePosition {
            get => basePosition;
            set { basePosition = value; rectTransform.anchoredPosition = basePosition + offset; }
        }

        private Vector3 offset;
        public Vector3 Offset {
            get => offset;
            set { offset = value; rectTransform.anchoredPosition = basePosition + offset; }
        }

        public Anchor Anchor {
            set { rectTransform.SetAnchor(value); }
        }

        private ClickButtonEffect _effect;
        public ClickButtonEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<ClickButtonEffect>();
                return _effect;
            }
        }

        private void Update () {
            highlightTween = highlightTween.MoveTowardsUnscaled(IsHighlighted, 8);
            heldTween = heldTween.MoveTowardsUnscaled(IsHeld, 8);
            AnimateHover(highlightTween, heldTween);
            OnUpdate();
        }

        protected abstract void AnimateHover (float highlightTween, float heldTween);
        protected abstract void AnimateClick ();

        public void UpdateMouseHover (bool firstFrame, HoverParams highlightParams) {
            IsHighlighted = true;
            IsHeld = highlightParams.PressButton != MouseButton.None;
            OnHighlight(firstFrame, highlightParams);
        }

        public void EndMouseHover () {
            IsHighlighted = false;
            IsHeld = false;
            OnDehighlight();
        }

        protected virtual void OnUpdate() { }

        public virtual void ApplyMouseClick (ClickParams clickParams) {
            AnimateClick();

            if (TryGetEffect == null) {
                Debug.LogWarning("ButtonNode is missing a ClickButtonEffect.");
                return;
            }
            if (TryGetEffect.MouseButtonIsPermitted(clickParams.ClickButton)) {
                TryGetEffect.Activate(clickParams.ClickButton);
            }
        }

        bool ClickTarget.TryMouseUnclick(ClickParams clickParams) {
            if (TryGetEffect != null) {
                return TryGetEffect.TryUnclick(clickParams.ClickButton);
            }
            return true;
        }

        public void SetEnterTween (float tween) {
            if (TryGetEffect != null) {
                Offset = (1 - tween) * TryGetEffect.EntryOffset;
            }
        }

        protected virtual void OnHighlight (bool firstFrame, HoverParams highlightParams) {
            if (firstFrame && TryGetEffect != null) {
                TryGetEffect.MouseOver();
            }
        }

        protected virtual void OnDehighlight () {
            //
        }

    }

}
