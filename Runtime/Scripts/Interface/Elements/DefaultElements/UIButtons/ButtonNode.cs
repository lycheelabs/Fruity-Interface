using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary> Abstract implementation of a simple button. </summary>
    public abstract class ButtonNode : LayoutNode, ClickTarget {

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

        private void Update () {
            highlightTween = highlightTween.MoveTowardsUnscaled(IsHighlighted, 8);
            heldTween = heldTween.MoveTowardsUnscaled(IsHeld, 8);
            Animate(highlightTween, heldTween);
            OnUpdate();
        }

        protected virtual void Animate (float highlightTween, float heldTween) {
            var scaleShift = 0.1f * Tweens.EaseOutQuad(highlightTween) - 0.07f * Tweens.EaseOutQuad(heldTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.BaseScale = Vector3.one * highlightScale * BaseScale;
        }

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
        public abstract void ApplyMouseClick (ClickParams clickParams);
        protected abstract void OnHighlight (bool firstFrame, HoverParams highlightParams);
        protected abstract void OnDehighlight ();

    }

}